import { createContext, useContext, useEffect, useState } from 'react';
import {
  login as loginUser,
  logout as logoutUser,
  register as registerUser,
  getUserProfile,
} from '../api/users';

import {
  loginAsSeller,
  getSellerProfile,
  switchToUser
} from '../api/sellers';

import PropTypes from 'prop-types';
import { useNavigate } from 'react-router-dom';

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);        // профіль користувача або продавця
  const [loading, setLoading] = useState(true);  // завантаження профілю
  const navigate = useNavigate();
  // === Завантажити профіль на старті
  useEffect(() => {
  const fetchProfile = async () => {
    try {
      // 1. Пробуємо отримати профіль користувача
      const res = await getUserProfile();
      setUser({
        id: res.data.id,
        role: 'user',
        ...res.data
      });
    } catch (error) {
      try {
        console.log(error);
        const res = await getSellerProfile();
        if (res.data?.role === "seller" && res.data?.seller) {
          setUser({
            id: res.data.seller.id,
            role: 'seller',
            ...res.data.seller
          });
        } else if (res.data?.id) {
          setUser({
            id: res.data.id,
            role: 'seller',
            ...res.data
          });
        } else {
          setUser(null);
        }
      } catch (error2) {
        console.log(error2);
        setUser(null);
      }
    } finally {
      setLoading(false);
    }
  };

  fetchProfile();
}, []);

  // === Вхід як користувач
  const login = async (credentials) => {
    await loginUser(credentials);
    const res = await getUserProfile();
    setUser(res.data);
  };

  // === Реєстрація як користувач
  const register = async (credentials) => {
    await registerUser(credentials);
    const res = await getUserProfile();
    setUser(res.data);
  };

  // === Вихід
  const logout = async () => {
    await logoutUser();
    setUser(null);
  };

  // === Примусове оновлення профілю (user або seller)
  const refreshProfile = async () => {
    try {
      // Пробуємо отримати профіль користувача
      const res = await getUserProfile();
      setUser(res.data);
    } catch {
      try {
        // Якщо не вдалось — можливо, токен продавця
        const res = await getSellerProfile();
        setUser(res.data);
      } catch (error) {
        console.error('❌ Не вдалося оновити профіль:', error);
        setUser(null);
      }
    }
  };

  // === Перемикання між ролями
  const switchToRole = async (targetRole) => {
  try {
    if (targetRole === 'seller') {
      await loginAsSeller();
      const res = await getSellerProfile();
      setUser({
        id: res.data.seller.id,
        role: 'seller',
        storeName: res.data.seller.storeName,
        storeDescription: res.data.seller.storeDescription,
        contactAddress: res.data.seller.contactAddress,
        contactPhone: res.data.seller.contactPhone,
        products: res.data.seller.products
    });
      console.log(res.data);

      // Дочекайся оновлення userId (через useEffect) і тільки тоді роби navigate
      // Тому краще навігацію винести в useEffect компонента SellerProfile:
      // navigate(`/Seller/${res.data.id}`);
    } else if (targetRole === 'user') {
      await switchToUser();
      const res = await getUserProfile();
      setUser(res.data);

      // navigate(`/User/${res.data.id}`);
    }
  } catch (error) {
    console.log(error);
    navigate('/register-seller');
  }
};

  return (
    <AuthContext.Provider
      value={{
        user,
        loading,
        login,
        register,
        logout,
        refreshProfile,
        switchToRole,
        isAuthenticated: !!user,
        role: user?.role,
        userId: user?.id,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

AuthProvider.propTypes = {
  children: PropTypes.node.isRequired,
};

export const useAuth = () => useContext(AuthContext);