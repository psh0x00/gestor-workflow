import React, { useState, useImperativeHandle, forwardRef, useEffect } from 'react';
import { TransformWrapper, TransformComponent } from 'react-zoom-pan-pinch';
import WorkflowPreview from './WorkflowPreview';
import { WorkflowModelo } from '../types/workflow';

const desfazerButtonStyle: React.CSSProperties = {
  minWidth: 90,
  background: 'linear-gradient(90deg, #ff5f6d 0%, #2b86ff 100%)',
  color: '#fff',
  border: 'none',
  borderRadius: 16,
  fontWeight: 600,
  fontSize: 18,
  padding: '8px 24px',
  boxShadow: '0 2px 8px #0002',
  transition: 'filter 0.2s',
  outline: 'none',
  cursor: 'pointer',
  letterSpacing: 0.5,
};

interface WorkflowPreviewZoomProps {
  modelo: WorkflowModelo;
  onSalvar?: (estadosConcluidos: Array<number|string>) => void;
  onConcluir?: () => void;
}

const buttonStyle: React.CSSProperties = {
  background: '#fff',
  border: '1px solid #bbb',
  borderRadius: 6,
  padding: '4px 10px',
  fontWeight: 600,
  fontSize: 16,
  cursor: 'pointer',
  boxShadow: '0 1px 4px #0001',
};

