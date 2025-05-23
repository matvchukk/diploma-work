import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { makeOrder } from '../api/orders';
import { Form, Button, Alert, Container, Row, Col, Spinner } from 'react-bootstrap';

function MakeOrder() {
  const [shippingAddress, setShippingAddress] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setIsLoading(true);

    try {
      await makeOrder({ shippingAddress }); // { shippingAddress } - має збігатися з CreateOrderRequest
      setSuccess('Замовлення успішно створено!');
      setShippingAddress('');
      setTimeout(() => {
        navigate('/'); // перенаправлення на головну чи сторінку "замовлення"
      }, 1500);
    } catch (err) {
      const message =
        err.response?.data?.error ||
        'Сталася помилка при створенні замовлення. Спробуйте ще раз.';
      setError(message);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container className="mt-4" style={{minHeight: 657}}>
      <Row className="justify-content-center">
        <Col xs={12} md={8} lg={6}>
          <h3 className="mb-4">Оформлення замовлення</h3>
          {error && <Alert variant="danger">{error}</Alert>}
          {success && <Alert variant="success">{success}</Alert>}
          <Form onSubmit={handleSubmit}>
            <Form.Group controlId="shippingAddress" className="mb-3">
              <Form.Label>Адреса доставки</Form.Label>
              <Form.Control
                type="text"
                placeholder="Введіть адресу доставки"
                value={shippingAddress}
                onChange={(e) => setShippingAddress(e.target.value)}
                required
                disabled={isLoading}
              />
            </Form.Group>
            <Button variant="success" type="submit" disabled={isLoading || !shippingAddress.trim()}>
              {isLoading ? <Spinner size="sm" animation="border" /> : 'Підтвердити замовлення'}
            </Button>
          </Form>
        </Col>
      </Row>
    </Container>
  );
}

export default MakeOrder;

