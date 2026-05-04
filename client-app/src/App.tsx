import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Navbar from './components/Navbar'
import Dashboard from './pages/Dashboard'
import DevPage from './pages/DevPage'
import AboutPage from './pages/AboutPage'
import './App.css'

function App() {
  return (
    <BrowserRouter>
      <Navbar />
      <div className="app-container">
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/dev" element={<DevPage />} />
          <Route path="/about" element={<AboutPage />} />
        </Routes>
      </div>
    </BrowserRouter>
  )
}

export default App