const WorkflowPreviewZoomInner = ({ modelo, onSalvar, onConcluir }: WorkflowPreviewZoomProps, ref: React.Ref<any>) => {
  const [estadoSelecionado, setEstadoSelecionado] = useState<any | null>(null);
  type CondicaoTransicao = { nome: string; origem?: string; destino?: string };
  const [condicoesEstado, setCondicoesEstado] = useState<{ pre: CondicaoTransicao[]; pos: CondicaoTransicao[] }>({ pre: [], pos: [] });
  // Estados concluídos (por id ou nome)

  const [estadosConcluidos, setEstadosConcluidos] = useState<Array<number|string>>([]);

  // Sempre que o modelo mudar, inicializa os estados concluídos vindos do backend
  useEffect(() => {
    if (modelo && Array.isArray((modelo as any).estadosConcluidos)) {
      setEstadosConcluidos((modelo as any).estadosConcluidos);
    } else {
      setEstadosConcluidos([]);
    }
  }, [modelo]);

  // Busca nomes das condições associadas às transições de entrada/saída do estado
  const handleEstadoClick = (estado: any) => {
    if (!modelo || !modelo.transicoes) {
      setEstadoSelecionado(estado);
      setCondicoesEstado({ pre: [], pos: [] });
      return;
    }
    // Transições de saída (onde o estado é origem)
    const transicoesSaida = modelo.transicoes.filter(
      (t: any) => t.origem === estado.nome || t.nomeEstadoOrigem === estado.nome
    );
    // Transições de entrada (onde o estado é destino)
    const transicoesEntrada = modelo.transicoes.filter(
      (t: any) => t.destino === estado.nome || t.nomeEstadoDestino === estado.nome
    );
    const getPreNome = (t: any) => t.preCondicaoNome || t.nomePreCondicao || t.preCondicao?.nome || t.preCondicao || '';
    const getPosNome = (t: any) => t.posCondicaoNome || t.nomePosCondicao || t.posCondicao?.nome || t.posCondicao || '';

    // Para qualquer estado, buscar condições pelas transições
    setEstadoSelecionado(estado);
    setCondicoesEstado({
      pre: transicoesEntrada.map((t: any) => ({ nome: getPreNome(t), origem: t.origem || t.nomeEstadoOrigem || '' })).filter((item: any) => !!item.nome && typeof item.nome === 'string'),
      pos: transicoesSaida.map((t: any) => ({ nome: getPosNome(t), destino: t.destino || t.nomeEstadoDestino || '' })).filter((item: any) => !!item.nome && typeof item.nome === 'string')
    });
  };

  const closeModal = () => setEstadoSelecionado(null);

  // Funções para marcar/desmarcar como concluído
  const marcarConcluido = (estado: any) => {
    setEstadosConcluidos(prev => [
      ...prev,
      ...(estado.id !== undefined ? [estado.id] : []),
      ...(estado.nome !== undefined ? [estado.nome] : [])
    ]);
  };
  const desfazerConcluido = (estado: any) => {
    setEstadosConcluidos(prev => prev.filter(e => e !== estado.id && e !== estado.nome));
  };

  // Permite ao pai acessar os estados concluídos via ref
  useImperativeHandle(ref, () => ({
    getEstadosConcluidos: () => estadosConcluidos,
    salvar: () => onSalvar && onSalvar(estadosConcluidos)
  }), [onSalvar, estadosConcluidos]);

  // Verifica se todos os estados do modelo estão concluídos
  const todosConcluidos = modelo && Array.isArray(modelo.estados) && modelo.estados.length > 0 && modelo.estados.every(
    (e: any) => estadosConcluidos.includes(e.id ?? e.nome)
  );

  return (
    <div
      style={{
        width: '100%',
        maxWidth: 1100,
        minWidth: 340,
        minHeight: 300,
        margin: '0 auto',
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'flex-end', // alinhamento à direita
        justifyContent: 'center',
        background: 'transparent',
        boxShadow: 'none',
        borderRadius: 0,
        padding: 0,
        position: 'relative',
      }}
      data-estados-concluidos={JSON.stringify(estadosConcluidos)}
    >
      <TransformWrapper
        initialScale={1}
        minScale={0.5}
        maxScale={2.5}
        wheel={{ step: 0.1 }}
        doubleClick={{ disabled: true }}
        panning={{ velocityDisabled: true }}
      >
        {(zoomProps) => {
          const { zoomIn, zoomOut, resetTransform } = zoomProps;
          return (
            <div style={{ width: '95%', minHeight: 300, minWidth: 340, display: 'flex', alignItems: 'center', justifyContent: 'flex-end', position: 'relative', background: '#f8f9fa', borderRadius: 24, boxShadow: '0 2px 16px #0001', padding: 24, marginLeft: 'auto' }}>
              <div style={{ position: 'absolute', top: 24, right: 32, zIndex: 10, display: 'flex', gap: 8 }}>
                <button onClick={() => zoomIn()} style={buttonStyle}>+</button>
                <button onClick={() => zoomOut()} style={buttonStyle}>-</button>
                <button onClick={() => resetTransform()} style={buttonStyle}>Reset</button>
              </div>
              <TransformComponent>
                <div
                  style={{
                    width: '100%',
                    minHeight: 220,
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    overflow: 'hidden',
                  }}
                >
                  <WorkflowPreview modelo={modelo} visualOnly onEstadoClick={handleEstadoClick} estadosConcluidos={estadosConcluidos} />
                </div>
              </TransformComponent>
              {/* Modal de detalhes do estado */}
              {estadoSelecionado && (
                <div style={{
                  position: 'fixed',
                  top: 0,
                  left: 0,
                  width: '100vw',
                  height: '100vh',
                  background: 'rgba(0,0,0,0.35)',
                  zIndex: 1000,
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                }} onClick={closeModal}>
                  <div style={{
                    background: '#fff',
                    borderRadius: 16,
                    minWidth: 340,
                    maxWidth: 440,
                    width: '100%',
                    boxShadow: '0 4px 24px #0002',
                    position: 'relative',
                    cursor: 'auto',
                    display: 'flex',
                    flexDirection: 'column',
                    padding: 0,
                  }} onClick={e => e.stopPropagation()}>
                    {/* Header */}
                    <div style={{
                      padding: '28px 32px 0 32px',
                      borderTopLeftRadius: 16,
                      borderTopRightRadius: 16,
                      display: 'flex',
                      alignItems: 'flex-start',
                      justifyContent: 'space-between',
                      minHeight: 40,
                    }}>
                      <h2 style={{ color: estadoSelecionado.corHexadecimal || '#333', margin: 0, fontWeight: 700, fontSize: 28, flex: 1 }}>{estadoSelecionado.nome}</h2>
                      <button onClick={closeModal} style={{ background: 'none', border: 'none', fontSize: 26, cursor: 'pointer', color: '#888', marginLeft: 12, marginTop: 2, lineHeight: 1 }}>&times;</button>
                    </div>
                    {/* Body */}
                    <div style={{ padding: '8px 32px 0 32px', fontSize: 15, color: '#222', flex: 1 }}>
                      {estadoSelecionado.tipo && (
                        <div style={{ fontSize: 15, color: '#444', marginBottom: 6 }}>
                          <b>Tipo:</b> {typeof estadoSelecionado.tipo === 'number' ? (estadoSelecionado.tipo === 1 ? 'Inicial' : estadoSelecionado.tipo === 3 ? 'Final' : 'Intermédio') : estadoSelecionado.tipo}
                        </div>
                      )}
                      {estadoSelecionado.funcoes && estadoSelecionado.funcoes.length > 0 && (
                        <div style={{ fontSize: 15, color: '#444', marginBottom: 6 }}>
                          <b>Funções:</b> {estadoSelecionado.funcoes.join(', ')}
                        </div>
                      )}
                      {estadoSelecionado.descricao && (
                        <div style={{ fontSize: 15, color: '#444', marginBottom: 6 }}>
                          <b>Descrição:</b> {estadoSelecionado.descricao}
                        </div>
                      )}
                      {/* Pré/pós-condição: exibe apenas se não for estado final */}
                      {(() => {
                        const pre = estadoSelecionado.preCondicao || estadoSelecionado.preCondicaoNome || estadoSelecionado.nomePreCondicao;
                        const pos = estadoSelecionado.posCondicao || estadoSelecionado.posCondicaoNome || estadoSelecionado.nomePosCondicao;
                        const isFinal = typeof estadoSelecionado.tipo === 'number' ? estadoSelecionado.tipo === 3 : (estadoSelecionado.tipo === 'Final');
                        if (isFinal) return null;
                        return <>
                          {pre && (
                            <div style={{ fontSize: 15, color: '#444', marginBottom: 6, display: 'flex', alignItems: 'center' }}>
                              <b>Pré-condição do estado:</b>&nbsp;{pre}
                            </div>
                          )}
                          {pos && (
                            <div style={{ fontSize: 15, color: '#444', marginBottom: 6, display: 'flex', alignItems: 'center' }}>
                              <b>Pós-condição do estado:</b>&nbsp;{pos}
                            </div>
                          )}
                        </>;
                      })()}
                      {/* Exibe condições apenas se não for estado final */}
                      {(() => {
                        const isFinal = typeof estadoSelecionado.tipo === 'number' ? estadoSelecionado.tipo === 3 : (estadoSelecionado.tipo === 'Final');
                        if (isFinal) return null;
                        return <>
                          {condicoesEstado.pre && condicoesEstado.pre.length > 0 && (
                            <div style={{ fontSize: 15, color: '#444', marginBottom: 6, display: 'flex', alignItems: 'center' }}>
                              <b>Pré-condições:</b>&nbsp;
                              {condicoesEstado.pre.map((item: any, idx: number) => (
                                <span key={idx} style={{ marginRight: 8 }}>{item.nome}{idx < condicoesEstado.pre.length - 1 ? ',' : ''}</span>
                              ))}
                            </div>
                          )}
                          {condicoesEstado.pos && condicoesEstado.pos.length > 0 && (
                            <div style={{ fontSize: 15, color: '#444', marginBottom: 6, display: 'flex', alignItems: 'center' }}>
                              <b>Pós-condições:</b>&nbsp;
                              {condicoesEstado.pos.map((item: any, idx: number) => (
                                <span key={idx} style={{ marginRight: 8 }}>{item.nome}{idx < condicoesEstado.pos.length - 1 ? ',' : ''}</span>
                              ))}
                            </div>
                          )}
                        </>;
                      })()}
                    </div>
                    {/* Footer */}
                    <div style={{
                      padding: '24px 32px 24px 32px',
                      borderBottomLeftRadius: 16,
                      borderBottomRightRadius: 16,
                      display: 'flex',
                      justifyContent: 'flex-end',
                      gap: 16,
                      background: 'transparent',
                      minHeight: 40,
                    }}>
                      {(() => {
                        if (!modelo || !estadoSelecionado) return null;
                        const estados = modelo.estados || [];
                        const idx = estados.findIndex(e => (((e as any).id ?? e.nome) === (estadoSelecionado.id ?? estadoSelecionado.nome)));
                        if (idx === 0) {
                          return (
                            <>
                              {estadosConcluidos.includes(estadoSelecionado.id ?? estadoSelecionado.nome) ? (
                              <button
                                onClick={() => desfazerConcluido(estadoSelecionado)}
                                style={desfazerButtonStyle}
                                onMouseOver={e => (e.currentTarget.style.filter = 'brightness(0.97)')}
                                onMouseOut={e => (e.currentTarget.style.filter = 'none')}
                              >
                                Desfazer
                              </button>
                              ) : (
                                <button
                                  onClick={() => marcarConcluido(estadoSelecionado)}
                                  style={{
                                    minWidth: 90,
                                    background: 'linear-gradient(90deg, #3b82f6 0%, #06d6df 100%)',
                                    color: '#fff',
                                    border: 'none',
                                    borderRadius: 16,
                                    fontWeight: 600,
                                    fontSize: 18,
                                    padding: '8px 24px',
                                    boxShadow: '0 2px 8px #0002',
                                    transition: 'filter 0.2s',
                                    outline: 'none',
                                    cursor: 'pointer',
                                  }}
                                  onMouseOver={e => (e.currentTarget.style.filter = 'brightness(0.97)')}
                                  onMouseOut={e => (e.currentTarget.style.filter = 'none')}
                                >
                                  Feito
                                </button>
                              )}
                            </>
                          );
                        }
                        // Para outros estados, só mostrar se o anterior estiver concluído
                        const anterior = estados[idx - 1];
                        if (!anterior) return null;
                        const anteriorConcluido = estadosConcluidos.includes((anterior as any).id ?? anterior.nome);
                        if (!anteriorConcluido) return null;
                        return (
                          <>
                            {estadosConcluidos.includes(estadoSelecionado.id ?? estadoSelecionado.nome) ? (
                              <button
                                onClick={() => desfazerConcluido(estadoSelecionado)}
                                style={desfazerButtonStyle}
                                onMouseOver={e => (e.currentTarget.style.filter = 'brightness(0.97)')}
                                onMouseOut={e => (e.currentTarget.style.filter = 'none')}
                              >
                                Desfazer
                              </button>
                            ) : (
                              <button
                                onClick={() => marcarConcluido(estadoSelecionado)}
                                style={{
                                  minWidth: 90,
                                  background: 'linear-gradient(90deg, #3b82f6 0%, #06d6df 100%)',
                                  color: '#fff',
                                  border: 'none',
                                  borderRadius: 16,
                                  fontWeight: 600,
                                  fontSize: 18,
                                  padding: '8px 24px',
                                  boxShadow: '0 2px 8px #0002',
                                  transition: 'filter 0.2s',
                                  outline: 'none',
                                  cursor: 'pointer',
                                }}
                                onMouseOver={e => (e.currentTarget.style.filter = 'brightness(0.97)')}
                                onMouseOut={e => (e.currentTarget.style.filter = 'none')}
                              >
                                Feito
                              </button>
                            )}
                          </>
                        );
                      })()}
                    </div>
                  </div>
                </div>
              )}
            </div>
          );
        }}
      </TransformWrapper>
      {/* Botão Concluir ao centro, apenas se todos concluídos */}
      {todosConcluidos && (
        <div style={{ display: 'flex', gap: 16, alignSelf: 'center', marginTop: 24 }}>
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
            onClick={typeof onConcluir === 'function' ? onConcluir : undefined}
          >
            Concluir
          </button>
        </div>
      )}
    </div>
  );
};
const WorkflowPreviewZoom = React.memo(forwardRef(WorkflowPreviewZoomInner));
export default WorkflowPreviewZoom;