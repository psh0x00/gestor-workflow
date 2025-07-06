import React, { useState } from 'react';
import { WorkflowModelo } from '../types/workflow';
import Modal from './Modal';
import WorkflowPreview from './WorkflowPreview';
import './Modal.css';

interface WorkflowModeloViewModalProps {
  isOpen: boolean;
  onClose: () => void;
  modelo: WorkflowModelo | null;
}

const WorkflowModeloViewModal: React.FC<WorkflowModeloViewModalProps> = ({ isOpen, onClose, modelo }) => {
  const [tab, setTab] = useState<'preview' | 'details'>('preview');
  if (!modelo) return null;
  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <div style={{ minWidth: 340, maxWidth: 600, padding: 0 }}>
        <div style={{ display: 'flex', flexDirection: 'column', gap: 0 }}>
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '0 0 12px 0' }}>
            <h2 style={{ margin: 0, fontSize: 22, fontWeight: 600 }}>{modelo.nome}</h2>
            <div style={{ display: 'flex', gap: 8 }}>
              <button
                className={tab === 'preview' ? 'active' : ''}
                style={{
                  background: 'none', border: 'none', fontSize: 16, padding: '8px 18px', borderBottom: tab === 'preview' ? '2px solid #4f46e5' : '2px solid transparent', color: tab === 'preview' ? '#4f46e5' : '#222', fontWeight: tab === 'preview' ? 600 : 500, cursor: 'pointer', transition: 'border 0.2s, color 0.2s'
                }}
                onClick={() => setTab('preview')}
              >Preview</button>
              <button
                className={tab === 'details' ? 'active' : ''}
                style={{
                  background: 'none', border: 'none', fontSize: 16, padding: '8px 18px', borderBottom: tab === 'details' ? '2px solid #4f46e5' : '2px solid transparent', color: tab === 'details' ? '#4f46e5' : '#222', fontWeight: tab === 'details' ? 600 : 500, cursor: 'pointer', transition: 'border 0.2s, color 0.2s'
                }}
                onClick={() => setTab('details')}
              >Details</button>
            </div>
          </div>
          <div className="modal-body" style={{ minHeight: 120, padding: 0 }}>
            {tab === 'preview' && (
              <div style={{ padding: 8 }}>
                <WorkflowPreview modelo={{ ...modelo, nome: '' }} visualOnly />
              </div>
            )}
            {tab === 'details' && (
              <>
                <h3 style={{ marginLeft: 8, marginBottom: -10, marginTop: 50 }}>Descrição</h3>
                <WorkflowPreview modelo={{ ...modelo, nome: '' }} />
              </>
            )}
          </div>
        </div>
      </div>
    </Modal>
  );
};

export default WorkflowModeloViewModal;