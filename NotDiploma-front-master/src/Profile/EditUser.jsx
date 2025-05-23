import { useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import { useAuth } from '../Contexts/AuthProvider';
import { updateUser } from '../api/users'; // <-- Функція для оновлення користувача

export default function EditUser({ userData, onClose }) {
    const { userId } = useAuth(); // userId із кастомного хука
    const [newUserData, setNewUserData] = useState({
        UserName: userData.userName,
        Email: userData.email,
        Password: '', // Можна залишати порожнім, якщо не змінюється
        WalletAddress: userData.walletAddress || ''
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setNewUserData(prevState => ({ ...prevState, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await updateUser(userId, newUserData); // Використовуємо API-модуль
            onClose();
        } catch (error) {
            console.error('Failed to update user:', error);
        }
    };

    return (
        <Modal show={true} onHide={onClose}>
            <Modal.Header closeButton>
                <Modal.Title>Edit User</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form onSubmit={handleSubmit}>
                    <Form.Group controlId="formUserName">
                        <Form.Label>Username</Form.Label>
                        <Form.Control
                            type="text"
                            name="UserName"
                            value={newUserData.UserName}
                            onChange={handleChange}
                            required
                        />
                    </Form.Group>
                    <Form.Group controlId="formEmail">
                        <Form.Label>Email</Form.Label>
                        <Form.Control
                            type="email"
                            name="Email"
                            value={newUserData.Email}
                            onChange={handleChange}
                            required
                        />
                    </Form.Group>
                    <Form.Group controlId="formPassword">
                        <Form.Label>Password</Form.Label>
                        <Form.Control
                            type="password"
                            name="Password"
                            value={newUserData.Password}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group controlId="formWalletAddress">
                        <Form.Label>Wallet Address</Form.Label>
                        <Form.Control
                            type="text"
                            name="WalletAddress"
                            value={newUserData.WalletAddress}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Button variant="primary" type="submit">Save Changes</Button>
                </Form>
            </Modal.Body>
        </Modal>
    );
}