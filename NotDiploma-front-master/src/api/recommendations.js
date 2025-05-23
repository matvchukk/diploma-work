import API from './client';

export const getRecommendations = (data) => API.get('/recommendations', data);

export const getDailyMenu = () => API.get('/recommendations/menu/daily');
