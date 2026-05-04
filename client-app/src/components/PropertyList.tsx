import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Property, PropertyStatus, PropertyType } from '../types';
import { Home, Building2, Building, Info, MapPin, Tag } from 'lucide-react';

const PropertyList: React.FC = () => {
    const [properties, setProperties] = useState<Property[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
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

        fetchProperties();
    }, []);

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

    if (loading) return <div style={{ textAlign: 'center', padding: '2rem' }}>Загрузка...</div>;
    if (error) return <div style={{ color: 'red', textAlign: 'center', padding: '2rem' }}>{error}</div>;

    return (
        <div style={{ padding: '2rem', maxWidth: '1200px', margin: '0 auto' }}>
            <h1 style={{ marginBottom: '2rem', color: '#1f2937', textAlign: 'center' }}>Объекты недвижимости</h1>
            <div style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
                gap: '1.5rem'
            }}>
                {properties.map(property => (
                    <div key={property.id} style={{
                        backgroundColor: '#fff',
                        borderRadius: '12px',
                        boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
                        padding: '1.5rem',
                        transition: 'transform 0.2s',
                        cursor: 'pointer',
                        border: '1px solid #f3f4f6'
                    }}
                        onMouseEnter={(e) => e.currentTarget.style.transform = 'translateY(-5px)'}
                        onMouseLeave={(e) => e.currentTarget.style.transform = 'translateY(0)'}
                    >
                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                            <div style={{
                                padding: '0.5rem',
                                backgroundColor: '#f3f4f6',
                                borderRadius: '8px',
                                color: '#4b5563'
                            }}>
                                {getTypeIcon(property.type)}
                            </div>
                            <span style={{
                                backgroundColor: getStatusColor(property.status),
                                color: '#fff',
                                padding: '0.25rem 0.75rem',
                                borderRadius: '9999px',
                                fontSize: '0.875rem',
                                fontWeight: 500
                            }}>
                                {getStatusText(property.status)}
                            </span>
                        </div>

                        <h3 style={{ margin: '0 0 0.5rem 0', color: '#111827' }}>{property.address}</h3>
                        <p style={{ color: '#6b7280', fontSize: '0.875rem', marginBottom: '1rem' }}>{property.description}</p>

                        <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', color: '#374151' }}>
                                <Tag size={16} />
                                <span style={{ fontWeight: 600, fontSize: '1.25rem' }}>
                                    {property.price.toLocaleString('ru-RU')} ₽
                                </span>
                            </div>
                            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', color: '#6b7280', fontSize: '0.875rem' }}>
                                <MapPin size={16} />
                                <span>{property.area} м²</span>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
            {properties.length === 0 && (
                <div style={{ textAlign: 'center', color: '#6b7280', marginTop: '2rem' }}>
                    Нет доступных объектов
                </div>
            )}
        </div>
    );
};

export default PropertyList;
