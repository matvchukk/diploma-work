import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Card, Button, Container, Row, Col } from 'react-bootstrap';
import WishlistModal from '../Modals/WishListModal';
import { useAuth } from '../Contexts/AuthProvider';
import { getProducts } from '../api/products';
import { getUserWishlists, addProductToWishlist } from '../api/wishLists';

export default function Category() {
    const { userId, isAuthenticated } = useAuth();
    const [products, setProducts] = useState([]);
    const [wishlists, setWishlists] = useState([]);
    const [selectedProductId, setSelectedProductId] = useState(null);
    const [showWishlistModal, setShowWishlistModal] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const { categoryId } = useParams();
    const navigate = useNavigate();

    const handleViewDetails = (productId) => {
        navigate(`/Product/${productId}`);
    };

    const handleAddToWishlist = (productId) => {
        setSelectedProductId(productId);
        setShowWishlistModal(true);
    };

    const handleSelectWishlist = async (wishlistId) => {
        try {
            await addProductToWishlist(wishlistId, selectedProductId);
            alert('Product added to wishlist!');
        } catch (error) {
            alert('Failed to add product to wishlist.' + error);
        }
        setShowWishlistModal(false);
    };

    useEffect(() => {
        const fetchProductsByCategory = async () => {
            if (!categoryId) {
                setErrorMessage("Category ID is missing.");
                return;
            }
            try {
                const response = await getProducts({ CategoryId: categoryId });
                setProducts(response.data); // <-- це масив продуктів!
                console.log(response.data);
            } catch (error) {
                setErrorMessage('An error occurred while fetching category data.' + error);
            }
        };

        const fetchWishListsData = async () => {
            if (isAuthenticated && userId) {
                try {
                    const resposnse = await getUserWishlists();
                    setWishlists(resposnse.data || []);
                    console.log(resposnse.data);
                } catch (error) {
                    setErrorMessage('An error occurred while fetching wishlist data.' + error);
                }
            }
        };

        fetchProductsByCategory();
        fetchWishListsData();
    }, [categoryId, isAuthenticated, userId]);

    if (errorMessage) return <p>{errorMessage}</p>;
    if (!products) return <p>Loading...</p>;

    return (
        <Container style={{ minHeight: '100vh' }}>
            <Row>
                {products.length > 0 ? (
                    products.map((product) => (
                        <Col key={product.id} md={4} className="mb-4">
                            <Card>
                                <Card.Body>
                                    <Card.Title>{product.productName}</Card.Title>
                                    <Card.Text>{product.description ?? ''}</Card.Text>
                                    <Card.Text>Price: {product.price}</Card.Text>
                                    <Button
                                        variant="primary"
                                        onClick={() => handleViewDetails(product.id)}
                                    >
                                        Details
                                    </Button>{' '}
                                    <Button
                                        variant="secondary"
                                        onClick={() => handleAddToWishlist(product.id)}
                                    >
                                        To wish list
                                    </Button>
                                </Card.Body>
                            </Card>
                        </Col>
                    ))
                ) : (
                    <p>No products available.</p>
                )}
            </Row>
            <WishlistModal
                show={showWishlistModal}
                onHide={() => setShowWishlistModal(false)}
                wishlists={wishlists}
                handleSelectWishlist={handleSelectWishlist}
            />
        </Container>
    );
}
