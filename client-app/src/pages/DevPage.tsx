import React from 'react';
import { ExternalLink, Terminal } from 'lucide-react';

const DevPage = () => {
  return (
    <div style={{ maxWidth: '800px', margin: '0 auto', textAlign: 'center', padding: '2rem' }}>
      <div className="icon-box" style={{ width: 'fit-content', margin: '0 auto 1.5rem auto', padding: '1.5rem' }}>
        <Terminal size={48} />
      </div>
      <h1 className="main-title">Разработчикам</h1>
      <p className="property-description" style={{ fontSize: '1.125rem', marginBottom: '2.5rem', maxWidth: '600px', margin: '0 auto 2.5rem auto' }}>
        Наше API построено на ASP.NET Core Web API с использованием принципов SOLID и Clean Architecture. 
        Вы можете протестировать все эндпоинты через интерактивную документацию Swagger.
      </p>
      
      <a 
        href="http://localhost:5120/swagger" 
        target="_blank" 
        rel="noopener noreferrer"
        className="btn-sell"
        style={{ 
          display: 'inline-flex', 
          alignItems: 'center', 
          justifyContent: 'center', 
          gap: '0.75rem', 
          padding: '1.25rem 2.5rem', 
          fontSize: '1.125rem',
          textDecoration: 'none',
          width: 'auto'
        }}
      >
        Открыть Swagger UI
        <ExternalLink size={20} />
      </a>
    </div>
  );
};

export default DevPage;
