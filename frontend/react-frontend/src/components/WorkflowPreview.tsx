import React from 'react';
import { Estado, Transicao, WorkflowModelo } from '../types/workflow';

interface WorkflowPreviewProps {
  modelo: WorkflowModelo;
  visualOnly?: boolean;
}

const CARD_WIDTH = 160;
const CARD_HEIGHT = 80;
const CARD_GAP = 60;

const WorkflowPreview: React.FC<WorkflowPreviewProps> = ({ modelo, visualOnly }) => {
  if (visualOnly) {
    // Layout: estados em linha, transições como curvas (bezier) tracejadas e animadas
    const estados = modelo.estados || [];
    const transicoes = modelo.transicoes || [];
    return (
      <div style={{
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)',
        minHeight: CARD_HEIGHT + 80,
        minWidth: estados.length * (CARD_WIDTH + CARD_GAP),
        display: 'block',
        width: 'fit-content',
        maxWidth: '100%'
      }}>
        <style>{`
          @keyframes dashmove {
            to {
              stroke-dashoffset: 0;
            }
          }
        `}</style>
        {/* Estados em linha */}
        {estados.map((estado, idx) => (
          <div key={idx} style={{
            position: 'absolute',
            left: idx * (CARD_WIDTH + CARD_GAP),
            top: 60,
            width: CARD_WIDTH,
            height: CARD_HEIGHT,
            background: '#fff',
            border: `2px solid ${estado.corHexadecimal || '#bbb'}`,
            borderRadius: 12,
            boxShadow: '0 2px 8px #0001',
            textAlign: 'center',
            zIndex: 2,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            justifyContent: 'center',
          }}>
            <div style={{ fontWeight: 600, color: estado.corHexadecimal || '#333', fontSize: 16 }}>{estado.nome}</div>
            {estado.tipo && (
              <div style={{ fontSize: 12, color: '#888', marginTop: 2 }}>
                {typeof estado.tipo === 'number' ? (estado.tipo === 1 ? 'Inicial' : estado.tipo === 3 ? 'Final' : 'Intermédio') : estado.tipo}
              </div>
            )}
          </div>
        ))}
        {/* Ligações SVG curvas (bezier) tracejadas e animadas */}
        <svg style={{ position: 'absolute', left: 0, top: 0, pointerEvents: 'none', zIndex: 1 }} width={estados.length * (CARD_WIDTH + CARD_GAP)} height={CARD_HEIGHT + 120}>
          {transicoes.map((t, idx) => {
            const origemIdx = estados.findIndex(e => e.nome === (t.nomeEstadoOrigem || t.origem));
            const destinoIdx = estados.findIndex(e => e.nome === (t.nomeEstadoDestino || t.destino));
            if (origemIdx === -1 || destinoIdx === -1) return null;
            const x1 = origemIdx * (CARD_WIDTH + CARD_GAP) + CARD_WIDTH;
            const y1 = 60 + CARD_HEIGHT / 2;
            const x2 = destinoIdx * (CARD_WIDTH + CARD_GAP);
            const y2 = 60 + CARD_HEIGHT / 2;
            const curve = 40 + Math.abs(destinoIdx - origemIdx) * 10;
            return (
              <g key={idx}>
                <path
                  d={`M${x1},${y1} C${x1 + curve},${y1} ${x2 - curve},${y2} ${x2},${y2}`}
                  stroke="#4f46e5"
                  strokeWidth={2}
                  fill="none"
                  strokeDasharray="8 6"
                  style={{
                    strokeDashoffset: 28,
                    animation: 'dashmove 1.2s linear infinite',
                  }}
                  markerEnd="url(#arrow)"
                />
                {t.nome && (
                  <text x={(x1 + x2) / 2} y={y1 - 20} fontSize={12} fill="#888" textAnchor="middle">{t.nome}</text>
                )}
              </g>
            );
          })}
          <defs>
            <marker id="arrow" markerWidth="10" markerHeight="10" refX="10" refY="5" orient="auto" markerUnits="strokeWidth">
              <path d="M0,0 L10,5 L0,10 Z" fill="#4f46e5" />
            </marker>
          </defs>
        </svg>
      </div>
    );
  }
  return (
    <div style={{ padding: 8 }}>
      <h3 style={{ marginBottom: 8 }}>{modelo.nome}</h3>
      <p style={{ color: '#555', marginBottom: 12 }}>{modelo.descricao}</p>
      <div style={{ marginBottom: 12 }}>
        <b>Estados:</b>
        <ul style={{ margin: 0, paddingLeft: 18 }}>
          {modelo.estados?.map((e, idx) => (
            <li key={idx} style={{ color: e.corHexadecimal || '#222' }}>
              <span style={{ fontWeight: 500 }}>{e.nome}</span>
              {e.tipo && (
                <span style={{ fontSize: 12, marginLeft: 8, color: '#888' }}>
                  ({typeof e.tipo === 'number' ? (e.tipo === 1 ? 'Inicial' : e.tipo === 3 ? 'Final' : 'Intermédio') : e.tipo})
                </span>
              )}
              {e.funcoes && e.funcoes.length > 0 && (
                <span style={{ fontSize: 12, marginLeft: 8, color: '#1976d2' }}>
                  Funções: {e.funcoes.join(', ')}
                </span>
              )}
            </li>
          ))}
        </ul>
      </div>
      <div>
        <b>Transições:</b>
        <ul style={{ margin: 0, paddingLeft: 18 }}>
          {modelo.transicoes?.map((t, idx) => (
            <li key={idx}>
              <span style={{ fontWeight: 500 }}>{t.nomeEstadoOrigem ?? t.origem ?? '-'}</span>
              <span style={{ color: '#888', margin: '0 6px' }}>→</span>
              <span style={{ fontWeight: 500 }}>{t.nomeEstadoDestino ?? t.destino ?? '-'}</span>
              {t.nome && <span style={{ fontSize: 12, marginLeft: 8, color: '#888' }}>({t.nome})</span>}
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
};

export default WorkflowPreview;