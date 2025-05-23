import './App.css'
import 'bootstrap/dist/css/bootstrap.min.css'
import Header from './Header/Header'
import Main from './Main/Main'
import FooterComponent from './Footer/Footer'
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Login from './Login/Login'
import Registration from './Reg/Reg'
import UserProfile from './Profile/UserProfile'
import SellerProfile from './Profile/SellerProfile'
import { AuthProvider } from './Contexts/AuthProvider';
import RegSeller from './Reg/RegSeller'
import Category from './ListGroup/Category'
import Product from './Product/Product'
import WishLists from './UserAtributes/WishLists'
import MakeOrder from './Order/MakeOrder'
import ReviewsPage from './Review/ReviewsPage'

function App() {
  return (
    <>
    <BrowserRouter>
      <AuthProvider>
          <Header />
          <Routes>
            <Route path="/" index element={<Main />} />
            <Route path="/loginUser" element={<Login />} />
            <Route path="/registration" element={<Registration />} />
            <Route path="/Seller" element={<SellerProfile />} />
            <Route path="/Seller/:sellerId" element={<SellerProfile />} />
            <Route path="/User/:userID" element={<UserProfile />} />
            <Route path="/register-seller" element={<RegSeller/>} />
            <Route path="/Category/:categoryId" element={<Category />} />
            <Route path="/Product/:productId" element={<Product />} />
            <Route path="/WishLists" element={<WishLists />} />
            <Route path="/MakeOrder" element={<MakeOrder />} />
            <Route path="/ReviewsPage" element={<ReviewsPage />} />
          </Routes>
          <FooterComponent />
        </AuthProvider>
      </BrowserRouter>
    </>
  );
}



export default App
