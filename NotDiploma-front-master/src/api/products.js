import API from './client';

export const getProducts = (params = {}) =>
  API.get('/products', { params });

export const getProductById = (id) =>
  API.get(`/products/${id}`);

export const createProduct = (data) =>
  API.post('/products', data);

export const updateProduct = (data) =>
  API.put('/products', data);

export const deleteProduct = (id) =>
  API.delete(`/products/${id}`);