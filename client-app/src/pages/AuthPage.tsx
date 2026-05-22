import React, { useState } from 'react';
import axios from 'axios';
import { Mail, Lock, User, Phone, Briefcase, FileText } from 'lucide-react';

interface Props {
  onLogin: () => void;
}

const AuthPage: React.FC<Props> = ({ onLogin }) => {
  const [isLogin, setIsLogin] = useState(true);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [fullName, setFullName] = useState('');
  const [phone, setPhone] = useState('');
  const [role, setRole] = useState<'Client' | 'Realtor'>('Client');
  const [preferences, setPreferences] = useState('');
  const [commissionRate, setCommissionRate] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      if (isLogin) {
        const response = await axios.post('/api/auth/login', { email, password });
        localStorage.setItem('user', JSON.stringify(response.data));
        onLogin();
      } else {
        const payload = {
          email,
          password,
          fullName,
          phone,
          role,
          preferences: role === 'Client' ? preferences : undefined,
          commissionRate: role === 'Realtor' ? Number(commissionRate) : undefined,
        };
        await axios.post('/api/auth/register', payload);
        alert('Регистрация прошла успешно! Теперь вы можете войти.');
        setIsLogin(true);
        setPassword('');
      }
    } catch (err: any) {
      setError(err.response?.data || 'Произошла ошибка при выполнении операции.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-wrapper">
      <div className="auth-card">
        <div className="auth-header">
          <img src="/logo.svg" alt="RealtyCRM Logo" className="auth-logo" />
          <p>Информационная система управления недвижимостью</p>
        </div>

        <div className="auth-tabs">
          <button 
            type="button" 
            className={`auth-tab-btn ${isLogin ? 'active' : ''}`}
            onClick={() => { setIsLogin(true); setError(null); }}
          >
            Войти
          </button>
          <button 
            type="button" 
            className={`auth-tab-btn ${!isLogin ? 'active' : ''}`}
            onClick={() => { setIsLogin(false); setError(null); }}
          >
            Регистрация
          </button>
        </div>

        <form onSubmit={handleSubmit} className="auth-form">
          {error && <div className="auth-error">{error}</div>}

          {!isLogin && (
            <>
              <div className="input-group">
                <label className="label">ФИО</label>
                <div className="input-with-icon">
                  <User size={18} className="input-icon" />
                  <input 
                    type="text" 
                    placeholder="Иванов Иван Иванович" 
                    value={fullName}
                    onChange={e => setFullName(e.target.value)}
                    required
                    className="input-field"
                  />
                </div>
              </div>

              <div className="input-group">
                <label className="label">Телефон</label>
                <div className="input-with-icon">
                  <Phone size={18} className="input-icon" />
                  <input 
                    type="tel" 
                    placeholder="+7 (999) 999-99-99" 
                    value={phone}
                    onChange={e => setPhone(e.target.value)}
                    required
                    className="input-field"
                  />
                </div>
              </div>

              <div className="input-group">
                <label className="label">Роль в системе</label>
                <div className="role-selector-grid">
                  <button
                    type="button"
                    className={`role-btn ${role === 'Client' ? 'active' : ''}`}
                    onClick={() => setRole('Client')}
                  >
                    <User size={16} />
                    Клиент
                  </button>
                  <button
                    type="button"
                    className={`role-btn ${role === 'Realtor' ? 'active' : ''}`}
                    onClick={() => setRole('Realtor')}
                  >
                    <Briefcase size={16} />
                    Риэлтор
                  </button>
                </div>
              </div>

              {role === 'Client' && (
                <div className="input-group">
                  <label className="label">Пожелания по недвижимости</label>
                  <div className="input-with-icon">
                    <FileText size={18} className="input-icon" />
                    <input 
                      type="text" 
                      placeholder="Ищет 2-к квартиру в центре" 
                      value={preferences}
                      onChange={e => setPreferences(e.target.value)}
                      className="input-field"
                    />
                  </div>
                </div>
              )}

              {role === 'Realtor' && (
                <div className="input-group">
                  <label className="label">Комиссионная ставка (%)</label>
                  <div className="input-with-icon">
                    <Briefcase size={18} className="input-icon" />
                    <input 
                      type="number" 
                      step="0.1" 
                      placeholder="2.5" 
                      value={commissionRate}
                      onChange={e => setCommissionRate(e.target.value)}
                      required
                      className="input-field"
                    />
                  </div>
                </div>
              )}
            </>
          )}

          <div className="input-group">
            <label className="label">Электронная почта</label>
            <div className="input-with-icon">
              <Mail size={18} className="input-icon" />
              <input 
                type="email" 
                placeholder="example@mail.com" 
                value={email}
                onChange={e => setEmail(e.target.value)}
                required
                className="input-field"
              />
            </div>
          </div>

          <div className="input-group">
            <label className="label">Пароль</label>
            <div className="input-with-icon">
              <Lock size={18} className="input-icon" />
              <input 
                type="password" 
                placeholder="••••••••" 
                value={password}
                onChange={e => setPassword(e.target.value)}
                required
                className="input-field"
              />
            </div>
          </div>

          <button type="submit" disabled={loading} className="btn-primary auth-submit-btn">
            {loading ? 'Загрузка...' : isLogin ? 'Войти' : 'Зарегистрироваться'}
          </button>
        </form>
      </div>
    </div>
  );
};

export default AuthPage;
