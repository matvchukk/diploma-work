import { useNavigate } from 'react-router-dom';
import { Modal, ListGroup, Button } from 'react-bootstrap';
import { useState } from 'react';
import RationModal from './RationModal';
import { getDailyMenu } from '../api/recommendations';

import { updateCartQuantity, removeFromCart } from '../api/cart';

function CartModal({ items, show, onClose, refreshCart }) {
  const navigate = useNavigate();
  const [dailyMenu, setDailyMenu] = useState(null); // Об'єкт або null
  const [showRation, setShowRation] = useState(false);

  const fetchDailyMenuData = async () => {
     onClose();
    try {
      const response = await getDailyMenu();
      setDailyMenu(response.data);
      setShowRation(true);
    } catch (error) {
      alert('Не вдалося отримати раціон на день');
      console.log(error);
    }
  };

  async function increaseQuantity(productId, currentQuantity) {
    try {
      await updateCartQuantity({ ProductId: productId, Quantity: currentQuantity + 1 });
      if (refreshCart) refreshCart();
    } catch {
      alert('Не вдалося збільшити кількість товару');
    }
  }

  async function decreaseQuantity(productId, currentQuantity) {
    if (currentQuantity <= 1) return;
    try {
      await updateCartQuantity({ ProductId: productId, Quantity: currentQuantity - 1 });
      if (refreshCart) refreshCart();
    } catch {
      alert('Не вдалося зменшити кількість товару');
    }
  }

  async function handleRemoveFromCart(productId) {
    try {
      await removeFromCart(productId);
      if (refreshCart) refreshCart();
    } catch {
      alert('Не вдалося видалити товар з кошика');
    }
  }

  const handleOrder = () => {
    onClose();
    navigate(`/MakeOrder`);
  };

  const cartTotal = items.reduce((sum, item) => sum + item.total, 0);

  return (
    <>
      <Modal show={show} onHide={onClose} centered>
        <Modal.Header closeButton>
          <Modal.Title>Cart</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {items.length > 0 ? (
            <ListGroup>
              {items.map((item) => (
                <ListGroup.Item
                  key={item.productId}
                  className="d-flex align-items-center position-relative"
                >
                  <div className="flex-grow-1">
                    <h6>{item.productName}</h6>
                    <p>Price: {item.price}</p>
                    <div className="d-flex align-items-center">
                      <Button
                        variant="outline-secondary"
                        size="sm"
                        onClick={() => decreaseQuantity(item.productId, item.quantity)}
                        disabled={item.quantity <= 1}
                      >
                        -
                      </Button>
                      <span className="mx-2">{item.quantity}</span>
                      <Button
                        variant="outline-secondary"
                        size="sm"
                        onClick={() => increaseQuantity(item.productId, item.quantity)}
                      >
                        +
                      </Button>
                    </div>
                  </div>
                  <div>
                    <p>Sum: {item.total.toFixed(2)} </p>
                  </div>
                  <Button
                    variant="dark"
                    className="position-absolute top-0 end-0 p-1"
                    onClick={() => handleRemoveFromCart(item.productId)}
                  >
                    &times;
                  </Button>
                </ListGroup.Item>
              ))}
              <ListGroup.Item className="d-flex justify-content-between align-items-center mt-3">
                <h5>Total: {cartTotal.toFixed(2)}</h5>
                <Button variant="success" onClick={handleOrder}>
                  Make order
                </Button>
              </ListGroup.Item>
              {/* Ось тут кнопка */}
              <ListGroup.Item className="mt-2 text-center">
                <Button
                  variant="info"
                  onClick={fetchDailyMenuData}
                >
                  Отримати раціон на день
                </Button>
              </ListGroup.Item>
            </ListGroup>
          ) : (
            <div className="d-flex flex-column align-items-center">
              <p>Cart is empty</p>
              <Button
                variant="info"
                onClick={fetchDailyMenuData}
              >
                Отримати раціон на день
              </Button>
            </div>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={onClose}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>

      {/* RationModal (вікно рекомендацій) */}
      <RationModal
        show={showRation}
        onClose={() => setShowRation(false)}
        ration={dailyMenu}
      />
    </>
  );
}

export default CartModal;

