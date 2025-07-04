import React, { useState } from 'react';
import Modal from './Modal';
import EstadoFuncoesModal from './EstadoFuncoesModal';
import './Modal.css';
import { criarWorkflowModelo } from '../services/workflowModeloService';

interface Estado {
  nome: string;
  descricao?: string;
  preCondicao?: string;
  posCondicao?: string;
}

interface EstadoComTipo extends Estado {
  tipo: 'inicial' | 'final' | 'intermedio';
  corHexadecimal: string;
  funcoes: string[];
}

interface Transicao {
  origem: string;
  destino: string;
}

interface WorkflowModeloModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: any) => void;
}

const corPorTipo = {
  inicial: '#4CAF50', // verde
  intermedio: '#FFC107', // amarelo
  final: '#F44336', // vermelho
};

const WorkflowModeloModal: React.FC<WorkflowModeloModalProps> = ({ isOpen, onClose, onSubmit }) => {
  const [nome, setNome] = useState('');
  const [descricao, setDescricao] = useState('');
  const [estados, setEstados] = useState<Estado[]>([]);
  const [novoEstado, setNovoEstado] = useState<Estado>({ nome: '', descricao: '', preCondicao: '', posCondicao: '' });
  const [funcoes, setFuncoes] = useState<string[]>([]);
  const [novaFuncao, setNovaFuncao] = useState('');
  const [estadoInicialIdx, setEstadoInicialIdx] = useState<number | null>(null);
  const [estadoFinalIdx, setEstadoFinalIdx] = useState<number | null>(null);
  const [estadoFuncoesModalOpenIdx, setEstadoFuncoesModalOpenIdx] = useState<number | null>(null);
  const [funcoesPorEstado, setFuncoesPorEstado] = useState<{ [idx: number]: string[] }>({});
  const [mensagem, setMensagem] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [transicoes, setTransicoes] = useState<Transicao[]>([]);
  const [novaTransicao, setNovaTransicao] = useState<Transicao>({ origem: '', destino: '' });

  const adicionarEstado = () => {
    if (novoEstado.nome.trim() !== '') {
      setEstados([...estados, novoEstado]);
      setNovoEstado({ nome: '', descricao: '', preCondicao: '', posCondicao: '' });
    }
  };

  const removerEstado = (index: number) => {
    setEstados(estados.filter((_, i) => i !== index));
    if (estadoInicialIdx === index) setEstadoInicialIdx(null);
    if (estadoFinalIdx === index) setEstadoFinalIdx(null);
    if (estadoInicialIdx !== null && index < estadoInicialIdx) setEstadoInicialIdx(estadoInicialIdx - 1);
    if (estadoFinalIdx !== null && index < estadoFinalIdx) setEstadoFinalIdx(estadoFinalIdx - 1);
  };

  const adicionarFuncao = () => {
    if (novaFuncao.trim() !== '' && !funcoes.includes(novaFuncao.trim())) {
      setFuncoes([...funcoes, novaFuncao.trim()]);
      setNovaFuncao('');
    }
  };

  const removerFuncao = (index: number) => {
    setFuncoes(funcoes.filter((_, i) => i !== index));
  };

  const handleFuncoesEstadoChange = (idx: number, funcoesSelecionadas: string[]) => {
    setFuncoesPorEstado(prev => ({ ...prev, [idx]: funcoesSelecionadas }));
  };

  const adicionarTransicao = () => {
    if (novaTransicao.origem && novaTransicao.destino && novaTransicao.origem !== novaTransicao.destino) {
      setTransicoes([...transicoes, { ...novaTransicao }]);
      setNovaTransicao({ origem: '', destino: '' });
    }
  };

  const removerTransicao = (idx: number) => {
    setTransicoes(transicoes.filter((_, i) => i !== idx));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setMensagem(null);
    if (
      nome.trim() &&
      estados.length > 1 &&
      estadoInicialIdx !== null &&
      estadoFinalIdx !== null &&
      estadoInicialIdx !== estadoFinalIdx
    ) {
      setLoading(true);
      // Construir estados com tipo, cor e flag inicial/final
      const estadosComTipo = estados.map((estado, idx) => {
        let tipo: 'inicial' | 'final' | 'intermedio' = 'intermedio';
        if (idx === estadoInicialIdx) tipo = 'inicial';
        else if (idx === estadoFinalIdx) tipo = 'final';
        return {
          ...estado,
          tipo,
          corHexadecimal: corPorTipo[tipo],
          funcoes: funcoesPorEstado[idx] || [],
          isInicial: idx === estadoInicialIdx,
          isFinal: idx === estadoFinalIdx
        };
      });

      // Mapear para DTO do backend (sem estadoInicialId e sem criadoPorId)
      const criarWorkflowModeloDTO = {
        nome,
        descricao,
        estados: estadosComTipo.map(e => ({
          nome: e.nome,
          descricao: e.descricao,
          tipo: e.tipo === 'inicial' ? 1 : e.tipo === 'final' ? 3 : 2,
          corHexadecimal: e.corHexadecimal,
          funcoes: e.funcoes,
          isInicial: e.isInicial,
          isFinal: e.isFinal
        })),
        transicoes: transicoes.map(t => ({
          origem: t.origem,
          destino: t.destino
        }))
      };
      try {
        // Obter token JWT do localStorage/sessionStorage/cookie conforme sua implementação
        const token = localStorage.getItem('token');
        await criarWorkflowModelo(criarWorkflowModeloDTO, token ?? undefined);
        setNome('');
        setDescricao('');
        setEstados([]);
        setFuncoes([]);
        setEstadoInicialIdx(null);
        setEstadoFinalIdx(null);
        setMensagem('Workflow criado com sucesso!');
        setLoading(false);
        setTimeout(() => {
          setMensagem(null);
          onClose();
        }, 1200);
      } catch (error: any) {
        setLoading(false);
        setMensagem('Erro ao criar workflow.');
      }
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <h2 style={{ marginBottom: 24 }}>Criar Modelo de Workflow</h2>
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
        <label style={{ fontWeight: 500 }}>Nome do Workflow</label>
        <input
          className="input-padrao"
          value={nome}
          onChange={e => setNome(e.target.value)}
          required
          style={{ width: '100%', padding: 8, borderRadius: 6, border: '1px solid #ddd' }}
        />
        <label style={{ fontWeight: 500 }}>Descrição</label>
        <textarea
          className="input-padrao"
          value={descricao}
          onChange={e => setDescricao(e.target.value)}
          style={{ width: '100%', padding: 8, borderRadius: 6, border: '1px solid #ddd' }}
        />
        <h3 style={{ margin: '16px 0 8px 0' }}>Funções/Papéis</h3>
        <ul style={{ marginBottom: 8 }}>
          {funcoes.map((funcao, idx) => (
            <li key={idx} style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
              <span style={{ fontWeight: 500 }}>{funcao}</span>
              <button
                type="button"
                className="remover-btn"
                onClick={() => removerFuncao(idx)}
                style={{
                  background: 'linear-gradient(90deg, #fc466b 0%, #3f5efb 100%)',
                  color: '#fff',
                  border: 'none',
                  borderRadius: 6,
                  padding: '6px 12px',
                  cursor: 'pointer',
                  fontWeight: 500
                }}
              >
                Remover
              </button>
            </li>
          ))}
        </ul>
        <div style={{ display: 'flex', gap: 8, marginBottom: 8 }}>
          <input
            className="input-padrao"
            placeholder="Nova função/papel"
            value={novaFuncao}
            onChange={e => setNovaFuncao(e.target.value)}
            style={{ flex: 2, padding: 8, borderRadius: 6, border: '1px solid #ddd' }}
          />
          <button type="button" className="novo-modelo-btn" onClick={adicionarFuncao} style={{ padding: '8px 12px', borderRadius: 6 }}>
            Adicionar Função
          </button>
        </div>
        <h3 style={{ margin: '16px 0 8px 0' }}>Estados</h3>
        <div style={{ display: 'flex', gap: 16, marginBottom: 8 }}>
          <label style={{ fontWeight: 500 }}>
            Estado Inicial:
            <select
              value={estadoInicialIdx !== null ? estadoInicialIdx : ''}
              onChange={e => setEstadoInicialIdx(e.target.value === '' ? null : Number(e.target.value))}
              style={{ marginLeft: 8, padding: 6, borderRadius: 6, border: '1px solid #ddd' }}
            >
              <option value="">Selecione</option>
              {estados.map((estado, idx) => (
                <option key={idx} value={idx}>{estado.nome}</option>
              ))}
            </select>
          </label>
          <label style={{ fontWeight: 500 }}>
            Estado Final:
            <select
              value={estadoFinalIdx !== null ? estadoFinalIdx : ''}
              onChange={e => setEstadoFinalIdx(e.target.value === '' ? null : Number(e.target.value))}
              style={{ marginLeft: 8, padding: 6, borderRadius: 6, border: '1px solid #ddd' }}
            >
              <option value="">Selecione</option>
              {estados.map((estado, idx) => (
                <option key={idx} value={idx}>{estado.nome}</option>
              ))}
            </select>
          </label>
        </div>
        <ul style={{ marginBottom: 8 }}>
          {estados.map((estado, idx) => {
            let tipo: 'inicial' | 'final' | 'intermedio' = 'intermedio';
            if (idx === estadoInicialIdx) tipo = 'inicial';
            else if (idx === estadoFinalIdx) tipo = 'final';
            return (
              <li key={idx} style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                <span style={{ fontWeight: 500 }}>{estado.nome}</span>
                <span style={{ color: corPorTipo[tipo], fontWeight: 600, marginLeft: 8 }}>
                  {tipo.charAt(0).toUpperCase() + tipo.slice(1)}
                </span>
                <button
                  type="button"
                  title="Atribuir funções ao estado"
                  onClick={() => setEstadoFuncoesModalOpenIdx(idx)}
                  style={{
                    background: 'linear-gradient(90deg, #1565c0 0%, #1976d2 100%)',
                    border: 'none',
                    cursor: 'pointer',
                    padding: 6,
                    borderRadius: 6,
                    color: '#fff',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: 18
                  }}
                >
                  <span role="img" aria-label="perfil" style={{ filter: 'brightness(0) invert(1)' }}>👤</span>
                </button>
                <button
                  type="button"
                  className="remover-btn"
                  onClick={() => removerEstado(idx)}
                  style={{
                    background: 'linear-gradient(90deg, #fc466b 0%, #3f5efb 100%)',
                    color: '#fff',
                    border: 'none',
                    borderRadius: 6,
                    padding: '6px 12px',
                    cursor: 'pointer',
                    fontWeight: 500
                  }}
                >
                  Remover
                </button>
                <EstadoFuncoesModal
                  isOpen={estadoFuncoesModalOpenIdx === idx}
                  onClose={() => setEstadoFuncoesModalOpenIdx(null)}
                  funcoes={funcoes}
                  funcoesSelecionadas={funcoesPorEstado[idx] || []}
                  onChange={funcoesSel => handleFuncoesEstadoChange(idx, funcoesSel)}
                />
              </li>
            );
          })}
        </ul>
        <div style={{ display: 'flex', gap: 8, marginBottom: 8 }}>
          <input
            className="input-padrao"
            placeholder="Nome do Estado"
            value={novoEstado.nome}
            onChange={e => setNovoEstado({ ...novoEstado, nome: e.target.value })}
            style={{ flex: 2, padding: 8, borderRadius: 6, border: '1px solid #ddd' }}
          />
          <input
            className="input-padrao"
            placeholder="Descrição"
            value={novoEstado.descricao}
            onChange={e => setNovoEstado({ ...novoEstado, descricao: e.target.value })}
            style={{ flex: 3, padding: 8, borderRadius: 6, border: '1px solid #ddd' }}
          />
          <input
            className="input-padrao"
            placeholder="Pré-condição"
            value={novoEstado.preCondicao}
            onChange={e => setNovoEstado({ ...novoEstado, preCondicao: e.target.value })}
            style={{ flex: 2, padding: 8, borderRadius: 6, border: '1px solid #ddd' }}
          />
          <input
            className="input-padrao"
            placeholder="Pós-condição"
            value={novoEstado.posCondicao}
            onChange={e => setNovoEstado({ ...novoEstado, posCondicao: e.target.value })}
            style={{ flex: 2, padding: 8, borderRadius: 6, border: '1px solid #ddd' }}
          />
          <button type="button" className="novo-modelo-btn" onClick={adicionarEstado} style={{ padding: '8px 12px', borderRadius: 6 }}>
            Adicionar Estado
          </button>
        </div>
        <h3 style={{ margin: '16px 0 8px 0' }}>Transições</h3>
        <ul style={{ marginBottom: 8 }}>
          {transicoes.map((t, idx) => (
            <li key={idx} style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 4 }}>
              <span style={{ fontWeight: 500 }}><b>{t.origem}</b> <span style={{ color: '#888', fontWeight: 400 }}>→</span> <b>{t.destino}</b></span>
              <button
                type="button"
                className="remover-btn"
                onClick={() => removerTransicao(idx)}
                style={{
                  background: 'linear-gradient(90deg, #fc466b 0%, #3f5efb 100%)',
                  color: '#fff',
                  border: 'none',
                  borderRadius: 6,
                  padding: '6px 12px',
                  cursor: 'pointer',
                  fontWeight: 500
                }}
              >
                Remover
              </button>
            </li>
          ))}
        </ul>
        <div style={{ display: 'flex', gap: 8, marginBottom: 8, flexWrap: 'wrap' }}>
          <select
            className="input-padrao"
            value={novaTransicao.origem}
            onChange={e => setNovaTransicao({ ...novaTransicao, origem: e.target.value })}
            style={{ flex: 2, padding: 8, borderRadius: 6, border: '1px solid #ddd' }}
          >
            <option value="">Origem</option>
            {estados.map((estado, idx) => (
              <option key={idx} value={estado.nome}>{estado.nome}</option>
            ))}
          </select>
          <select
            className="input-padrao"
            value={novaTransicao.destino}
            onChange={e => setNovaTransicao({ ...novaTransicao, destino: e.target.value })}
            style={{ flex: 2, padding: 8, borderRadius: 6, border: '1px solid #ddd' }}
          >
            <option value="">Destino</option>
            {estados.map((estado, idx) => (
              <option key={idx} value={estado.nome}>{estado.nome}</option>
            ))}
          </select>
          <button
            type="button"
            className="novo-modelo-btn"
            onClick={adicionarTransicao}
            style={{ padding: '8px 12px', borderRadius: 6 }}
          >
            Adicionar Transição
          </button>
        </div>
        {mensagem && (
          <div style={{
            background: mensagem.includes('sucesso') ? '#4CAF50' : '#F44336',
            color: '#fff',
            padding: 12,
            borderRadius: 6,
            marginBottom: 8,
            textAlign: 'center',
            fontWeight: 500
          }}>{mensagem}</div>
        )}
        <button type="submit" className="novo-modelo-btn" style={{ alignSelf: 'flex-end', padding: '10px 24px', borderRadius: 6 }} disabled={loading}>
          {loading ? 'A criar...' : 'Criar Workflow'}
        </button>
      </form>
    </Modal>
  );
};

export default WorkflowModeloModal;
