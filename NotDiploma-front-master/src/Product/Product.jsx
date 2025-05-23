import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Container, Row, Col, Card, Button, Alert } from 'react-bootstrap';
import WishlistModal from '../Modals/WishListModal';
import ReviewCard from '../Review/ReviewCard';
import { useAuth } from '../Contexts/AuthProvider';
import { getProductById } from '../api/products';
import { addToCart } from '../api/cart';
import { getUserWishlists, addProductToWishlist } from '../api/wishLists';
import { getReviews } from '../api/reviews';

export default function Product() {
    const { isAuthenticated } = useAuth();
    const [product, setProduct] = useState(null);
    const [wishlists, setWishlists] = useState([]);
    const [selectedProductId, setSelectedProductId] = useState(null);
    const [showWishlistModal, setShowWishlistModal] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const { productId } = useParams();
    const [reviews, setReviews] = useState([]);

    // Відкрити модалку вибору списку бажань
    const handleAddToWishlist = (productId) => {
        setSelectedProductId(productId);
        setShowWishlistModal(true);
    };

    // Додати товар у вибраний список бажань
    const handleSelectWishlist = async (wishListId) => {
        try {
            await addProductToWishlist(wishListId, selectedProductId);
            setShowWishlistModal(false);
        } catch (error) {
            setErrorMessage('Error adding product to wishlist.' + error);
        }
    };

    // Додати у кошик
    const handleAddToShoppingCart = async (productId) => {
        try {
            await addToCart({ productId: productId, quantity: 1 }); // API приймає об'єкт { productId }
            // Можна додати інформування про успіх
        } catch (error) {
            setErrorMessage('Error adding product to cart.' + error);
        }
    };

    useEffect(() => {
        // Завантаження даних продукту
        const fetchProductData = async () => {
            try {
                const response = await getProductById(productId);
                setProduct(response.data);
            } catch (error) {
                setErrorMessage('An error occurred while fetching product data.' + error);
            }
        };

        // Завантаження списків бажань
        const fetchWishListsData = async () => {
            try {
                const { data } = await getUserWishlists();
                setWishlists(data);
            } catch (error) {
                setErrorMessage('An error occurred while fetching wishlist data.' + error);
            }
        };

        // Завантаження відгуків для продукту
        const fetchReviewsData = async () => {
            try {
                // ВАЖЛИВО: Передавати параметри як об'єкт!
                const { data } = await getReviews({ productId });
                setReviews(data);
            } catch (error) {
                setErrorMessage('An error occurred while fetching reviews.' + error);
            }
        };

        fetchProductData();
        if (isAuthenticated) fetchWishListsData();
        fetchReviewsData();
    }, [productId, isAuthenticated]);

    return (
        <Container style={{ minHeight: '100vh' }}>
            {errorMessage && <Alert variant="danger">{errorMessage}</Alert>}
            {product ? (
                <Row className="justify-content-center mt-4">
                    <Col md={8}>
                        <Card>
                            {product.imageUrl && (
                                <Card.Img variant="top" src={product.imageUrl} alt={product.productName} />
                            )}
                            <Card.Body>
                                <Card.Title>{product.productName}</Card.Title>
                                {product.description && (
                                    <Card.Text>
                                        <strong>Description:</strong> {product.description}
                                    </Card.Text>
                                )}
                                <Card.Text>
                                    <strong>Price:</strong> {product.price}
                                </Card.Text>
                                <Card.Text>
                                    <strong>Calories:</strong> {product.calories} kcal
                                </Card.Text>
                                <Card.Text>
                                    <strong>Protein:</strong> {product.protein} г
                                </Card.Text>
                                <Card.Text>
                                    <strong>Fat:</strong> {product.fat} г
                                </Card.Text>
                                <Card.Text>
                                    <strong>Carbs:</strong> {product.carbs} г
                                </Card.Text>
                                <Card.Text>
                                    <strong>Vegan:</strong> {product.isVegan ? 'Yes' : 'No'}
                                </Card.Text>
                                <Card.Text>
                                    <strong>Restrictions:</strong> {Array.isArray(product.restrictions) 
                                        ? product.restrictions.join(', ') 
                                        : (product.restrictions || 'None')}
                                </Card.Text>
                                <Card.Text>
                                    <strong>Category:</strong> {product.categoryName || 'Unknown'}
                                </Card.Text>
                                <Card.Text>
                                    <strong>Seller:</strong> {product.sellerName || 'Unknown'}
                                </Card.Text>
                                <Card.Text>
                                    <strong>Created At:</strong> {new Date(product.createdAt).toLocaleString()}
                                </Card.Text>
                                <Card.Text>
                                    <strong>Average Rating:</strong> {product.averageRating}
                                </Card.Text>
                                {isAuthenticated && (
                                    <>
                                        <Button
                                            variant="primary"
                                            onClick={() => handleAddToShoppingCart(productId)}
                                        >
                                            Add to Cart
                                        </Button>{' '}
                                        <Button
                                            variant="secondary"
                                            onClick={() => handleAddToWishlist(productId)}
                                        >
                                            Add to Wishlist
                                        </Button>
                                    </>
                                )}
                            </Card.Body>
                        </Card>
                    </Col>
                    <ReviewCard reviews={reviews} />
                </Row>
            ) : (
                <Row className="justify-content-center mt-4">
                    <Col md={8}>
                        <Alert variant="info">Loading product details...</Alert>
                    </Col>
                </Row>
            )}

            {/* WishList Modal */}
            <WishlistModal
                show={showWishlistModal}
                onHide={() => setShowWishlistModal(false)}
                wishlists={wishlists}
                handleSelectWishlist={handleSelectWishlist}
            />
        </Container>
    );
}