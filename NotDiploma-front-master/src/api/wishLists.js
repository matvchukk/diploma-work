import API from './client';

export const getUserWishlists = () => API.get('/wishlists');

export const createWishlist = (name) =>
  API.post('/wishlists', {name});

export const addProductToWishlist = (wishlistId, productId) =>
  API.post(`/wishlists/${wishlistId}/products/${productId}`);

export const removeProductFromWishlist = (wishlistId, productId) =>
  API.delete(`/wishlists/${wishlistId}/products/${productId}`);

export const deleteWishlist = (wishlistId) =>
  API.delete(`/wishlists/${wishlistId}`);

// Додаємо метод для перейменування списку бажань
export const renameWishlist = (wishlistId, newWishListName) =>
  API.patch(`/wishlists/${wishlistId}`, { newWishListName });