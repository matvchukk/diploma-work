import { Link, useNavigate } from 'react-router-dom';
import '../Login/Login.css';
import { useState } from 'react';
import { registerSeller } from '../api/sellers'; // Функція для реєстрації продавця

export default function RegSeller() {
    const [storeName, setStoreName] = useState('');
    const [storeDescription, setStoreDescription] = useState('');
    const [contactEmail, setContactEmail] = useState('');
    const [contactPhone, setContactPhone] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setErrorMessage('');
        try {
            await registerSeller({
                storeName,
                storeDescription,
                contactAddress: contactEmail, // правильно передаємо email як ContactAddress
                contactPhone
            });
            navigate('/'); // Або на сторінку магазину
        } catch (error) {
            setErrorMessage('Failed to register store. Please try again.' + error);
        }
    };

    return (
        <div className="box">
            <div className="lrheader">
                <header>Register Store</header>
            </div>
            {errorMessage && <p className="error-message">{errorMessage}</p>}
            <form onSubmit={handleSubmit}>
                <div className="input-box">
                    <input
                        type="text"
                        className="input-field"
                        placeholder="Store Name"
                        autoComplete="off"
                        required
                        value={storeName}
                        onChange={(e) => setStoreName(e.target.value)}
                    />
                </div>
                <div className="input-box">
                    <input
                        type="text"
                        className="input-field"
                        placeholder="Store Description"
                        autoComplete="off"
                        required
                        value={storeDescription}
                        onChange={(e) => setStoreDescription(e.target.value)}
                    />
                </div>
                <div className="input-box">
                    <input
                        type="email"
                        className="input-field"
                        placeholder="Contact Email"
                        autoComplete="off"
                        required
                        value={contactEmail}
                        onChange={(e) => setContactEmail(e.target.value)}
                    />
                </div>
                <div className="input-box">
                    <input
                        type="tel"
                        className="input-field"
                        placeholder="Contact Phone"
                        autoComplete="off"
                        required
                        value={contactPhone}
                        onChange={(e) => setContactPhone(e.target.value)}
                    />
                </div>
                <div className="input-submit">
                    <button className="submit-btn" id="submit" type="submit">Sign Up</button>
                </div>
            </form>

            <div className="sign-in-link">
                <p>Already have an account? <Link to="/login">Login</Link></p>
            </div>
        </div>
    );
}