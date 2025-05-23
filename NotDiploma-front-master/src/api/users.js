import API from './client';

export const register = (data) => API.post('/users/register', data);

export const login = (data) => API.post('/users/login', data);

export const logout = () => API.post('/users/logout');

export const updateUser = (data) => API.put('/users', data);

export const deleteUser = () => API.delete('/users');


export const getUserProfile = () => API.get('/users/profile');

// Отримати профіль продавця (з SellersController)
export const getSellerProfile = () => API.get('/sellers/profile');
