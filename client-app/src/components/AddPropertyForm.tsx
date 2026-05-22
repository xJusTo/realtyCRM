import React, { useState } from 'react';
import axios from 'axios';
import { PropertyType, PropertyStatus } from '../types/models';
import { PlusCircle } from 'lucide-react';

interface Props {
    onPropertyAdded: () => void;
}

const AddPropertyForm: React.FC<Props> = ({ onPropertyAdded }) => {
    const [address, setAddress] = useState('');
    const [price, setPrice] = useState('');
    const [area, setArea] = useState('');
    const [type, setType] = useState<PropertyType>(PropertyType.Apartment);
    const [description, setDescription] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);

        const newProperty = {
            id: Math.floor(Math.random() * 10000), // Простая генерация ID для демонстрации
            address,
            price: Number(price),
            area: Number(area),
            type: Number(type),
            description,
            status: PropertyStatus.Available,
            realtorId: 1 // ID риэлтора по умолчанию
        };

        try {
            await axios.post('/api/properties', newProperty);
            setAddress('');
            setPrice('');
            setArea('');
            setDescription('');
            onPropertyAdded();
        } catch (err) {
            alert('Ошибка при добавлении объекта');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="form-card">
            <h2 className="form-title">
                <PlusCircle size={24} color="#3b82f6" />
                Добавить новый объект
            </h2>
            <div className="form-grid">
                <div className="input-group">
                    <label className="label">Адрес</label>
                    <input 
                        placeholder="ул. Пушкина, д. 10" 
                        value={address} 
                        onChange={e => setAddress(e.target.value)} 
                        required 
                        className="input-field"
                    />
                </div>
                <div className="input-group">
                    <label className="label">Цена (₽)</label>
                    <input 
                        placeholder="5000000" 
                        type="number" 
                        value={price} 
                        onChange={e => setPrice(e.target.value)} 
                        required 
                        className="input-field"
                    />
                </div>
                <div className="input-group">
                    <label className="label">Площадь (м²)</label>
                    <input 
                        placeholder="50" 
                        type="number" 
                        value={area} 
                        onChange={e => setArea(e.target.value)} 
                        required 
                        className="input-field"
                    />
                </div>
                <div className="input-group">
                    <label className="label">Тип недвижимости</label>
                    <select 
                        value={type} 
                        onChange={e => setType(Number(e.target.value))} 
                        className="input-field"
                    >
                        <option value={PropertyType.Apartment}>Квартира</option>
                        <option value={PropertyType.House}>Дом</option>
                        <option value={PropertyType.Commercial}>Коммерция</option>
                    </select>
                </div>
            </div>
            <div className="input-group" style={{ marginTop: '1rem' }}>
                <label className="label">Описание</label>
                <textarea 
                    placeholder="Краткое описание объекта..." 
                    value={description} 
                    onChange={e => setDescription(e.target.value)} 
                    className="input-field textarea"
                />
            </div>
            <button 
                type="submit" 
                disabled={loading}
                className="btn-primary"
            >
                {loading ? 'Добавление...' : 'Опубликовать объект'}
            </button>
        </form>
    );
};

export default AddPropertyForm;
