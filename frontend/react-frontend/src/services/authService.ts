// src/services/authService.ts
export async function register({ nome, funcao, email, password }: { nome: string; funcao: string; email: string; password: string }) {
  const res = await fetch('http://localhost:5148/api/auth/register', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ nome, funcao, email, password }),
  });
  if (!res.ok) throw new Error(await res.text());
  return res.text();
}

export async function login({ email, password }: { email: string; password: string }) {
  const res = await fetch('http://localhost:5148/api/auth/login', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  });
  if (!res.ok) throw new Error(await res.text());
  return res.json(); // Agora retorna { token: ... }
}
