import React, { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import styles from './Login.module.css';

const Login: React.FC = () => {
  const [form, setForm] = useState({ email: '', password: '' });
  const [message, setMessage] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setMessage('');
    try {
      await login(form.email, form.password);
      navigate('/'); // Redireciona para a página principal após login
    } catch (err: any) {
      setMessage(err.message || 'Erro ao autenticar.');
    }
    setLoading(false);
  };

  return (
    <div className={styles.container}>
      <form className={styles.form} onSubmit={handleSubmit}>
        <h2>Iniciar Sessão</h2>
        <input name="email" type="email" placeholder="Email" value={form.email} onChange={handleChange} required />
        <input name="password" type="password" placeholder="Password" value={form.password} onChange={handleChange} required />
        <button type="submit" disabled={loading}>{loading ? 'A entrar...' : 'Entrar'}</button>
        {message && <div className={styles.message}>{message}</div>}
        <div className={styles.linkContainer}>
          <span>Ainda não tem conta?</span>
          <a href="/register" className={styles.link}>Criar conta</a>
        </div>
      </form>
    </div>
  );
};

export default Login;
