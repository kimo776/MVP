import axios from 'axios';

const api = axios.create({ baseURL: 'http://localhost:8080/api' });

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export const login = (email, password) => api.post('/auth/login', { email, password }).then(r => r.data);

export const getProductOrders = (filters = {}) => api.get('/productorders', { params: filters }).then(r => r.data);
export const getMyProductOrders = (customerId) => api.get(`/productorders/my/${customerId}`).then(r => r.data);
export const createProductOrder = (data) => api.post('/productorders', data).then(r => r.data);
export const updateProductOrder = (id, data) => api.put(`/productorders/${id}`, data).then(r => r.data);
export const deleteProductOrder = (id) => api.delete(`/productorders/${id}`);

export const getDeliveryTasks = () => api.get('/deliverytasks').then(r => r.data);
export const getMyDeliveryTasks = (deliveryUserId) => api.get(`/deliverytasks/my/${deliveryUserId}`).then(r => r.data);
export const createDeliveryTask = (data) => api.post('/deliverytasks', data).then(r => r.data);
export const updateDeliveryTaskStatus = (id, status, notes = '') => api.patch(`/deliverytasks/${id}/status`, { status, notes }).then(r => r.data);
export const deleteDeliveryTask = (id) => api.delete(`/deliverytasks/${id}`);

export const getShipments = (filters = {}) => api.get('/shipments', { params: filters }).then(r => r.data);
export const getMyShipments = (customerId) => api.get(`/shipments/my/${customerId}`).then(r => r.data);
export const createShipment = (data) => api.post('/shipments', data).then(r => r.data);
export const updateShipment = (id, data) => api.put(`/shipments/${id}`, data).then(r => r.data);
export const updateShipmentStatus = (id, status) => api.patch(`/shipments/${id}/status`, { status }).then(r => r.data);
export const deleteShipment = (id) => api.delete(`/shipments/${id}`);

export const getUsers = () => api.get('/admin/users').then(r => r.data);
export const createUser = (data) => api.post('/admin/users', data).then(r => r.data);
export const updateUser = (id, data) => api.put(`/admin/users/${id}`, data).then(r => r.data);
export const deleteUser = (id) => api.delete(`/admin/users/${id}`);
