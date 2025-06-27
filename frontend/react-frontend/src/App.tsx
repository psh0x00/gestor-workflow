import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login';
import Register from './pages/Register';
import { useAuth } from './context/AuthContext';
import './App.css';

const Home: React.FC = () => {
  const { user } = useAuth();
  return (
    <div style={{ padding: 40 }}>
      <h1>Bem-vindo{user ? `, ${user}` : ''}!</h1>
      <p>Esta é a página principal protegida.</p>
    </div>
  );
};

const PrivateRoute = ({ children }: { children: React.ReactNode }): React.ReactElement | null => {
  const { user } = useAuth();
  return user ? <>{children}</> : <Navigate to="/login" />;
};

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/" element={<PrivateRoute><Home /></PrivateRoute>} />
      </Routes>
    </Router>
  );
}

export default App;
