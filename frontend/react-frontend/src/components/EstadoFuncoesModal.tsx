import React from 'react';
import Modal from './Modal';

interface EstadoFuncoesModalProps {
  isOpen: boolean;
  onClose: () => void;
  funcoes: string[];
  funcoesSelecionadas: string[];
  onChange: (funcoesSelecionadas: string[]) => void;
}

const EstadoFuncoesModal: React.FC<EstadoFuncoesModalProps> = ({ isOpen, onClose, funcoes, funcoesSelecionadas, onChange }) => {
  const handleToggle = (funcao: string) => {
    if (funcoesSelecionadas.includes(funcao)) {
      onChange(funcoesSelecionadas.filter(f => f !== funcao));
    } else {
      onChange([...funcoesSelecionadas, funcao]);
    }
  };

  return (
    <Modal isOpen={isOpen} onClose={onClose}>
      <h3>Selecionar Funções para o Estado</h3>
      <ul style={{ listStyle: 'none', padding: 0 }}>
        {funcoes.map(funcao => (
          <li key={funcao} style={{ marginBottom: 8 }}>
            <label style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
              <input
                type="checkbox"
                checked={funcoesSelecionadas.includes(funcao)}
                onChange={() => handleToggle(funcao)}
              />
              {funcao}
            </label>
          </li>
        ))}
      </ul>
      <button className="novo-modelo-btn" onClick={onClose} style={{ marginTop: 16 }}>Fechar</button>
    </Modal>
  );
};

export default EstadoFuncoesModal;
