import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { PropertyType, PropertyStatus } from '../types/models';
import type { Property, Booking, Deal, User } from '../types/models';
import { PlusCircle, Search, MapPin, Tag, Minimize2, Calendar, Check, X, ShieldAlert, FileText, DollarSign, Percent, BarChart3, UserCheck, Briefcase } from 'lucide-react';

const RealtorDashboard: React.FC = () => {
  const [properties, setProperties] = useState<Property[]>([]);
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [deals, setDeals] = useState<Deal[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  // Tab control
  const [activeTab, setActiveTab] = useState<'properties' | 'bookings' | 'deals'>('properties');

  // Search/Filter for properties tab
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedStatus, setSelectedStatus] = useState<string>('all');

  // Modal State for adding property
  const [showAddModal, setShowAddModal] = useState(false);
  const [address, setAddress] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('');
  const [area, setArea] = useState('');
  const [type, setType] = useState<PropertyType>(PropertyType.Apartment);
  const [photoUrl, setPhotoUrl] = useState('');
  const [addLoading, setAddLoading] = useState(false);

  const currentUser: User = JSON.parse(localStorage.getItem('user') || '{}');

  const fetchProperties = async () => {
    if (!currentUser.id) return;
    try {
      const response = await axios.get<Property[]>('/api/properties', {
        params: {
          realtorId: currentUser.id
        }
      });
      setProperties(response.data);
    } catch (err) {
      console.error('Ошибка при загрузке недвижимости', err);
      setError('Не удалось загрузить ваши объекты недвижимости.');
    }
  };

  const fetchBookings = async () => {
    if (!currentUser.id) return;
    try {
      const response = await axios.get<Booking[]>('/api/bookings', {
        params: {
          realtorId: currentUser.id
        }
      });
      setBookings(response.data);
    } catch (err) {
      console.error('Ошибка при загрузке заявок', err);
    }
  };

  const fetchDeals = async () => {
    if (!currentUser.id) return;
    try {
      const response = await axios.get<Deal[]>('/api/deals', {
        params: {
          realtorId: currentUser.id
        }
      });
      setDeals(response.data);
    } catch (err) {
      console.error('Ошибка при загрузке сделок', err);
    }
  };

  const loadAllData = async () => {
    setLoading(true);
    await Promise.all([fetchProperties(), fetchBookings(), fetchDeals()]);
    setLoading(false);
  };

  useEffect(() => {
    loadAllData();
  }, []);

  const handleOpenAddModal = () => {
    setShowAddModal(true);
  };

  const handleCloseAddModal = () => {
    setShowAddModal(false);
    setAddress('');
    setDescription('');
    setPrice('');
    setArea('');
    setType(PropertyType.Apartment);
    setPhotoUrl('');
  };

  const handleAddProperty = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!currentUser.id) return;

    setAddLoading(true);
    const newProperty = {
      address,
      description,
      price: Number(price),
      area: Number(area),
      type: Number(type),
      status: PropertyStatus.Available,
      realtorId: currentUser.id,
      photoUrl: photoUrl.trim() || 'https://images.unsplash.com/photo-1564013799919-ab600027ffc6?auto=format&fit=crop&w=600&q=80'
    };

    try {
      await axios.post('/api/properties', newProperty);
      alert('Объект недвижимости успешно добавлен!');
      handleCloseAddModal();
      await fetchProperties();
    } catch (err) {
      console.error(err);
      alert('Ошибка при добавлении объекта.');
    } finally {
      setAddLoading(false);
    }
  };

  const handleUpdateBookingStatus = async (bookingId: number, status: 'Approved' | 'Rejected') => {
    const statusMsg = status === 'Approved' ? 'одобрить заявку и оформить сделку' : 'отклонить заявку';
    if (!window.confirm(`Вы уверены, что хотите ${statusMsg}?`)) {
      return;
    }

    try {
      // Отправляем JSON-строку
      await axios.patch(`/api/bookings/${bookingId}/status`, `"${status}"`, {
        headers: {
          'Content-Type': 'application/json'
        }
      });
      alert(status === 'Approved' ? 'Сделка успешно зарегистрирована!' : 'Заявка отклонена, объект возвращен в продажу.');
      await loadAllData();
    } catch (err: any) {
      console.error(err);
      alert(err.response?.data || 'Не удалось обновить статус заявки.');
    }
  };

  // Helper formatting
  const getPropertyTypeName = (type: PropertyType) => {
    switch (type) {
      case PropertyType.Apartment: return 'Квартира';
      case PropertyType.House: return 'Дом';
      case PropertyType.Commercial: return 'Коммерция';
      default: return 'Недвижимость';
    }
  };

  const getPropertyStatusText = (status: PropertyStatus) => {
    switch (status) {
      case PropertyStatus.Available: return 'Доступно';
      case PropertyStatus.Booked: return 'Забронировано';
      case PropertyStatus.Sold: return 'Продано';
      default: return 'Неизвестно';
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

  const getBookingStatusBadge = (status: string) => {
    switch (status) {
      case 'Pending': 
        return <span className="status-badge" style={{ backgroundColor: '#f59e0b' }}>В ожидании</span>;
      case 'Approved': 
        return <span className="status-badge" style={{ backgroundColor: '#10b981' }}>Сделка совершена</span>;
      case 'Rejected': 
        return <span className="status-badge" style={{ backgroundColor: '#ef4444' }}>Отклонено</span>;
      default: 
        return <span className="status-badge" style={{ backgroundColor: '#6b7280' }}>{status}</span>;
    }
  };

  // Find address from property pool or loading
  const getPropertyAddress = (propertyId: number) => {
    const prop = properties.find(p => p.id === propertyId);
    return prop ? prop.address : `Объект #${propertyId}`;
  };

  // Filtering properties
  const filteredProperties = properties.filter(prop => {
    const matchesSearch = prop.address.toLowerCase().includes(searchQuery.toLowerCase()) || 
                          prop.description.toLowerCase().includes(searchQuery.toLowerCase());
    const matchesStatus = selectedStatus === 'all' || prop.status === Number(selectedStatus);
    return matchesSearch && matchesStatus;
  });

  // Calculate statistics
  const totalRevenue = deals.reduce((acc, deal) => acc + deal.finalPrice, 0);
  const commRate = currentUser.commissionRate || 5; // Default to 5% commission rate
  const totalCommission = totalRevenue * (commRate / 100);

  return (
    <div className="realtor-dashboard">
      {/* Realtor Stats & Profile Info */}
      <div className="realtor-header-card welcome-banner">
        <div className="realtor-profile-details">
          <h2>Кабинет риэлтора: {currentUser.fullName || 'Специалист'}</h2>
          <p>Эффективное управление портфелем недвижимости и клиентскими заявками.</p>
          <div className="realtor-meta-badges">
            <span className="meta-badge">
              <Percent size={14} />
              Ставка комиссии: {commRate}%
            </span>
            <span className="meta-badge">
              <Briefcase size={14} />
              Всего сделок: {deals.length}
            </span>
            <span className="meta-badge highlight">
              <DollarSign size={14} />
              Заработано: {totalCommission.toLocaleString('ru-RU')} ₽
            </span>
          </div>
        </div>
        <div className="dashboard-stats-summary">
          <div className="quick-stat-box">
            <span className="stat-label">В продаже</span>
            <span className="stat-num">{properties.filter(p => p.status === PropertyStatus.Available).length}</span>
          </div>
          <div className="quick-stat-box">
            <span className="stat-label">Забронировано</span>
            <span className="stat-num">{properties.filter(p => p.status === PropertyStatus.Booked).length}</span>
          </div>
          <div className="quick-stat-box">
            <span className="stat-label">Продано всего</span>
            <span className="stat-num">{properties.filter(p => p.status === PropertyStatus.Sold).length}</span>
          </div>
        </div>
      </div>

      {/* Tabs Switcher */}
      <div className="tabs-nav-bar">
        <button 
          onClick={() => setActiveTab('properties')}
          className={`tab-btn ${activeTab === 'properties' ? 'active' : ''}`}
        >
          <MapPin size={18} />
          Мои объекты ({properties.length})
        </button>
        <button 
          onClick={() => setActiveTab('bookings')}
          className={`tab-btn ${activeTab === 'bookings' ? 'active' : ''}`}
        >
          <Calendar size={18} />
          Заявки клиентов ({bookings.filter(b => b.status === 'Pending').length} новые)
        </button>
        <button 
          onClick={() => setActiveTab('deals')}
          className={`tab-btn ${activeTab === 'deals' ? 'active' : ''}`}
        >
          <FileText size={18} />
          Архив сделок ({deals.length})
        </button>
      </div>

      {loading ? (
        <div className="loading">Загрузка данных риэлтора...</div>
      ) : error ? (
        <div className="error">{error}</div>
      ) : (
        <div className="tab-content-area animated-fade-in">
          
          {/* PROPERTIES TAB */}
          {activeTab === 'properties' && (
            <div className="properties-tab-view">
              <div className="actions-header-bar">
                <div className="search-filter-controls">
                  <div className="search-input-wrapper">
                    <Search className="search-icon" size={20} />
                    <input 
                      type="text" 
                      placeholder="Поиск объектов по адресу..." 
                      value={searchQuery}
                      onChange={e => setSearchQuery(e.target.value)}
                      className="search-input"
                    />
                  </div>
                  <select 
                    value={selectedStatus}
                    onChange={e => setSelectedStatus(e.target.value)}
                    className="status-filter-select"
                  >
                    <option value="all">Все статусы</option>
                    <option value={PropertyStatus.Available}>Доступные</option>
                    <option value={PropertyStatus.Booked}>Забронированные</option>
                    <option value={PropertyStatus.Sold}>Проданные</option>
                  </select>
                </div>
                <button 
                  onClick={handleOpenAddModal} 
                  className="btn-add-property"
                >
                  <PlusCircle size={18} />
                  Добавить объект
                </button>
              </div>

              {filteredProperties.length === 0 ? (
                <div className="empty-state-card">
                  <ShieldAlert size={48} className="empty-icon" />
                  <h4>Объекты отсутствуют</h4>
                  <p>У вас нет объектов с выбранными критериями фильтрации.</p>
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
                        <div className="card-top-badges">
                          <span className="property-type-tag">
                            {getPropertyTypeName(property.type)}
                          </span>
                          <span 
                            className="status-badge"
                            style={{ backgroundColor: getStatusColor(property.status) }}
                          >
                            {getPropertyStatusText(property.status)}
                          </span>
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
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {/* BOOKINGS TAB */}
          {activeTab === 'bookings' && (
            <div className="bookings-tab-view">
              <h3 className="tab-section-title">Заявки на бронирование просмотра ваших объектов</h3>
              {bookings.length === 0 ? (
                <div className="empty-state-card">
                  <Calendar size={48} className="empty-icon" />
                  <h4>Заявок пока нет</h4>
                  <p>Клиенты еще не оставляли запросов на просмотр ваших объектов недвижимости.</p>
                </div>
              ) : (
                <div className="bookings-table-wrapper">
                  <table className="premium-table">
                    <thead>
                      <tr>
                        <th>Дата подачи</th>
                        <th>Объект недвижимости</th>
                        <th>Клиент (ФИО)</th>
                        <th>Телефон</th>
                        <th>Email</th>
                        <th>Статус</th>
                        <th>Действия</th>
                      </tr>
                    </thead>
                    <tbody>
                      {bookings.map(booking => (
                        <tr key={booking.id}>
                          <td>{new Date(booking.bookingDate).toLocaleDateString('ru-RU')}</td>
                          <td>
                            <div className="table-address">
                              <MapPin size={14} className="text-secondary" />
                              <span>{getPropertyAddress(booking.propertyId)}</span>
                            </div>
                          </td>
                          <td><strong>{booking.clientName}</strong></td>
                          <td>{booking.clientPhone}</td>
                          <td>{booking.clientEmail}</td>
                          <td>{getBookingStatusBadge(booking.status)}</td>
                          <td>
                            {booking.status === 'Pending' ? (
                              <div className="table-actions-cell">
                                <button 
                                  onClick={() => handleUpdateBookingStatus(booking.id, 'Approved')} 
                                  className="btn-action-approve"
                                  title="Одобрить продажу объекта"
                                >
                                  <Check size={16} />
                                  Продано
                                </button>
                                <button 
                                  onClick={() => handleUpdateBookingStatus(booking.id, 'Rejected')} 
                                  className="btn-action-reject"
                                  title="Отклонить заявку"
                                >
                                  <X size={16} />
                                  Отказать
                                </button>
                              </div>
                            ) : (
                              <span className="text-muted">—</span>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}

          {/* DEALS TAB */}
          {activeTab === 'deals' && (
            <div className="deals-tab-view">
              <div className="deals-analytics-grid">
                <div className="analytics-card">
                  <div className="analytics-icon-wrapper blue">
                    <BarChart3 size={24} />
                  </div>
                  <div className="analytics-details">
                    <span className="analytics-label">Оборот продаж</span>
                    <h4>{totalRevenue.toLocaleString('ru-RU')} ₽</h4>
                  </div>
                </div>
                <div className="analytics-card">
                  <div className="analytics-icon-wrapper green">
                    <DollarSign size={24} />
                  </div>
                  <div className="analytics-details">
                    <span className="analytics-label">Чистая комиссия</span>
                    <h4>{totalCommission.toLocaleString('ru-RU')} ₽</h4>
                  </div>
                </div>
                <div className="analytics-card">
                  <div className="analytics-icon-wrapper orange">
                    <UserCheck size={24} />
                  </div>
                  <div className="analytics-details">
                    <span className="analytics-label">Коэффициент конверсии</span>
                    <h4>
                      {bookings.length > 0 
                        ? `${Math.round((deals.length / bookings.length) * 100)}%` 
                        : '0%'
                      }
                    </h4>
                  </div>
                </div>
              </div>

              <h3 className="tab-section-title">История совершенных сделок</h3>
              {deals.length === 0 ? (
                <div className="empty-state-card">
                  <FileText size={48} className="empty-icon" />
                  <h4>Сделок не найдено</h4>
                  <p>В системе пока нет записей о совершенных вами продажах.</p>
                </div>
              ) : (
                <div className="bookings-table-wrapper">
                  <table className="premium-table">
                    <thead>
                      <tr>
                        <th>ID Сделки</th>
                        <th>Дата заключения</th>
                        <th>Объект недвижимости</th>
                        <th>Сумма сделки</th>
                        <th>Ваша комиссия ({commRate}%)</th>
                      </tr>
                    </thead>
                    <tbody>
                      {deals.map(deal => (
                        <tr key={deal.id}>
                          <td># {deal.id}</td>
                          <td>{new Date(deal.dealDate).toLocaleDateString('ru-RU')}</td>
                          <td>
                            <div className="table-address">
                              <MapPin size={14} />
                              {getPropertyAddress(deal.propertyId)}
                            </div>
                          </td>
                          <td><strong>{deal.finalPrice.toLocaleString('ru-RU')} ₽</strong></td>
                          <td className="text-success font-semibold">
                            +{(deal.finalPrice * (commRate / 100)).toLocaleString('ru-RU')} ₽
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}
        </div>
      )}

      {/* Adding property form modal */}
      {showAddModal && (
        <div className="modal-overlay">
          <div className="modal-card animated-zoom-in">
            <div className="modal-header">
              <h3>Добавление нового объекта недвижимости</h3>
              <button onClick={handleCloseAddModal} className="btn-close-modal">
                <X size={20} />
              </button>
            </div>
            
            <form onSubmit={handleAddProperty} className="modal-form">
              <div className="input-group">
                <label className="label">Адрес объекта</label>
                <input 
                  type="text" 
                  value={address} 
                  onChange={e => setAddress(e.target.value)} 
                  required 
                  className="input-field"
                  placeholder="г. Москва, ул. Ленина, д. 15, кв. 42"
                />
              </div>

              <div className="form-grid">
                <div className="input-group">
                  <label className="label">Цена (₽)</label>
                  <input 
                    type="number" 
                    value={price} 
                    onChange={e => setPrice(e.target.value)} 
                    required 
                    className="input-field"
                    placeholder="7500000"
                  />
                </div>

                <div className="input-group">
                  <label className="label">Площадь (м²)</label>
                  <input 
                    type="number" 
                    value={area} 
                    onChange={e => setArea(e.target.value)} 
                    required 
                    className="input-field"
                    placeholder="65"
                  />
                </div>
              </div>

              <div className="form-grid">
                <div className="input-group">
                  <label className="label">Тип недвижимости</label>
                  <select 
                    value={type} 
                    onChange={e => setType(Number(e.target.value) as PropertyType)} 
                    className="input-field"
                  >
                    <option value={PropertyType.Apartment}>Квартира</option>
                    <option value={PropertyType.House}>Дом</option>
                    <option value={PropertyType.Commercial}>Коммерция</option>
                  </select>
                </div>

                <div className="input-group">
                  <label className="label">Ссылка на фото (URL)</label>
                  <input 
                    type="url" 
                    value={photoUrl} 
                    onChange={e => setPhotoUrl(e.target.value)} 
                    className="input-field"
                    placeholder="https://images.unsplash.com/..."
                  />
                </div>
              </div>

              <div className="input-group">
                <label className="label">Краткое описание преимуществ</label>
                <textarea 
                  value={description} 
                  onChange={e => setDescription(e.target.value)} 
                  required
                  className="input-field textarea"
                  placeholder="Отличная светлая квартира в тихом спальном районе с евроремонтом..."
                />
              </div>

              <div className="modal-actions">
                <button 
                  type="button" 
                  onClick={handleCloseAddModal} 
                  className="btn-modal-secondary"
                  disabled={addLoading}
                >
                  Отмена
                </button>
                <button 
                  type="submit" 
                  className="btn-modal-primary"
                  disabled={addLoading}
                >
                  {addLoading ? 'Сохранение...' : 'Опубликовать'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default RealtorDashboard;
