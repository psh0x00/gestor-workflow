import React, { useState, useEffect } from 'react';
import WorkflowModeloModal from '../components/WorkflowModeloModal';
import WorkflowModeloViewModal from '../components/WorkflowModeloViewModal';
import Modal from '../components/Modal';
import WorkflowPreviewZoom from '../components/WorkflowPreviewZoom';
import { atualizarEstadosConcluidos, obterWorkflowInstanciaPorId } from '../services/workflowInstanciaService';
import { useRef } from 'react';
import './Home.css';
import { listarWorkflowModelos, obterWorkflowModeloPorId } from '../services/workflowModeloService';
import { listarPendentes, listarInstanciados, confirmarParticipacao } from '../services/workflowInstanciaService';
import { useAuth } from '../context/AuthContext';

const Home: React.FC = () => {
  const [instanciaModalOpen, setInstanciaModalOpen] = useState(false);
  const [modeloPreview, setModeloPreview] = useState<any | null>(null);
  const [loadingModeloPreview, setLoadingModeloPreview] = useState(false);
  const workflowZoomRef = useRef<any>(null);
  const { logout, user } = useAuth();
  const [activeTab, setActiveTab] = useState<'todos' | 'meus' | 'pendentes' | 'instanciados'>('todos');
  const [search, setSearch] = useState('');
  const [filtros, setFiltros] = useState({ ativo: 'todos', statusInstanciado: 'todos' });
  const [modelos, setModelos] = useState<any[]>([]);
  const [pendentes, setPendentes] = useState<any[]>([]);
  const [instanciados, setInstanciados] = useState<any[]>([]);
  // const [loadingModelos, setLoadingModelos] = useState(true);
  const [loadingPendentes, setLoadingPendentes] = useState(false);
  const [loadingInstanciados, setLoadingInstanciados] = useState(false);
  // const [erroModelos, setErroModelos] = useState<string | null>(null);
  const [erroPendentes, setErroPendentes] = useState<string | null>(null);
  const [erroInstanciados, setErroInstanciados] = useState<string | null>(null);
  const [workflowModalOpen, setWorkflowModalOpen] = useState(false);
  const [viewModalOpen, setViewModalOpen] = useState(false);
  const [modeloSelecionado, setModeloSelecionado] = useState<any | null>(null);

  useEffect(() => {
    async function fetchModelos() {
      try {
        const token = localStorage.getItem('token');
        const res = await listarWorkflowModelos(token ?? undefined);
        setModelos(res.data);
      } catch (err: any) {
        // Erro ao carregar modelos
      }
    }
    fetchModelos();
  }, []);

  useEffect(() => {
    if (activeTab === 'pendentes') {
      setLoadingPendentes(true);
      setErroPendentes(null);
      const token = localStorage.getItem('token');
      listarPendentes(token ?? undefined)
        .then(res => setPendentes(res.data))
        .catch(() => setErroPendentes('Erro ao carregar pendentes.'))
        .finally(() => setLoadingPendentes(false));
    } else if (activeTab === 'instanciados') {
      setLoadingInstanciados(true);
      setErroInstanciados(null);
      const token = localStorage.getItem('token');
      let statusId: number | undefined = undefined;
      if (filtros.statusInstanciado === 'aberto') statusId = 1;
      else if (filtros.statusInstanciado === 'terminado') statusId = 2;
      listarInstanciados(token ?? undefined, statusId)
        .then(res => setInstanciados(res.data))
        .catch(() => setErroInstanciados('Erro ao carregar instanciados.'))
        .finally(() => setLoadingInstanciados(false));
    }
  }, [activeTab, filtros.statusInstanciado]);

  const handleNovoModelo = (data: any) => {
    // Aqui podes fazer a chamada à API ou atualizar o estado local
    alert('Novo modelo criado: ' + JSON.stringify(data, null, 2));
    setWorkflowModalOpen(false);
  };

  // Obter o id do utilizador logado (assumindo que está no token JWT)
  function getUserIdFromToken() {
    const token = localStorage.getItem('token');
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload['nameid'] || payload['sub'] || payload['userId'] || payload['userid'] || null;
    } catch {
      return null;
    }
  }
  const userId = getUserIdFromToken();

  const modelosFiltrados = modelos.filter(m => {
    if (activeTab === 'meus' && userId && m.criadoPorId !== undefined && m.criadoPorId !== userId && m.criadoPorId !== Number(userId)) return false;
    if (filtros.ativo === 'ativos' && !m.ativo) return false;
    if (filtros.ativo === 'inativos' && m.ativo) return false;
    if (search && !m.nome.toLowerCase().includes(search.toLowerCase())) return false;
    return true;
  });

  // Novo: busca detalhes completos ao abrir modal de visualização
  const handleAbrirViewModal = async (modeloResumo: any) => {
    setModeloSelecionado(null); // Limpa antes de buscar
    setViewModalOpen(true);
    try {
      const token = localStorage.getItem('token');
      const res = await obterWorkflowModeloPorId(modeloResumo.id, token ?? undefined);
      setModeloSelecionado(res.data);
    } catch (err) {
      setModeloSelecionado({ ...modeloResumo, erro: 'Erro ao buscar detalhes.' });
    }
  };

  const handleConfirmar = async (id: number, aceitar: boolean) => {
    const token = localStorage.getItem('token') || undefined;
    try {
      await confirmarParticipacao(id, aceitar, token);
      setPendentes(pendentes.filter(p => p.id !== id));
    } catch {
      alert('Erro ao confirmar participação.');
    }
  };

  return (
    <div className="home-bg">
      <div className="home-container">
        <div className="home-header">
          <h1>Gestão de Workflows</h1>
          <div style={{ marginLeft: 'auto', display: 'flex', alignItems: 'center', gap: 16 }}>
            {user && (
              <>
                <span style={{ fontWeight: 500, color: '#333' }}>{user}</span>
                <button
                  onClick={logout}
                  style={{
                    background: 'linear-gradient(90deg, #fc466b 0%, #3f5efb 100%)',
                    color: '#fff',
                    border: 'none',
                    borderRadius: 6,
                    padding: '8px 18px',
                    fontWeight: 500,
                    cursor: 'pointer',
                    fontSize: 16
                  }}
                >
                  Logout
                </button>
              </>
            )}
          </div>
        </div>
        <div className="tabs">
          <button className={activeTab === 'todos' ? 'active' : ''} onClick={() => setActiveTab('todos')}>Todos os Modelos</button>
          <button className={activeTab === 'meus' ? 'active' : ''} onClick={() => setActiveTab('meus')}>Meus Modelos</button>
          <button className={activeTab === 'instanciados' ? 'active' : ''} onClick={() => setActiveTab('instanciados')}>Instanciados</button>
          <button className={activeTab === 'pendentes' ? 'active' : ''} onClick={() => setActiveTab('pendentes')}>Pendentes</button>
        </div>
        {activeTab === 'pendentes' ? (
          <div className="filtros-bar">
            <input
              type="text"
              placeholder="Pesquisar pendentes..."
              value={search}
              onChange={e => setSearch(e.target.value)}
            />
          </div>
        ) : activeTab === 'instanciados' ? (
          <div className="filtros-bar">
            <input
              type="text"
              placeholder="Pesquisar instanciados..."
              value={search}
              onChange={e => setSearch(e.target.value)}
            />
            <select value={filtros.statusInstanciado || 'todos'} onChange={e => setFiltros({ ...filtros, statusInstanciado: e.target.value })}>
              <option value="todos">Todos</option>
              <option value="aberto">Aberto</option>
              <option value="terminado">Terminado</option>
            </select>
          </div>
        ) : (
          <div className="filtros-bar">
            <input
              type="text"
              placeholder="Pesquisar modelos..."
              value={search}
              onChange={e => setSearch(e.target.value)}
            />
            <select value={filtros.ativo} onChange={e => setFiltros({ ...filtros, ativo: e.target.value })}>
              <option value="todos">Todos</option>
              <option value="ativos">Ativos</option>
              <option value="inativos">Inativos</option>
            </select>
            <button className="novo-modelo-btn" onClick={() => setWorkflowModalOpen(true)}> + Novo </button>
          </div>
        )}
        <WorkflowModeloModal
          isOpen={workflowModalOpen}
          onClose={() => setWorkflowModalOpen(false)}
          onSubmit={handleNovoModelo}
        />
        <WorkflowModeloViewModal
          isOpen={viewModalOpen}
          onClose={() => setViewModalOpen(false)}
          modelo={modeloSelecionado}
        />
        <div className="modelos-lista">
          {activeTab === 'pendentes' ? (
            <div style={{ minHeight: 220, width: '100%', textAlign: 'center', color: '#888', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
              {loadingPendentes ? (
                <p>A carregar pendentes...</p>
              ) : erroPendentes ? (
                <p style={{ color: 'red' }}>{erroPendentes}</p>
              ) : pendentes.length === 0 ? (
                <>
                  <span style={{ fontSize: 48, marginBottom: 12, opacity: 0.6 }}>⏳</span>
                  <p style={{ fontSize: 20, fontWeight: 500, margin: 0 }}>Nenhuma confirmação pendente.</p>
                </>
              ) : (
                <ul style={{ width: '100%', maxWidth: 600, margin: '0 auto', padding: 0 }}>
                  {pendentes.map((p, i) => (
                    <li key={p.id || i} style={{ background: '#fafbff', borderRadius: 8, margin: '12px 0', padding: 18, boxShadow: '0 2px 8px #0001', textAlign: 'left', color: '#222', listStyle: 'none' }}>
                      <b>{p.nomeWorkflowModelo || 'Workflow'}</b><br />
                      Estado: <b>{p.nomeEstadoAtual || '-'}</b><br />
                      Iniciado por: <b>{p.nomeIniciador || '-'}</b>
                      <div style={{ marginTop: 12, display: 'flex', gap: 12 }}>
                        <button onClick={() => handleConfirmar(p.id, true)} style={{ background: '#22c55e', color: '#fff', border: 'none', borderRadius: 6, padding: '7px 18px', cursor: 'pointer' }}>Aceitar</button>
                        <button onClick={() => handleConfirmar(p.id, false)} style={{ background: '#ef4444', color: '#fff', border: 'none', borderRadius: 6, padding: '7px 18px', cursor: 'pointer' }}>Recusar</button>
                      </div>
                    </li>
                  ))}
                </ul>
              )}
            </div>
          ) : activeTab === 'instanciados' ? (
            <div style={{ minHeight: 220, width: '100%' }}>
              {loadingInstanciados ? (
                <p style={{ textAlign: 'center', color: '#888' }}>A carregar instanciados...</p>
              ) : erroInstanciados ? (
                <p style={{ color: 'red', textAlign: 'center' }}>{erroInstanciados}</p>
              ) : instanciados.filter(i => {
                  // Filtro por status_id (1=Ativo, 2=Concluido, 3=Suspenso, 4=Cancelado)
                  if (filtros.statusInstanciado === 'aberto') return i.status === 1;
                  if (filtros.statusInstanciado === 'terminado') return i.status === 2;
                  return true;
                }).length === 0 ? (
                <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', minHeight: '220px', width: '100%', textAlign: 'center', color: '#888' }}>
                  <span style={{ fontSize: 48, marginBottom: 12, opacity: 0.6 }}>📦</span>
                  <p style={{ fontSize: 20, fontWeight: 500, margin: 0 }}>Nenhum workflow instanciado encontrado.</p>
                </div>
              ) : (
                <div className="modelos-cards">
                  {instanciados.filter(i => {
                    if (filtros.statusInstanciado === 'aberto') return i.status === 1;
                    if (filtros.statusInstanciado === 'terminado') return i.status === 2;
                    return true;
                  }).map(i => (
                    <div
                      key={i.id}
                      className="modelo-card ativo"
                      style={{ cursor: i.status === 2 ? 'default' : 'pointer', opacity: i.status === 2 ? 0.7 : 1 }}
                      onClick={i.status === 2 ? undefined : async () => {
                        setInstanciaModalOpen(true);
                        setLoadingModeloPreview(true);
                        setModeloPreview(null);
                        try {
                          const token = localStorage.getItem('token');
                          // Busca modelo e instância em paralelo (igual ao botão Salvar)
                          const [instanciaRes, modeloRes] = await Promise.all([
                            obterWorkflowInstanciaPorId(i.id, token ?? undefined),
                            obterWorkflowModeloPorId(i.workflowModeloId, token ?? undefined)
                          ]);
                          setModeloPreview({ ...modeloRes.data, ...instanciaRes.data, instanciaId: i.id });
                        } catch {
                          setModeloPreview(null);
                        } finally {
                          setLoadingModeloPreview(false);
                        }
                      }}
                    >
                      <div className="modelo-card-header">
                        <span className="modelo-nome">{i.nomeWorkflowModelo || 'Workflow'}</span>
                        <span className="status ativo">Instanciado</span>
                      </div>
                      <div style={{ marginTop: 8, fontSize: 15 }}>
                        <b>Data de início:</b> {i.dataInicio ? new Date(i.dataInicio).toLocaleString() : '-'}<br />
                        <b>Data de fim:</b> {i.dataFim ? new Date(i.dataFim).toLocaleString() : '-'}
                      </div>
                    </div>
                  ))}
      <Modal isOpen={instanciaModalOpen} onClose={async () => {
        setInstanciaModalOpen(false);
        setModeloPreview(null);
        // Atualiza a lista de instanciados ao fechar o modal
        if (activeTab === 'instanciados') {
          setLoadingInstanciados(true);
          setErroInstanciados(null);
          const token = localStorage.getItem('token');
          let statusId: number | undefined = undefined;
          if (filtros.statusInstanciado === 'aberto') statusId = 1;
          else if (filtros.statusInstanciado === 'terminado') statusId = 2;
          try {
            const res = await listarInstanciados(token ?? undefined, statusId);
            setInstanciados(res.data);
          } catch {
            setErroInstanciados('Erro ao carregar instanciados.');
          } finally {
            setLoadingInstanciados(false);
          }
        }
      }}>
        <div style={{ minWidth: 340, maxWidth: 900, padding: 0 }}>
          {loadingModeloPreview ? (
            <div style={{ padding: 32, textAlign: 'center' }}>A carregar modelo...</div>
          ) : modeloPreview ? (
            <>
              <div style={{ display: 'flex', flexDirection: 'column', gap: 0 }}>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '0 0 12px 0' }}>
                  <h2 style={{ margin: 0, fontSize: 22, fontWeight: 600 }}>{modeloPreview.nome || 'Workflow'}</h2>
                </div>
                <div className="modal-body" style={{ padding: 0, minHeight: 400, position: 'relative', marginBottom: 0 }}>
                  <WorkflowPreviewZoom
                    ref={workflowZoomRef}
                    modelo={modeloPreview}
                    onSalvar={async (estadosConcluidos: Array<number|string>) => {
                      if (!modeloPreview || !modeloPreview.instanciaId) return;
                      const token = localStorage.getItem('token');
                      try {
                        // Salva os estados concluídos normalmente
                        await atualizarEstadosConcluidos(modeloPreview.instanciaId, estadosConcluidos, token ?? undefined);
                        // Verifica se todos os estados do modelo estão concluídos
                        const estadosModelo = Array.isArray(modeloPreview.estados) ? modeloPreview.estados : [];
                        const todosConcluidos = estadosModelo.length > 0 && estadosModelo.every((e: any) => estadosConcluidos.includes(e.id ?? e.nome));
                        if (todosConcluidos) {
                          // Chama o endpoint para terminar a instância
                          await fetch(`/api/workflow-instancias/${modeloPreview.instanciaId}/concluir`, {
                            method: 'POST',
                            headers: {
                              'Content-Type': 'application/json',
                              ...(token ? { 'Authorization': `Bearer ${token}` } : {})
                            }
                          });
                        }
                        // Buscar novamente a instância após salvar
                        const instanciaId = modeloPreview.instanciaId;
                        const [instanciaRes, modeloRes] = await Promise.all([
                          obterWorkflowInstanciaPorId(instanciaId, token ?? undefined),
                          obterWorkflowModeloPorId(modeloPreview.workflowModeloId || modeloPreview.id, token ?? undefined)
                        ]);
                        setModeloPreview({ ...modeloRes.data, ...instanciaRes.data, instanciaId });
                        setLoadingInstanciados(true);
                        setErroInstanciados(null);
                        try {
                          const res = await listarInstanciados(token ?? undefined);
                          setInstanciados(res.data);
                        } catch {
                          setErroInstanciados('Erro ao carregar instanciados.');
                        } finally {
                          setLoadingInstanciados(false);
                        }
                        setInstanciaModalOpen(false);
                      } catch (err) {
                        alert('Erro ao salvar estados concluídos.');
                      }
                    }}
                  />
                  <button
                    style={{
                      minWidth: 110,
                      background: 'linear-gradient(90deg, #06d6df 0%, #3b82f6 100%)',
                      color: '#fff',
                      border: 'none',
                      borderRadius: 16,
                      fontWeight: 600,
                      fontSize: 18,
                      padding: '10px 32px',
                      boxShadow: '0 2px 8px #0002',
                      transition: 'filter 0.2s',
                      outline: 'none',
                      cursor: 'pointer',
                    }}
                    onClick={async () => {
                      if (workflowZoomRef.current && modeloPreview && modeloPreview.instanciaId) {
                        const estadosConcluidos = workflowZoomRef.current.getEstadosConcluidos();
                        const token = localStorage.getItem('token');
                        try {
                          await atualizarEstadosConcluidos(modeloPreview.instanciaId, estadosConcluidos, token ?? undefined);
                          // Verifica se todos os estados do modelo estão concluídos
                          const estadosModelo = Array.isArray(modeloPreview.estados) ? modeloPreview.estados : [];
                          const todosConcluidos = estadosModelo.length > 0 && estadosModelo.every((e: any) => estadosConcluidos.includes(e.id ?? e.nome));
                          if (todosConcluidos) {
                            // Chama o endpoint para terminar a instância
                            await fetch(`/api/workflow-instancias/${modeloPreview.instanciaId}/concluir`, {
                              method: 'POST',
                              headers: {
                                'Content-Type': 'application/json',
                                ...(token ? { 'Authorization': `Bearer ${token}` } : {})
                              }
                            });
                          }
                          // Buscar novamente a instância após salvar
                          const instanciaId = modeloPreview.instanciaId;
                          const [instanciaRes, modeloRes] = await Promise.all([
                            obterWorkflowInstanciaPorId(instanciaId, token ?? undefined),
                            obterWorkflowModeloPorId(modeloPreview.workflowModeloId || modeloPreview.id, token ?? undefined)
                          ]);
                          // Atualiza o preview do modal
                          setModeloPreview({ ...modeloRes.data, ...instanciaRes.data, instanciaId });
                          // Busca novamente a lista de instanciados do backend para garantir atualização
                          setLoadingInstanciados(true);
                          setErroInstanciados(null);
                          try {
                            const res = await listarInstanciados(token ?? undefined);
                            setInstanciados(res.data);
                          } catch {
                            setErroInstanciados('Erro ao carregar instanciados.');
                          } finally {
                            setLoadingInstanciados(false);
                          }
                          setInstanciaModalOpen(false);
                        } catch (err) {
                          alert('Erro ao salvar estados concluídos.');
                        }
                      }
                    }}
                  >
                    Salvar
                  </button>
                </div>
              </div>
            </>
          ) : null}
        </div>
      </Modal>
                </div>
              )}
            </div>
          ) : modelosFiltrados.length === 0 ? (
            <div style={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              justifyContent: 'center',
              minHeight: '220px',
              width: '100%',
              textAlign: 'center',
              color: '#888'
            }}>
              <span style={{ fontSize: 48, marginBottom: 12, opacity: 0.6 }}>📄</span>
              <p style={{ fontSize: 20, fontWeight: 500, margin: 0 }}>Nenhum modelo encontrado.</p>
            </div>
          ) : (
            <div className="modelos-cards">
              {modelosFiltrados.map(m => (
                <div
                  key={m.id}
                  className={`modelo-card ${m.ativo ? 'ativo' : 'inativo'}`}
                  onClick={() => handleAbrirViewModal(m)}
                  style={{ cursor: 'pointer' }}
                >
                  <div className="modelo-card-header">
                    <span className="modelo-nome">{m.nome}</span>
                    {m.ativo ? <span className="status ativo">Ativo</span> : <span className="status inativo">Inativo</span>}
                  </div>
                  {/* Aqui podes adicionar mais detalhes do modelo, se quiseres */}
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Home;
