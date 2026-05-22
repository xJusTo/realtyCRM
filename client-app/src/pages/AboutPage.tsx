import { Globe, Send, Mail, User } from 'lucide-react';

const AboutPage = () => {
  return (
    <div style={{ maxWidth: '900px', margin: '0 auto', padding: '2rem' }}>
      <div className="about-card">
        <div style={{ marginBottom: '2rem' }}>
          <img src="/logo.svg" alt="RealtyCRM Logo" style={{ height: '64px', margin: '0 auto 1rem auto', display: 'block' }} />
          <p style={{ fontWeight: 600, color: 'var(--primary)', marginTop: '0.5rem' }}>
            Система управления недвижимостью
          </p>
          <span style={{ 
            display: 'inline-block', 
            marginTop: '1rem', 
            padding: '0.25rem 0.75rem', 
            backgroundColor: '#f1f5f9', 
            borderRadius: '20px', 
            fontSize: '0.875rem',
            color: 'var(--text-muted)' 
          }}>
            Версия: 1.0.0 (Stable)
          </span>
        </div>

        <div className="developer-info">
          <h2 className="form-title" style={{ justifyContent: 'center', marginBottom: '1rem' }}>
            <User size={24} color="var(--primary)" />
            О разработчике
          </h2>
          <p>Разработал: <strong>Пожидаев Илья Денисович</strong></p>
          <p>Группа: <strong>24-КБ-ПР3</strong></p>
          <p>Направление: <strong>Программная инженерия</strong></p>
          <p style={{ marginTop: '1rem', fontStyle: 'italic', color: 'var(--text-muted)' }}>
            Краснодарский край, г. Краснодар, 2026г.
          </p>
        </div>

        <h2 className="form-title" style={{ justifyContent: 'center', marginTop: '3rem', marginBottom: '1rem' }}>
          Связаться с автором
        </h2>
        
        <div className="contact-grid">
          <a href="https://github.com/xJusTo" target="_blank" rel="noreferrer" className="contact-item">
            <Globe size={32} color="#111827" />
            <span className="contact-name">GitHub</span>
            <span className="contact-value">@xJusTo</span>
          </a>

          <a href="https://t.me/xJusT0" target="_blank" rel="noreferrer" className="contact-item">
            <Send size={32} color="#0088cc" />
            <span className="contact-name">Telegram</span>
            <span className="contact-value">@xJusT0</span>
          </a>

          <a href="mailto:iliapozhidaev@gmail.com" className="contact-item">
            <Mail size={32} color="#ea4335" />
            <span className="contact-name">Email</span>
            <span className="contact-value">iliapozhidaev@gmail.com</span>
          </a>
        </div>
      </div>
    </div>
  );
};

export default AboutPage;
