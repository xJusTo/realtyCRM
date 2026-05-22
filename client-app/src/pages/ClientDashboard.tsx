import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { PropertyType, PropertyStatus } from '../types/models';
import type { Property, Booking } from '../types/models';
import { Search, SlidersHorizontal, MapPin, Tag, Minimize2, Calendar, X, ShieldAlert } from 'lucide-react';

const ClientDashboard: React.FC = () => {
  const [properties, setProperties] = useState<Property[]>([]);
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Filters State
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedType, setSelectedType] = useState<string>('all');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [minArea, setMinArea] = useState('');
  const [maxArea, setMaxArea] = useState('');
  const [showFilters, setShowFilters] = useState(false);

  // Booking Modal State
  const [selectedProperty, setSelectedProperty] = useState<Property | null>(null);
  const [clientName, setClientName] = useState('');
  const [clientPhone, setClientPhone] = useState('');
  const [clientEmail, setClientEmail] = useState('');
  const [bookingLoading, setBookingLoading] = useState(false);

  const currentUser = JSON.parse(localStorage.getItem('user') || '{}');

  const fetchProperties = async () => {
    try {
      // Запрашиваем только доступные объекты недвижимости
      const response = await axios.get<Property[]>('/api/properties', {
        params: {
          status: PropertyStatus.Available
        }
      });
      setProperties(response.data);
    } catch (err) {
      console.error('Ошибка при загрузке недвижимости', err);
      setError('Не удалось загрузить каталог недвижимости.');
    }
  };

  const fetchBookings = async () => {
    if (!currentUser.email) return;
    try {
      const response = await axios.get<Booking[]>('/api/bookings', {
        params: {
          clientEmail: currentUser.email
        }
      });
      setBookings(response.data);
    } catch (err) {
      console.error('Ошибка при загрузке бронирований', err);
    }
  };

  useEffect(() => {
    const loadData = async () => {
      setLoading(true);
      await Promise.all([fetchProperties(), fetchBookings()]);
      setLoading(false);
    };
    loadData();

    // Заполняем контакты из профиля
    if (currentUser.fullName) setClientName(currentUser.fullName);
    if (currentUser.phone) setClientPhone(currentUser.phone);
    if (currentUser.email) setClientEmail(currentUser.email);
  }, []);

  const handleOpenBooking = (property: Property) => {
    setSelectedProperty(property);
  };

  const handleCloseBooking = () => {
    setSelectedProperty(null);
  };

  const handleCreateBooking = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedProperty) return;

    setBookingLoading(true);
    const bookingData = {
      propertyId: selectedProperty.id,
      clientName,
      clientPhone,
      clientEmail,
    };

    try {
      await axios.post('/api/bookings', bookingData);
      alert(`Вы успешно оставили заявку на просмотр объекта по адресу: ${selectedProperty.address}! Наш риэлтор свяжется с вами.`);
      handleCloseBooking();
      await Promise.all([fetchProperties(), fetchBookings()]);
    } catch (err: any) {
      alert(err.response?.data || 'Произошла ошибка при бронировании.');
    } finally {
      setBookingLoading(false);
    }
  };

  // Client-side filtering logic
  const filteredProperties = properties.filter(prop => {
    const matchesSearch = prop.address.toLowerCase().includes(searchQuery.toLowerCase()) || 
                          prop.description.toLowerCase().includes(searchQuery.toLowerCase());
    const matchesType = selectedType === 'all' || prop.type === Number(selectedType);
    const matchesMinPrice = !minPrice || prop.price >= Number(minPrice);
    const matchesMaxPrice = !maxPrice || prop.price <= Number(maxPrice);
    const matchesMinArea = !minArea || prop.area >= Number(minArea);
    const matchesMaxArea = !maxArea || prop.area <= Number(maxArea);

    return matchesSearch && matchesType && matchesMinPrice && matchesMaxPrice && matchesMinArea && matchesMaxArea;
  });

  const getPropertyTypeName = (type: PropertyType) => {
    switch (type) {
      case PropertyType.Apartment: return 'Квартира';
      case PropertyType.House: return 'Дом';
      case PropertyType.Commercial: return 'Коммерция';
      default: return 'Недвижимость';
    }
  };

  const getBookingStatusBadge = (status: string) => {
    switch (status) {
      case 'Pending': 
        return <span className="status-badge" style={{ backgroundColor: '#f59e0b' }}>В ожидании</span>;
      case 'Approved': 
        return <span className="status-badge" style={{ backgroundColor: '#10b981' }}>Сделка совершена</span>;
      case 'Rejected': 
        return <span className="status-badge" style={{ backgroundColor: '#ef4444' }}>Отменено</span>;
      default: 
        return <span className="status-badge" style={{ backgroundColor: '#6b7280' }}>{status}</span>;
    }
  };

  // Lookup map to get property details for bookings
  const getPropertyAddress = (propertyId: number) => {
    const prop = properties.find(p => p.id === propertyId);
    return prop ? prop.address : `Объект #${propertyId}`;
  };

  return (
    <div className="client-dashboard">
      <div className="welcome-banner">
        <h2>Здравствуйте, {currentUser.fullName || 'Гость'}!</h2>
        <p>Изучите наш каталог и найдите идеальное место для жизни или бизнеса.</p>
      </div>

      {/* Search and Filters Section */}
      <div className="search-filter-section">
        <div className="search-bar-row">
          <div className="search-input-wrapper">
            <Search className="search-icon" size={20} />
            <input 
              type="text" 
              placeholder="Поиск по адресу или описанию..." 
              value={searchQuery}
              onChange={e => setSearchQuery(e.target.value)}
              className="search-input"
            />
          </div>
          <button 
            onClick={() => setShowFilters(!showFilters)} 
            className={`btn-filters ${showFilters ? 'active' : ''}`}
          >
            <SlidersHorizontal size={18} />
            Фильтры
          </button>
        </div>

        {showFilters && (
          <div className="filters-grid-card animated-fade-in">
            <div className="filter-input-group">
              <label className="label">Тип недвижимости</label>
              <select 
                value={selectedType} 
                onChange={e => setSelectedType(e.target.value)} 
                className="input-field"
              >
                <option value="all">Все типы</option>
                <option value={PropertyType.Apartment}>Квартира</option>
                <option value={PropertyType.House}>Дом</option>
                <option value={PropertyType.Commercial}>Коммерция</option>
              </select>
            </div>

            <div className="filter-input-group">
              <label className="label">Цена от (₽)</label>
              <input 
                type="number" 
                placeholder="Минимум" 
                value={minPrice} 
                onChange={e => setMinPrice(e.target.value)} 
                className="input-field"
              />
            </div>

            <div className="filter-input-group">
              <label className="label">Цена до (₽)</label>
              <input 
                type="number" 
                placeholder="Максимум" 
                value={maxPrice} 
                onChange={e => setMaxPrice(e.target.value)} 
                className="input-field"
              />
            </div>

            <div className="filter-input-group">
              <label className="label">Площадь от (м²)</label>
              <input 
                type="number" 
                placeholder="Мин" 
                value={minArea} 
                onChange={e => setMinArea(e.target.value)} 
                className="input-field"
              />
            </div>

            <div className="filter-input-group">
              <label className="label">Площадь до (м²)</label>
              <input 
                type="number" 
                placeholder="Макс" 
                value={maxArea} 
                onChange={e => setMaxArea(e.target.value)} 
                className="input-field"
              />
            </div>
          </div>
        )}
      </div>

      <div className="catalog-layout">
        {/* Properties Catalog */}
        <div className="catalog-main">
          <h3 className="section-title">Каталог недвижимости ({filteredProperties.length})</h3>
          
          {loading ? (
            <div className="loading">Загрузка каталога...</div>
          ) : error ? (
            <div className="error">{error}</div>
          ) : filteredProperties.length === 0 ? (
            <div className="empty-state-card">
              <ShieldAlert size={48} className="empty-icon" />
              <h4>Объекты не найдены</h4>
              <p>Попробуйте сбросить фильтры или изменить поисковый запрос.</p>
            </div>
          ) : (
            <div className="properties-grid-premium">
              {filteredProperties.map(property => (
                <div key={property.id} className="property-card-premium">
                  {property.photoUrl ? (
                    <img 
                      src={property.photoUrl} 
                      alt={property.address} 
                      className="property-image-premium"
                    />
                  ) : (
                    <div className="property-image-placeholder">
                      <span>Нет фото</span>
                    </div>
                  )}

                  <div className="property-card-content">
                    <div className="property-type-tag">
                      {getPropertyTypeName(property.type)}
                    </div>
                    <h4 className="property-addr">{property.address}</h4>
                    <p className="property-desc">{property.description}</p>
                    
                    <div className="property-stats">
                      <div className="stat-item">
                        <Tag size={16} />
                        <span className="price-val">{property.price.toLocaleString('ru-RU')} ₽</span>
                      </div>
                      <div className="stat-item">
                        <Minimize2 size={16} />
                        <span>{property.area} м²</span>
                      </div>
                    </div>

                    <button 
                      onClick={() => handleOpenBooking(property)} 
                      className="btn-book"
                    >
                      Забронировать просмотр
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* User Bookings Side Panel */}
        <div className="bookings-sidebar">
          <h3 className="section-title">Мои заявки</h3>
          <div className="bookings-list-card">
            {bookings.length === 0 ? (
              <div className="empty-bookings">
                <Calendar size={32} />
                <p>У вас пока нет активных заявок на просмотр.</p>
              </div>
            ) : (
              <div className="sidebar-bookings">
                {bookings.map(booking => (
                  <div key={booking.id} className="booking-item-card">
                    <div className="booking-item-header">
                      <span className="booking-date">
                        {new Date(booking.bookingDate).toLocaleDateString('ru-RU')}
                      </span>
                      {getBookingStatusBadge(booking.status)}
                    </div>
                    <div className="booking-property-addr">
                      <MapPin size={14} />
                      {getPropertyAddress(booking.propertyId)}
                    </div>
                    <div className="booking-client-info">
                      <p><strong>Вы указали:</strong></p>
                      <p>{booking.clientName}</p>
                      <p>{booking.clientPhone}</p>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Booking Form Modal Overlay */}
      {selectedProperty && (
        <div className="modal-overlay">
          <div className="modal-card animated-zoom-in">
            <div className="modal-header">
              <h3>Бронирование просмотра</h3>
              <button onClick={handleCloseBooking} className="btn-close-modal">
                <X size={20} />
              </button>
            </div>
            
            <form onSubmit={handleCreateBooking} className="modal-form">
              <div className="modal-property-preview">
                <p className="preview-label">Вы бронируете просмотр объекта:</p>
                <h4 className="preview-address">{selectedProperty.address}</h4>
                <p className="preview-price">{selectedProperty.price.toLocaleString('ru-RU')} ₽</p>
              </div>

              <div className="input-group">
                <label className="label">Ваше ФИО</label>
                <input 
                  type="text" 
                  value={clientName} 
                  onChange={e => setClientName(e.target.value)} 
                  required 
                  className="input-field"
                  placeholder="Иван Иванов"
                />
              </div>

              <div className="input-group">
                <label className="label">Номер телефона для связи</label>
                <input 
                  type="tel" 
                  value={clientPhone} 
                  onChange={e => setClientPhone(e.target.value)} 
                  required 
                  className="input-field"
                  placeholder="+79990001122"
                />
              </div>

              <div className="input-group">
                <label className="label">Электронная почта</label>
                <input 
                  type="email" 
                  value={clientEmail} 
                  onChange={e => setClientEmail(e.target.value)} 
                  required 
                  className="input-field"
                  placeholder="client@mail.com"
                  disabled={!!currentUser.email} // Отключаем, если автозаполнено из профиля
                />
              </div>

              <div className="modal-actions">
                <button 
                  type="button" 
                  onClick={handleCloseBooking} 
                  className="btn-modal-secondary"
                  disabled={bookingLoading}
                >
                  Отмена
                </button>
                <button 
                  type="submit" 
                  className="btn-modal-primary"
                  disabled={bookingLoading}
                >
                  {bookingLoading ? 'Отправка...' : 'Отправить заявку'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default ClientDashboard;
