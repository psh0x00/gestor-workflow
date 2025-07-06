import axios from 'axios';

const API_URL = 'http://localhost:5148/api/workflow-modelos'; 

export async function criarWorkflowModelo(data: any, token?: string) {
  return axios.post(API_URL, data, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}

export async function listarWorkflowModelos(token?: string) {
  return axios.get(API_URL, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}

export async function obterWorkflowModeloPorId(id: number, token?: string) {
  return axios.get(`${API_URL}/${id}`, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}
