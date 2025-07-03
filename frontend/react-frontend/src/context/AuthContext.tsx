import React, { createContext, useContext, useState, ReactNode } from 'react';
import * as authService from '../services/authService';

interface AuthContextType {
  user: string | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [user, setUser] = useState<string | null>(null);

  const login = async (email: string, password: string) => {
    const result = await authService.login({ email, password });
    localStorage.setItem('token', result.token); // Salva o token JWT
    setUser(email); // Aqui pode guardar token ou info do utilizador
  };

  const logout = () => {
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within an AuthProvider');
  return context;
}
