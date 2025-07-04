import React, { useState, useEffect } from 'react';
import WorkflowModeloModal from '../components/WorkflowModeloModal';
import './Home.css';
import { listarWorkflowModelos } from '../services/workflowModeloService';
import { useAuth } from '../context/AuthContext';

const Home: React.FC = () => {
  const { logout, user } = useAuth();
  const [activeTab, setActiveTab] = useState<'todos' | 'meus'>('todos');
  const [search, setSearch] = useState('');
  const [filtros, setFiltros] = useState({ ativo: 'todos' });
  const [modelos, setModelos] = useState<any[]>([]);
  const [loadingModelos, setLoadingModelos] = useState(true);
  const [erroModelos, setErroModelos] = useState<string | null>(null);
  const [workflowModalOpen, setWorkflowModalOpen] = useState(false);

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
        </div>
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
        <WorkflowModeloModal
          isOpen={workflowModalOpen}
          onClose={() => setWorkflowModalOpen(false)}
          onSubmit={handleNovoModelo}
        />
        <div className="modelos-lista">
          {loadingModelos ? (
            <p>A carregar modelos...</p>
          ) : erroModelos ? (
            <p style={{ color: 'red' }}>{erroModelos}</p>
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
                  onClick={() => alert(`Modelo selecionado: ${m.nome}`)}
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
