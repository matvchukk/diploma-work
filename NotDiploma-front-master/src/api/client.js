import axios from "axios";

export const API = axios.create({
  baseURL: "https://localhost:54680/api",
  withCredentials: true, // важливо для HttpOnly cookie
});

export default API;