import { useEffect, useState } from 'react';
import { getRecommendations } from '../api/recommendations';
import { Card, Button, Spinner } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';

export default function RecommendationBar({ limit = 10 }) {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchRecommendations = async () => {
      setLoading(true);
      try {
        const response = await getRecommendations({ params: { limit } });
        setProducts(response.data);
      } catch (e) {
        console.log(e);
        setProducts([]);
      } finally {
        setLoading(false);
      }
    };
    fetchRecommendations();
  }, [limit]);

  if (loading) {
    return (
      <div className="d-flex justify-content-center py-5">
        <Spinner animation="border" />
      </div>
    );
  }

  if (products.length === 0) {
    return (
      <div className="text-center py-4 text-muted">
        Немає рекомендацій для вас.
      </div>
    );
  }

  return (
    <div className="my-4">
      <h4 className="mb-3">Рекомендовані товари</h4>
      <div
        className="recommend-bar d-flex overflow-auto pb-2"
        style={{
          gap: "1rem",
          scrollSnapType: "x mandatory",
          WebkitOverflowScrolling: "touch",
        }}
      >
        {products.map(product => (
          <Card
            key={product.id}
            style={{
              minWidth: 220,
              maxWidth: 250,
              flex: '0 0 auto',
              scrollSnapAlign: "start"
            }}
            className="shadow-sm"
          >
            <Card.Body>
              <Card.Title
                style={{ fontSize: '1rem', fontWeight: 500, minHeight: 36 }}
              >
                {product.productName}
              </Card.Title>
              <Card.Subtitle className="mb-2 text-muted" style={{ fontSize: 12 }}>
                {product.categoryName}
              </Card.Subtitle>
              <Card.Text style={{ fontSize: 14, minHeight: 42 }}>
                {product.description?.length > 40
                  ? product.description.slice(0, 40) + '…'
                  : product.description}
              </Card.Text>
              <div className="mb-2">
                <span className="fw-bold">{product.price} UAH</span>
              </div>
              <div className="mb-2" style={{ fontSize: 13 }}>
                <span>⭐ {product.averageRating || 0}</span>
                {product.isVegan && <span className="ms-2 badge bg-success">Vegan</span>}
              </div>
              <Button
                variant="outline-primary"
                size="sm"
                onClick={() => navigate(`/Product/${product.id}`)}
              >
                Детальніше
              </Button>
            </Card.Body>
          </Card>
        ))}
      </div>
    </div>
  );
}
