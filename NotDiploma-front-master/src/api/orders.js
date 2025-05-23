import API from './client';

export const makeOrder = (orderData) => API.post('/orders', orderData);

export const getOrders = () => API.get('/orders');

export const clearOrderHistory = () => API.delete('/orders');

export const updateOrderStatus = (updateData) => API.patch('/orders/status', updateData);
