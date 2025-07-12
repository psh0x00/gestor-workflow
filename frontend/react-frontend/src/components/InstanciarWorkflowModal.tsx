import React, { useState, useEffect } from 'react';
import Modal from './Modal';
import { pesquisarUtilizadores } from '../services/utilizadorService';
import { criarWorkflowInstancia } from '../services/workflowInstanciaService';
import { useAuth } from '../context/AuthContext';

interface InstanciarWorkflowModalProps {
  isOpen: boolean;
  onClose: () => void;
  modeloId: number;
  funcoes?: string[]; // array de funções/roles do modelo
  estadoInicialId?: number; // NOVO: id do estado inicial
}

interface Utilizador {
  id: number;
  nome: string;
  email?: string;
}

const InstanciarWorkflowModal: React.FC<InstanciarWorkflowModalProps> = ({ isOpen, onClose, modeloId, funcoes = [], estadoInicialId }) => {
  const { user } = useAuth();
  const [inputValues, setInputValues] = useState<{ [funcao: string]: string }>({});
  const [validos, setValidos] = useState<{ [funcao: string]: boolean }>({});
  const [sugestoes, setSugestoes] = useState<{ [funcao: string]: Utilizador[] }>({});
  const [selecionados, setSelecionados] = useState<{ [funcao: string]: Utilizador | null }>({});
  const [showSugestoes, setShowSugestoes] = useState<{ [funcao: string]: boolean }>({});

  // Buscar utilizadores do backend conforme o input
  const handleInput = async (funcao: string, nome: string) => {
    setInputValues(prev => ({ ...prev, [funcao]: nome }));
    setShowSugestoes(prev => ({ ...prev, [funcao]: true }));
    if (nome.length < 2) {
      setSugestoes(prev => ({ ...prev, [funcao]: [] }));
      setValidos(prev => ({ ...prev, [funcao]: false }));
      setSelecionados(prev => ({ ...prev, [funcao]: null }));
      return;
    }
    try {
      const lista: Utilizador[] = await pesquisarUtilizadores(nome);
      setSugestoes(prev => ({ ...prev, [funcao]: lista }));
      // Tentar encontrar utilizador por nome ou email exatos
      const normalizar = (s: string) => s.trim().toLowerCase();
      const nomeInput = normalizar(nome);
      const utilizador = lista.find(u =>
        normalizar(u.nome) === nomeInput ||
        (u.email && normalizar(u.email) === nomeInput) ||
        normalizar(`${u.nome} (${u.email})`) === nomeInput
      );
      setValidos(prev => ({ ...prev, [funcao]: !!utilizador }));
      setSelecionados(prev => ({ ...prev, [funcao]: utilizador || null }));
    } catch {
      setSugestoes(prev => ({ ...prev, [funcao]: [] }));
      setValidos(prev => ({ ...prev, [funcao]: false }));
      setSelecionados(prev => ({ ...prev, [funcao]: null }));
    }
  };

  // Quando o utilizador clica numa sugestão
  const handleSelect = (funcao: string, utilizador: Utilizador) => {
    setInputValues(prev => ({ ...prev, [funcao]: utilizador.nome + (utilizador.email ? ` (${utilizador.email})` : '') }));
    setSelecionados(prev => ({ ...prev, [funcao]: utilizador }));
    setValidos(prev => ({ ...prev, [funcao]: true }));
    setShowSugestoes(prev => ({ ...prev, [funcao]: false }));
  };

  // Associar automaticamente o utilizador logado se só houver uma função
  useEffect(() => {
    if (isOpen && funcoes.length === 1 && user) {
      (async () => {
        const lista = await pesquisarUtilizadores(user);
        const utilizador = lista.find((u: Utilizador) => u.email?.toLowerCase() === user.toLowerCase());
        if (utilizador) {
          setInputValues({ [funcoes[0]]: utilizador.nome + (utilizador.email ? ` (${utilizador.email})` : '') });
          setSelecionados({ [funcoes[0]]: utilizador });
          setValidos({ [funcoes[0]]: true });
        }
      })();
    }
  }, [isOpen, funcoes, user]);

  const handleIniciar = async () => {
    if (!funcoes.every(f => selecionados[f])) return;
    const equipa = funcoes.map(f => ({ funcao: f, utilizadorId: selecionados[f]!.id }));
    const token = localStorage.getItem('token') || undefined;
    // Tenta identificar o utilizador logado na equipa e já marcar como confirmado
    let equipaComConfirmado = equipa;
    if (user) {
      // Procura o utilizador logado na lista de selecionados
      const utilizadorLogado = Object.values(selecionados).find(u => u && (u.email?.toLowerCase() === user.toLowerCase() || u.nome?.toLowerCase() === user.toLowerCase()));
      if (utilizadorLogado) {
        equipaComConfirmado = equipa.map(e =>
          e.utilizadorId === utilizadorLogado.id ? { ...e, confirmado: true } : e
        );
      }
    }
    try {
      await criarWorkflowInstancia({
        workflowModeloId: modeloId,
        estadoInicialId, // NOVO: envia o estado inicial
        equipa: equipaComConfirmado
      }, token);
      onClose();
    } catch (e) {
      alert('Erro ao instanciar workflow.');
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <div style={{ minWidth: 320, maxWidth: 600, padding: 24 }}>
        <h2 style={{ marginTop: 0 }}>Equipa</h2>
        <form style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
          {funcoes.length === 0 && <div style={{ color: '#888' }}>Nenhuma função definida para este modelo.</div>}
          {funcoes.map(funcao => {
            const isValid = validos[funcao] || false;
            const sugestoesLista = sugestoes[funcao] || [];
            const inputValue = inputValues[funcao] || '';
            return (
              <div key={funcao} style={{ display: 'flex', flexDirection: 'column', gap: 2, position: 'relative', marginBottom: 12 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                  <span style={{ minWidth: 90, fontWeight: 500 }}>{funcao}</span>
                  <input
                    type="text"
                    placeholder="Pesquisar utilizador..."
                    value={inputValue}
                    onChange={e => handleInput(funcao, e.target.value)}
                    style={{ flex: 1, padding: 8, borderRadius: 6, border: isValid || !inputValue ? '1px solid #ddd' : '1.5px solid #e53e3e', background: isValid || !inputValue ? '#fff' : '#ffeaea' }}
                    autoComplete="off"
                    onFocus={() => setShowSugestoes(prev => ({ ...prev, [funcao]: true }))}
                  />
                  {!isValid && inputValue && (
                    <span style={{ color: '#e53e3e', fontSize: 13 }}>Utilizador não registado</span>
                  )}
                </div>
                {/* Sugestões dropdown */}
                {showSugestoes[funcao] && sugestoesLista.length > 0 && (
                  <div style={{ position: 'absolute', left: 110, right: 0, top: 38, zIndex: 20, background: '#fff', border: '1px solid #ddd', borderRadius: 6, boxShadow: '0 2px 8px #0002', maxHeight: 180, overflowY: 'auto' }}>
                    {sugestoesLista.map(u => (
                      <div
                        key={u.id}
                        style={{ padding: '7px 12px', cursor: 'pointer', color: '#222', fontSize: 15, borderBottom: '1px solid #f3f3f3' }}
                        onMouseDown={e => { e.preventDefault(); handleSelect(funcao, u); }}
                      >
                        {u.nome} {u.email && <span style={{ color: '#888', fontSize: 13 }}>({u.email})</span>}
                      </div>
                    ))}
                  </div>
                )}
              </div>
            );
          })}
        </form>
        <button
          onClick={handleIniciar}
          className="novo-modelo-btn"
          style={{ alignSelf: 'flex-end', padding: '10px 24px', borderRadius: 6, marginTop: 24 }}
          disabled={!funcoes.every(f => selecionados[f])}
        >Iniciar</button>
      </div>
    </Modal>
  );
};

export default InstanciarWorkflowModal;
