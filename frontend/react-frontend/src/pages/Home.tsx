import React, { useState, useEffect } from 'react';
import WorkflowModeloModal from '../components/WorkflowModeloModal';
import WorkflowModeloViewModal from '../components/WorkflowModeloViewModal';
import './Home.css';
import { listarWorkflowModelos, obterWorkflowModeloPorId } from '../services/workflowModeloService';
import { listarPendentes, listarInstanciados, confirmarParticipacao } from '../services/workflowInstanciaService';
import { useAuth } from '../context/AuthContext';

const Home: React.FC = () => {
  const { logout, user } = useAuth();
  const [activeTab, setActiveTab] = useState<'todos' | 'meus' | 'pendentes' | 'instanciados'>('todos');
  const [search, setSearch] = useState('');
  const [filtros, setFiltros] = useState({ ativo: 'todos', statusInstanciado: 'todos' });
  const [modelos, setModelos] = useState<any[]>([]);
  const [pendentes, setPendentes] = useState<any[]>([]);
  const [instanciados, setInstanciados] = useState<any[]>([]);
  const [loadingModelos, setLoadingModelos] = useState(true);
  const [loadingPendentes, setLoadingPendentes] = useState(false);
  const [loadingInstanciados, setLoadingInstanciados] = useState(false);
  const [erroModelos, setErroModelos] = useState<string | null>(null);
  const [erroPendentes, setErroPendentes] = useState<string | null>(null);
  const [erroInstanciados, setErroInstanciados] = useState<string | null>(null);
  const [workflowModalOpen, setWorkflowModalOpen] = useState(false);
  const [viewModalOpen, setViewModalOpen] = useState(false);
  const [modeloSelecionado, setModeloSelecionado] = useState<any | null>(null);

  useEffect(() => {
    async function fetchModelos() {
      setLoadingModelos(true);
      setErroModelos(null);
      try {
        const token = localStorage.getItem('token');
        const res = await listarWorkflowModelos(token ?? undefined);
        setModelos(res.data);
      } catch (err: any) {
        setErroModelos('Erro ao carregar modelos.');
      } finally {
        setLoadingModelos(false);
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
      listarInstanciados(token ?? undefined)
        .then(res => setInstanciados(res.data))
        .catch(() => setErroInstanciados('Erro ao carregar instanciados.'))
        .finally(() => setLoadingInstanciados(false));
    }
  }, [activeTab]);

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
                  if (filtros.statusInstanciado === 'aberto') return i.status === 1 || i.status === 'Aberto';
                  if (filtros.statusInstanciado === 'terminado') return i.status === 2 || i.status === 'Terminado';
                  return true;
                }).length === 0 ? (
                <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', minHeight: '220px', width: '100%', textAlign: 'center', color: '#888' }}>
                  <span style={{ fontSize: 48, marginBottom: 12, opacity: 0.6 }}>📦</span>
                  <p style={{ fontSize: 20, fontWeight: 500, margin: 0 }}>Nenhum workflow instanciado encontrado.</p>
                </div>
              ) : (
                <div className="modelos-cards">
                  {instanciados.filter(i => {
                    if (filtros.statusInstanciado === 'aberto') return i.status === 1 || i.status === 'Aberto';
                    if (filtros.statusInstanciado === 'terminado') return i.status === 2 || i.status === 'Terminado';
                    return true;
                  }).map(i => (
                    <div
                      key={i.id}
                      className="modelo-card ativo"
                      style={{ cursor: 'default' }}
                    >
                      <div className="modelo-card-header">
                        <span className="modelo-nome">{i.nomeWorkflowModelo || 'Workflow'}</span>
                        <span className="status ativo">Instanciado</span>
                      </div>
                      <div style={{ marginTop: 8, fontSize: 15 }}>
                        Estado: <b>{i.nomeEstadoAtual || '-'}</b><br />
                        Iniciado por: <b>{i.nomeIniciador || '-'}</b><br />
                        Data de início: <b>{i.dataInicio ? new Date(i.dataInicio).toLocaleString() : '-'}</b>
                      </div>
                    </div>
                  ))}
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
