import { useState } from 'react';
import './Login.css';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../Contexts/AuthProvider';

export default function Login() {
    const navigate = useNavigate();
    const { login } = useAuth();

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!email || !password) {
            setErrorMessage('Please fill in all fields.');
            return;
        }

        try {
            await login({ Email: email, Password: password });
            navigate('/'); // Перенаправлення після входу
        } catch (error) {
            // Axios повертає помилку тут, якщо статус 401
            setErrorMessage(
                error?.response?.data?.error ||
                'Authentication failed. Please check your credentials.'
            );
        }
    };

    return (
        <div className="box">
            <div className="lrheader">
                <header>Login</header>
            </div>
            <form onSubmit={handleSubmit}>
                <div className="input-box">
                    <input
                        type="text"
                        className="input-field"
                        placeholder="Email"
                        autoComplete="off"
                        required
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </div>
                <div className="input-box">
                    <input
                        type="password"
                        className="input-field"
                        placeholder="Password"
                        autoComplete="off"
                        required
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </div>
                {errorMessage && <div className="error-message">{errorMessage}</div>}
                <div className="input-submit">
                    <button type="submit" className="submit-btn" id="submit">Sign In</button>
                </div>
                <div className="sign-up-link">
                    <p>Don&apos;t have an account? <Link to="/registration">Sign Up</Link></p>
                </div>
            </form>
        </div>
    );
}
