import API from './client';

export const addToCart = (data) => API.post('/cart/add', data);
export const addToCartBatch = (data) => API.post('/cart/add-batch', data);
export const removeFromCart = (itemId) => API.delete(`/cart/${itemId}`);
export const getCart = () => API.get('/cart');
export const updateCartQuantity = (data) => API.patch('/cart/update', data);