import { useState } from 'react'
import PropertyList from '../components/PropertyList'
import AddPropertyForm from '../components/AddPropertyForm'

const Dashboard = () => {
  const [refreshKey, setRefreshKey] = useState(0);

  const handlePropertyAdded = () => {
    setRefreshKey(prev => prev + 1);
  };

  return (
    <>
      <h1 className="main-title">Панель управления RealtyCRM</h1>
      <AddPropertyForm onPropertyAdded={handlePropertyAdded} />
      <PropertyList refreshTrigger={refreshKey} />
    </>
  );
};

export default Dashboard;
