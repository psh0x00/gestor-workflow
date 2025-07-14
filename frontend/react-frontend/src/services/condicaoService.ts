import axios from 'axios';

const API_URL = 'http://localhost:5148/api/condicoes'; // Ajuste conforme seu backend

export async function listarCondicoes(token?: string) {
  return axios.get(API_URL, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}

export async function criarCondicao(nome: string, tipo: 'pre' | 'pos', token?: string) {
  return axios.post(API_URL, { nome, tipo }, {
    headers: token ? { Authorization: `Bearer ${token}` } : undefined
  });
}
