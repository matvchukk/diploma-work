import API from './client';

export const registerSeller = (data) => API.post('/sellers/register', data);

export const loginAsSeller = () => API.post('/sellers/login');

export const getSellerById = (id) => API.get(`/sellers/${id}`);

export const updateSeller = (data) => API.put('/sellers', data);

export const deleteSeller = () => API.delete('/sellers');

export const getSellerOrders = () => API.get('/sellers/orders');

export const switchToUser = () => API.post('/users/switch');

export const getSellerProfile = () => API.get('/sellers/me');

