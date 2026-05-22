import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { PropertyStatus, PropertyType } from '../types/models';
import type { Property } from '../types/models';
import { Home, Building2, Building, Info, MapPin, Tag } from 'lucide-react';

interface Props {
    refreshTrigger?: number;
}

const PropertyList: React.FC<Props> = ({ refreshTrigger }) => {
    const [properties, setProperties] = useState<Property[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchProperties = async () => {
        try {
            const response = await axios.get<Property[]>('/api/properties');
            setProperties(response.data);
        } catch (err) {
            setError('Ошибка при загрузке данных');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchProperties();
    }, [refreshTrigger]);

    const handleCreateDeal = async (property: Property) => {
        const dealData = {
            propertyId: property.id,
            realtorId: 1, // Захардкожено для примера
            clientId: 1,  // Захардкожено для примера
            dealDate: new Date().toISOString(),
            finalPrice: property.price
        };

        try {
            await axios.post('/api/deals', dealData);
            alert(`Объект по адресу "${property.address}" успешно продан!`);
            fetchProperties(); // Обновляем список
        } catch (err: any) {
            const errorMessage = err.response?.data || 'Произошла ошибка при оформлении сделки';
            alert(errorMessage);
        }
    };

    const getStatusColor = (status: PropertyStatus) => {
        switch (status) {
            case PropertyStatus.Available: return '#10b981'; // Green
            case PropertyStatus.Booked: return '#f59e0b';    // Orange
            case PropertyStatus.Sold: return '#ef4444';      // Red
            default: return '#6b7280';
        }
    };

    const getStatusText = (status: PropertyStatus) => {
        switch (status) {
            case PropertyStatus.Available: return 'Доступно';
            case PropertyStatus.Booked: return 'Забронировано';
            case PropertyStatus.Sold: return 'Продано';
            default: return 'Неизвестно';
        }
    };

    const getTypeIcon = (type: PropertyType) => {
        switch (type) {
            case PropertyType.Apartment: return <Building size={20} />;
            case PropertyType.House: return <Home size={20} />;
            case PropertyType.Commercial: return <Building2 size={20} />;
            default: return <Info size={20} />;
        }
    };

    if (loading) return <div className="loading">Загрузка...</div>;
    if (error) return <div className="error">{error}</div>;

    return (
        <div className="property-grid">
            {properties.map(property => (
                <div key={property.id} className="property-card">
                    <div className="card-header">
                        <div className="icon-box">
                            {getTypeIcon(property.type)}
                        </div>
                        <span 
                            className="status-badge"
                            style={{ backgroundColor: getStatusColor(property.status) }}
                        >
                            {getStatusText(property.status)}
                        </span>
                    </div>

                    <h3 className="property-address">{property.address}</h3>
                    <p className="property-description">{property.description}</p>
                    
                    <div className="property-footer">
                        <div className="price-tag">
                            <Tag size={20} />
                            <span>{property.price.toLocaleString('ru-RU')} ₽</span>
                        </div>
                        <div className="area-info">
                            <MapPin size={16} />
                            <span>{property.area} м²</span>
                        </div>
                        
                        {property.status === PropertyStatus.Available && (
                            <button 
                                className="btn-sell"
                                onClick={(e) => {
                                    e.stopPropagation();
                                    handleCreateDeal(property);
                                }}
                            >
                                Оформить продажу
                            </button>
                        )}
                    </div>
                </div>
            ))}
            {properties.length === 0 && (
                <div className="empty-state">
                    Нет доступных объектов
                </div>
            )}
        </div>
    );
};

export default PropertyList;
