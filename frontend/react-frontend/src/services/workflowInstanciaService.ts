import axios from 'axios';

const API_URL = 'http://localhost:5148/api/workflow-instancias';

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
