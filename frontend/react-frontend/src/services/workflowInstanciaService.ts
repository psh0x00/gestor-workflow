import axios from 'axios';

export async function obterWorkflowInstanciaPorId(instanciaId: number, token?: string) {
  return axios.get(`${API_URL}/${instanciaId}`, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}

const API_URL = 'http://localhost:5148/api/workflow-instancias';

export async function atualizarEstadosConcluidos(instanciaId: number, estadosConcluidos: Array<number|string>, token?: string) {
  console.log('instanciaId:', instanciaId);
  console.log('estadosConcluidos:', estadosConcluidos);
  console.log('token:', token);
  const estadosNumeros = estadosConcluidos.map(Number).filter((v) => !isNaN(v));
  const url = `${API_URL}/${instanciaId}/estados-concluidos`;
  console.log('URL chamada:', url);
  return axios.put(
    url,
    { estadosConcluidos: estadosNumeros },
    {
      headers: token ? { Authorization: `Bearer ${token}` } : undefined
    }
  );
}

export async function criarWorkflowInstancia(data: any, token?: string) {
  return axios.post(API_URL, data, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}

export async function listarPendentes(token?: string) {
  return axios.get(`${API_URL}/pendentes`, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}

export async function confirmarParticipacao(id: number, aceitar: boolean, token?: string) {
  return axios.post(`${API_URL}/${id}/confirmar`, { aceitar }, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}

export async function listarInstanciados(token?: string) {
  return axios.get(`${API_URL}/instanciados`, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}
