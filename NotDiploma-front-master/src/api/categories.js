import API from './client';

export const getAllCategories = () => API.get('/categories');

export const createCategory = (category) => API.post('/categories', category);

export const createCategoriesBatch = (categories) => API.post('/categories/batch', categories);

export const updateCategory = (id, category) => API.put(`/categories/${id}`, category);

export const deleteCategory = (id) => API.delete(`/categories/${id}`);