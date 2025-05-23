import API from './client';

export const searchItems = async (word) => {
    const res = await API.get(`/search/${word}`);
    // повертаємо або дані, або res.data — залежить від бекенду
    return res.data;
};