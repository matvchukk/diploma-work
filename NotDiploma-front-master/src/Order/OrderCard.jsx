import { useState } from 'react';
import { Card, Accordion, Button, ListGroup } from 'react-bootstrap';
import MakeReviewModal from '../Review/MakeReviewModal';
import { updateOrderStatus } from '../api/orders';

const OrderCard = ({ order, onRefresh }) => {
    const [showReviewModal, setShowReviewModal] = useState(false);
    const [selectedProductId, setSelectedProductId] = useState(null);

    const openReviewModal = (productId) => {
        setSelectedProductId(productId);
        setShowReviewModal(true);
    };

    const handleUpdateStatus = async () => {
        try {
            await updateOrderStatus({ orderId: order.id, newStatus: "Delivered" });
            alert('Order status updated!');
            if (onRefresh) onRefresh();
        } catch (e) {
            alert('Failed to update order status' + e);
        }
    };

    return (
        <Card className="mb-4">
            <Card.Body>
                <Accordion>
                    <Accordion.Item eventKey="0">
                        <Accordion.Header>
                            <div className="d-flex justify-content-between w-100">
                                <div>
                                    <strong>Order ID:</strong> {order.id}
                                </div>
                                <div>
                                    <strong>Status:</strong> {order.status}
                                </div>
                            </div>
                        </Accordion.Header>
                        <Accordion.Body>
                            <div className="d-flex flex-column">
                                <h5>Order Items</h5>
                                <ListGroup variant="flush">
                                    {order.items && order.items.length > 0 ? (
                                        order.items.map(item => (
                                            <ListGroup.Item key={item.productId} className="d-flex justify-content-between align-items-center">
                                                <div>
                                                    <strong>{item.productName}</strong> <br />
                                                    Quantity: {item.quantity} <br />
                                                    Unit price: {item.unitPrice}
                                                </div>
                                                <div className="d-flex align-items-center">
                                                    <span className="me-3">Total: {item.totalPrice} </span>
                                                    <Button
                                                        variant="primary"
                                                        onClick={() => openReviewModal(item.productId)}
                                                    >
                                                        Make Review
                                                    </Button>
                                                </div>
                                            </ListGroup.Item>
                                        ))
                                    ) : (
                                        <p>No items found.</p>
                                    )}
                                </ListGroup>

                                <div className="mt-3">
                                    <h5>Total Price: {order.totalPrice}</h5>
                                </div>

                                <h6 className="mt-4">Shipping Address</h6>
                                <p>{order.shippingAddress}</p>

                                <Button className="mt-3" onClick={handleUpdateStatus}>
                                    Mark as Delivered
                                </Button>
                            </div>
                        </Accordion.Body>
                    </Accordion.Item>
                </Accordion>
            </Card.Body>
            <MakeReviewModal
                show={showReviewModal}
                handleClose={() => setShowReviewModal(false)}
                productId={selectedProductId}
            />
        </Card>
    );
};

export default OrderCard;

