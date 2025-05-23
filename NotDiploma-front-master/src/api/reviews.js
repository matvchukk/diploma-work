import API from './client';

export const makeReview = (data) => API.post('/reviews', data);

export const getReviews = (params) =>
  API.get('/reviews', { params });

export const deleteReview = (reviewId) => API.delete(`/reviews/${reviewId}`);
