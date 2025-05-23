import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Container, Row, Col, Card, ListGroup, Button, ListGroupItem } from 'react-bootstrap';
import { useAuth } from '../Contexts/AuthProvider';
import ReviewCard from '../Review/ReviewCard';
import CreateProductModal from '../Product/CreateProductModal';
import EditSeller from './EditSeller';
import { getSellerById, getSellerProfile, updateSeller } from '../api/sellers';
import { getReviews } from '../api/reviews';
import { createProduct, updateProduct, deleteProduct } from '../api/products';
import EditProductModal from '../Product/EditProductModal';
import { getAllCategories } from '../api/categories';

export default function SellerProfile() {
    const { userId } = useAuth();
    const { sellerId } = useParams();
    const [sellerData, setSellerData] = useState(null);
    const [showProductModal, setShowProductModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [selectedProduct, setSelectedProduct] = useState(null);
    const [reviews, setReviews] = useState([]);
    const [showEditSellerModal, setShowEditSellerModal] = useState(false);
    const [categories, setCategories] = useState([]);
    const navigate = useNavigate();
    
    // 1. Чи це мій профіль продавця?
    const isMyProfile = !sellerId || sellerId === userId;

    // 2. Оновлений fetch (обирає правильний API)
    async function fetchSellerData() {
        try {
            let response;
            if (isMyProfile) {
                response = await getSellerProfile();
            } else {
                response = await getSellerById(sellerId);
            }
            setSellerData(response.data);
            console.log(response);
        } catch (error) {
            console.error('Failed to fetch seller data:', error);
        }
    }

    async function fetchReviewsData() {
        try {
            // Продавець завжди відомий (або свій, або чужий)
            const id = isMyProfile ? userId : sellerId;
            const response = await getReviews({ sellerId: id });
            setReviews(response.data);
        } catch (error) {
            console.error('Failed to fetch reviews:', error);
        }
    }

    useEffect(() => {
        async function fetchCategories() {
            try {
                const response = await getAllCategories();
                setCategories(response.data); // або setCategories(response), якщо API повертає масив одразу
            } catch (error) {
                console.error('Failed to fetch categories:', error);
            }
        }
        fetchCategories();
    }, []);

    useEffect(() => {
        if (isMyProfile && userId) {
            fetchSellerData();
            fetchReviewsData();
        } else if (!isMyProfile && sellerId) {
            fetchSellerData();
            fetchReviewsData();
        }
    }, [sellerId, userId, isMyProfile]);

    const handleDeleteProduct = async (productId) => {
        try {
            await deleteProduct(productId);
            fetchSellerData();
        } catch (error) {
            console.error('Failed to delete product:', error);
        }
    };

    const handleEditProduct = (product) => {
        setSelectedProduct(product);
        setShowEditModal(true);
    };


    const handleShowProductModal = () => setShowProductModal(true);
    const handleCloseProductModal = () => setShowProductModal(false);

    const handleCreateProduct = async (newProductData) => {
        try {
            await createProduct({
                sellerID: isMyProfile ? userId : sellerId,
                ...newProductData,
            });
            fetchSellerData();
        } catch (error) {
            console.error('Failed to create product:', error);
        }
    };

    const handleViewDetails = (productId) => {
        navigate(`/Product/${productId}`);
    };

    const handleOpenEditSellerModal = () => setShowEditSellerModal(true);
    const handleCloseEditSellerModal = () => setShowEditSellerModal(false);

    async function handleUpdateSellerData(id, newSellerData){
        try {
            await updateSeller(id, newSellerData);
            fetchSellerData();
        } catch (error) {
            console.error('Failed to update seller data:', error);
        }
    }

    if (!sellerData) {
        return <p>Loading...</p>;
    }

    return (
        <Container className="mt-5" style={{ minHeight: '100vh' }}>
            <Row>
                <Col md={4}>
                    <Card>
                        <Card.Body>
                            <Card.Title>Store Information</Card.Title>
                            <ListGroup variant="flush">
                                {isMyProfile && (
                                    <>
                                        <Button variant="primary" onClick={handleOpenEditSellerModal}>
                                            Edit Store
                                        </Button>
                                        <EditSeller
                                            show={showEditSellerModal}
                                            handleClose={handleCloseEditSellerModal}
                                            sellerData={sellerData}
                                            updateSellerData={handleUpdateSellerData}
                                        />
                                    </>
                                )}
                                <ListGroup.Item><strong>Store Name:</strong> {sellerData.seller.storeName}</ListGroup.Item>
                                <ListGroup.Item><strong>Description:</strong> {sellerData.seller.storeDescription}</ListGroup.Item>
                                <ListGroup.Item><strong>Email:</strong> {sellerData.seller.contactAddress}</ListGroup.Item>
                                <ListGroup.Item><strong>Phone Number:</strong> {sellerData.seller.contactPhone}</ListGroup.Item>
                                <ListGroup.Item><strong>Rating:</strong> {sellerData.seller.rating} <span style={{ color: 'orange' }}>★</span></ListGroup.Item>
                            </ListGroup>
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={8}>
                    <Card className="mb-4">
                        <Card.Body>
                            <Card.Title>Products</Card.Title>
                            {isMyProfile && (
                                <Button variant="primary" className="mb-3" onClick={handleShowProductModal}>
                                    Add New Product
                                </Button>
                            )}
                            {sellerData.seller.products && sellerData.seller.products.length > 0 ? (
                                <ListGroup variant="flush">
                                    {sellerData.seller.products.map((product) => (
                                        <ListGroup.Item key={product.id}>
                                            <span onClick={() => handleViewDetails(product.id)} style={{ cursor: 'pointer', color: 'black', textDecoration: 'none' }}>
                                                <strong>Product Name:</strong> {product.productName} 
                                                <strong> Description:</strong> {product.description} 
                                                <strong> Price:</strong> {product.price}
                                            </span>
                                            {isMyProfile && (
                                                <>
                                                    <Button
                                                        variant="danger"
                                                        onClick={(e) => {
                                                            e.stopPropagation();
                                                            handleDeleteProduct(product.id);
                                                        }}
                                                        style={{ float: 'right', marginLeft: '10px' }}
                                                    >
                                                        Delete
                                                    </Button>
                                                    <Button
                                                        variant="secondary"
                                                        onClick={(e) => {
                                                            e.stopPropagation();
                                                            handleEditProduct(product);
                                                        }}
                                                        style={{ float: 'right' }}
                                                    >
                                                        Edit
                                                    </Button>
                                                </>
                                            )}
                                        </ListGroup.Item>
                                    ))}
                                </ListGroup>
                            ) : (
                                <p>No products found.</p>
                            )}
                        </Card.Body>
                    </Card>
                </Col>

                <ListGroup>
                    <ListGroupItem>
                        <ReviewCard reviews={reviews} />
                    </ListGroupItem>
                </ListGroup>
            </Row>

            <CreateProductModal
                show={showProductModal}
                handleClose={handleCloseProductModal}
                handleCreateProduct={handleCreateProduct}
            />

            <EditProductModal
                show={showEditModal}
                handleClose={() => setShowEditModal(false)}
                selectedProduct={selectedProduct}
                categories={categories}
                handleSaveChanges={async (formData) => {
                    console.log(formData); // Переконайся, що тут всі поля коректні
                    await updateProduct(formData);
                    setShowEditModal(false);
                    setSelectedProduct(null);
                    fetchSellerData();
                }}
            />
        </Container>
    );
}