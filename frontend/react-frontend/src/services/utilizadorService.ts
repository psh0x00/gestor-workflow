import axios from 'axios';

const API_URL = 'http://localhost:5148/api/utilizadores';

export async function pesquisarUtilizadores(query: string, token?: string) {
  return axios.get(`${API_URL}?q=${encodeURIComponent(query)}`, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  }).then(res => res.data);
}

export async function listarTodosUtilizadores(token?: string) {
  return axios.get(API_URL, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  }).then(res => res.data);
}
