import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import styles from './Register.module.css';

const Register: React.FC = () => {
  const [form, setForm] = useState({ nome: '', email: '', password: '', confirmPassword: '' });
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setMessage('');
    if (form.password !== form.confirmPassword) {
      setMessage('As passwords não coincidem.');
      return;
    }
    setLoading(true);
    try {
      const res = await fetch('/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ nome: form.nome, email: form.email, password: form.password, funcao: '' }),
      });
      if (res.ok) {
        navigate('/login');
      } else {
        const data = await res.text();
        setMessage(data);
      }
    } catch {
      setMessage('Erro ao registar.');
    }
    setLoading(false);
  };

  return (
    <div className={styles.container}>
      <form className={styles.form} onSubmit={handleSubmit}>
        <h2>Criar Conta</h2>
        <input name="nome" placeholder="Nome" value={form.nome} onChange={handleChange} required />
        <input name="email" type="email" placeholder="Email" value={form.email} onChange={handleChange} required />
        <input name="password" type="password" placeholder="Password" value={form.password} onChange={handleChange} required />
        <input name="confirmPassword" type="password" placeholder="Confirmar Password" value={form.confirmPassword} onChange={handleChange} required />
        <button type="submit" disabled={loading}>{loading ? 'A criar...' : 'Registar'}</button>
        {message && <div className={styles.message}>{message}</div>}
        <div className={styles.linkContainer}>
          <span>Já tem conta?</span>
          <a href="/login" className={styles.link}>Entrar</a>
        </div>
      </form>
    </div>
  );
};

export default Register;
