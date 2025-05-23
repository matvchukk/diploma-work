import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import NavElement, { DropDownItem } from './NavElement';
import CartModal from '../ShoppingCart/CartModal';
import SearchElement from './SearchElement';
import { useAuth } from '../Contexts/AuthProvider';
import { getCart } from '../api/cart';

export default function Header() {
    const { isAuthenticated, logout, switchToRole, role, userId } = useAuth();
    const navigate = useNavigate();
    const [cartItems, setCartItems] = useState([]);
    const [isCartModalOpen, setIsCartModalOpen] = useState(false);

    // cart тільки для user!
    const handleCartClick = () => {
        fetchShoppingCartData();
        setIsCartModalOpen(true);
    };

    const closeCartModal = () => setIsCartModalOpen(false);

    const fetchShoppingCartData = async () => {
        try {
            const response = await getCart();
            setCartItems(response.data);
        } catch (error) {
            console.log(error);
        }
    };

    // Перемикання між аккаунтами
    const handleSwitchAccount = () => {
        switchToRole(role === "user" ? "seller" : "user");
    };

    const handleViewAnalytic = () => navigate('/SellersAnalytic');

    return (
        <header className="header d-flex justify-content-between align-items-center">
            <div className="logo">
                <h1>
                    <Link to="/" style={{ textDecoration: 'none', color: 'inherit' }}>Avet</Link>
                </h1>
            </div>
            <SearchElement />
            <nav className="nav">
                <ul className="nav-list d-flex">
                    {role === "user" && (
                        <>
                            <NavElement onClick={handleCartClick} text="Cart" />
                            <NavElement to="/WishLists" text="Wishlists" />
                        </>
                    )}

                    {isAuthenticated ? (
                        <NavElement text="Profile">
                            <DropDownItem
                                text="Profile Details"
                                to={role === "user" ? `/User/${userId}` : `/Seller`}
                            />
                            <DropDownItem
                                text={role === "user" ? "Switch to Seller" : "Switch to User"}
                                onClick={handleSwitchAccount}
                            />
                            {role === "seller" && (
                                <DropDownItem text="Analytic" onClick={handleViewAnalytic} />
                            )}
                            <DropDownItem text="Logout" onClick={logout} />
                        </NavElement>
                    ) : (
                        <NavElement text="Log In" to="/loginUser" />
                    )}
                </ul>
            </nav>
            <CartModal items={cartItems} show={isCartModalOpen} onClose={closeCartModal} refreshCart={fetchShoppingCartData} />
        </header>
    );
}