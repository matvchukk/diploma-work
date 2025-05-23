import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Container, Row, Col, Card, ListGroup, Button } from 'react-bootstrap';
import { useAuth } from '../Contexts/AuthProvider';
import OrderCard from '../Order/OrderCard';
import EditUser from './EditUser';
import { getUserProfile, deleteUser } from '../api/users';
import { getOrders } from '../api/orders';

export default function UserProfile() {
    const { userId, logout } = useAuth();
    const [userData, setUserData] = useState(null);
    const [isEditing, setIsEditing] = useState(false);
    const [orders, setOrders] = useState(false);
    const navigate = useNavigate();
    const { userID } = useParams();

    async function fetchUserData() {
        try {
            const response = await getUserProfile();
            setUserData(response.data);
        } catch (error) {
            console.error('Failed to fetch user data:', error);
        }
    }

    async function fetchOrdersData() {
        try {
            const response = await getOrders();
            setOrders(response.data);
            console.log(response.data);
        } catch (error) {
            console.error('Failed to fetch user data:', error);
        }
    }

    useEffect(() => {
        if (userID) {
            fetchUserData();
            fetchOrdersData();
        }
    // eslint-disable-next-line
    }, []);

    if (!userData) {
        return <p>Loading...</p>;
    }

    const handleViewAllReviews = () => {
        navigate('/ReviewsPage');
    };

    const handleEditUser = () => {
        setIsEditing(true);
    };

    const handleDeleteUser = async () => {
        try {
            await deleteUser(userID);
            logout();
            navigate('/');
        } catch (error) {
            console.error('Failed to delete user:', error);
        }
    };

    const handleViewWishListsDetails = (wishListId) => {
        navigate(`/WishLists/${wishListId}`);
    };

    const handleEditUserClose = () => {
        setIsEditing(false);
        fetchUserData();
    };

    return (
        <Container className="mt-5" style={{ minHeight: '100vh' }}>
            <Row>
                <Col md={4}>
                    <Card>
                        <Card.Body>
                            <Card.Title>Profile Information</Card.Title>
                            <ListGroup variant="flush">
                                <ListGroup.Item>
                                    <strong>Name:</strong> {userData.name}
                                </ListGroup.Item>
                                <ListGroup.Item>
                                    <strong>Email:</strong> {userData.email}
                                </ListGroup.Item>
                                <ListGroup.Item>
                                    <strong>Age:</strong> {userData.age}
                                </ListGroup.Item>
                                <ListGroup.Item>
                                    <strong>Sex:</strong> {userData.sex}
                                </ListGroup.Item>
                                <ListGroup.Item>
                                    <strong>Goal:</strong> {userData.goal}
                                </ListGroup.Item>
                                <ListGroup.Item>
                                    <strong>Budget/week:</strong> {userData.budgetPerWeek}
                                </ListGroup.Item>
                                <ListGroup.Item>
                                    <strong>Is Vegan:</strong> {userData.isVegan ? "Yes" : "No"}
                                </ListGroup.Item>
                                <ListGroup.Item>
                                    <strong>Restrictions:</strong> {userData.restrictions?.join(", ")}
                                </ListGroup.Item>
                            </ListGroup>
                            {userId === userID && (
                                <div className="mt-3">
                                    <Button variant="warning" onClick={handleEditUser}>Edit User</Button>
                                    <Button variant="danger" onClick={handleDeleteUser} className="ms-2">Delete User</Button>
                                </div>
                            )}
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={8}>
                    {/* Orders */}
                    <Card className="mb-4">
                        <Card.Body>
                            <Card.Title>Orders</Card.Title>
                            {orders && orders.length > 0 ? (
                                <ListGroup variant="flush">
                                    {userData.orders.map(order => (
                                        <OrderCard key={order.id} order={order} />
                                    ))}
                                </ListGroup>
                            ) : (
                                <p>No orders found.</p>
                            )}
                        </Card.Body>
                    </Card>

                    {/* Reviews */}
                    <Card className="mb-4">
                        <Card.Body>
                            <Card.Title>Reviews</Card.Title>
                            {userData.reviews && userData.reviews.length > 0 ? (
                                <Button variant="primary" onClick={handleViewAllReviews}>
                                    View All Reviews
                                </Button>
                            ) : (
                                <p>No reviews found.</p>
                            )}
                        </Card.Body>
                    </Card>

                    {/* WishLists */}
                    <Card className="mb-4">
                        <Card.Body>
                            <Card.Title>Wish Lists</Card.Title>
                            {userData.wishLists && userData.wishLists.length > 0 ? (
                                <ListGroup variant="flush">
                                    {userData.wishLists.map((wishList) => (
                                        <ListGroup.Item key={wishList.id}>
                                            <span
                                                style={{ cursor: 'pointer', color: 'black', textDecoration: 'none' }}
                                                onClick={() => handleViewWishListsDetails(wishList.id)}
                                            >
                                                <strong>{wishList.name}</strong>
                                            </span>
                                            <div>
                                                <strong>Products:</strong> {wishList.products.length}
                                            </div>
                                        </ListGroup.Item>
                                    ))}
                                </ListGroup>
                            ) : (
                                <p>No wish lists found.</p>
                            )}
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            {/* Edit User Modal */}
            {isEditing && (
                <EditUser userData={userData} onClose={handleEditUserClose} />
            )}
        </Container>
    );
}
