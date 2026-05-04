# 🏠 RealtyCRM — Информационная система риэлторского агентства

[![.NET](https://img.shields.io/badge/.NET-10.0-512bd4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19.0-61dafb?style=flat-square&logo=react)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.0-3178c6?style=flat-square&logo=typescript)](https://www.typescriptlang.org/)
[![Vite](https://img.shields.io/badge/Vite-6.0-646cff?style=flat-square&logo=vite)](https://vitejs.dev/)

Современная многостраничная информационная система для управления объектами недвижимости и оформления сделок купли-продажи. Проект разработан в рамках курсовой работы по дисциплине "Объектно-ориентированные анализ и дизайн".

---

## ✨ Основные возможности

- **📊 Панель управления (Dashboard)**: Интерактивный список объектов недвижимости с фильтрацией по статусу.
- **➕ Добавление объектов**: Удобная форма для внесения новых предложений в базу (адрес, цена, площадь, тип, описание).
- **🤝 Оформление сделок**: Возможность в один клик перевести объект в статус "Продано" с созданием записи о сделке.
- **📑 Интерактивное API (Swagger)**: Полная документация всех эндпоинтов для разработчиков.
- **📱 Адаптивный дизайн**: Современный интерфейс, корректно отображающийся на всех устройствах.

---

## 🛠 Технологический стек

### Backend (ASP.NET Core Web API)
- **Framework:** .NET 10.0
- **Архитектура:** Clean Architecture / Service Pattern.
- **Документация:** Swagger (Swashbuckle) UI.
- **Хранение данных:** InMemory Repository (с возможностью легкой миграции на EF Core/PostgreSQL).

### Frontend (React.js)
- **Framework:** Vite + React + TypeScript.
- **Навигация:** React Router 7.
- **Иконки:** Lucide React.
- **Стилизация:** Vanilla CSS с использованием современных Grid и Flexbox.
- **HTTP Клиент:** Axios с настроенным Proxy для API.

---

## 🚀 Быстрый запуск

### 1. Запуск Backend (API)
```bash
# Перейдите в корень проекта
cd realtyCRM
# Запустите проект API
dotnet run --project RealtyCRM.Api
```
API будет доступно по адресу: `http://localhost:5120`
Swagger UI: `http://localhost:5120/swagger` (или в корне `http://localhost:5120/` в зависимости от настроек)

### 2. Запуск Frontend (React)
```bash
# Откройте второе окно терминала и перейдите в папку клиента
cd client-app
# Установите зависимости (если еще не установлены)
npm install
# Запустите сервер разработки
npm run dev
```
Приложение будет доступно по адресу: `http://localhost:5173`

---

## 📂 Структура проекта

```text
realtyCRM/
├── RealtyCRM.Api/          # Исходный код бэкенда (C#)
│   ├── Controllers/        # API контроллеры
│   ├── Services/           # Бизнес-логика
│   ├── Models/             # Модели данных
│   └── Program.cs          # Конфигурация приложения
├── client-app/             # Исходный код фронтенда (React)
│   ├── src/
│   │   ├── components/     # UI компоненты (Navbar, Card и др.)
│   │   ├── pages/          # Страницы приложения
│   │   ├── types/          # TypeScript интерфейсы
│   │   └── App.tsx         # Главный компонент и роутинг
│   └── vite.config.ts      # Конфигурация Vite и Proxy
└── README.md               # Документация проекта
```

---

## 👨‍💻 Разработчик

- **Студент:** Пожидаев Илья Денисович
- **Группа:** 24-КБ-ПР3
- **ВУЗ:** КубГТУ
- **Версия:** 1.0.0 (Stable)

---
*Проект создан с использованием современных подходов к разработке ПО и принципов чистого кода.*
