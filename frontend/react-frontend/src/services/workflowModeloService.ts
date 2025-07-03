import axios from 'axios';

const API_URL = 'http://localhost:5148/api/workflow-modelos'; 

export async function criarWorkflowModelo(data: any, token?: string) {
  return axios.post(API_URL, data, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}
