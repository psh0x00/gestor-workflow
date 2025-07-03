import React, { useState } from 'react';
import Modal from '../components/Modal';
import WorkflowModeloModal from '../components/WorkflowModeloModal';
import './Home.css';

const Home: React.FC = () => {
  const [activeTab, setActiveTab] = useState<'todos' | 'meus'>('todos');
  const [search, setSearch] = useState('');
  const [filtros, setFiltros] = useState({ ativo: 'todos' });
  const [modalOpen, setModalOpen] = useState(false);

  // Mock data para exemplo
  const modelos = [
    { id: 1, nome: 'Aprovação de Documentos', ativo: true, criadoPorMim: true },
    { id: 2, nome: 'Processo de Compra', ativo: false, criadoPorMim: false },
    { id: 3, nome: 'Onboarding de Colaborador', ativo: true, criadoPorMim: true },
  ];

  const [workflowModalOpen, setWorkflowModalOpen] = useState(false);

  const handleNovoModelo = (data: any) => {
    // Aqui podes fazer a chamada à API ou atualizar o estado local
    alert('Novo modelo criado: ' + JSON.stringify(data, null, 2));
    setWorkflowModalOpen(false);
  };

  const modelosFiltrados = modelos.filter(m => {
    if (activeTab === 'meus' && !m.criadoPorMim) return false;
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
          {modelosFiltrados.length === 0 ? (
            <p>Nenhum modelo encontrado.</p>
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
