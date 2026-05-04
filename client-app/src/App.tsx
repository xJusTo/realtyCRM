import { useState } from 'react'
import PropertyList from './components/PropertyList'
import AddPropertyForm from './components/AddPropertyForm'
import './App.css'

function App() {
  const [refreshKey, setRefreshKey] = useState(0);

  const handlePropertyAdded = () => {
    setRefreshKey(prev => prev + 1);
  };

  return (
    <div className="app-container">
      <h1 className="main-title">Панель управления RealtyCRM</h1>
      <AddPropertyForm onPropertyAdded={handlePropertyAdded} />
      <PropertyList refreshTrigger={refreshKey} />
    </div>
  )
}

export default App
