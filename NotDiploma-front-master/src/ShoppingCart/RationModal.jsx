import { Modal, Button, Badge, Row, Col, Alert, Spinner } from 'react-bootstrap';
import { useState } from 'react';
import { addToCartBatch } from '../api/cart';

const NUTRIENT_COLOR = {
  'Calories': 'warning',
  'Protein': 'success',
  'Fat': 'danger',
  'Carbs': 'info',
};

function NutrientChip({ label, value }) {
  return (
    <span style={{ fontSize: 13, marginRight: 8 }}>
      <Badge bg={NUTRIENT_COLOR[label]}>{label}</Badge> <span>{value}</span>
    </span>
  );
}

function MealCard({ meal, items, mealKey, onRecipeClick, recipe, recipeLoading, onRecipeClose  }) {
  return (
    <div className="mb-4">
      <h5 className="fw-semibold mb-3">{meal}</h5>
      <Row xs={1} md={2} className="g-3">
        {items.map(({ product, grams }) => (
          <Col key={product.id}>
            <div className="ration-card shadow-sm px-3 py-2 h-100 d-flex flex-column justify-content-between">
              <div>
                <div className="d-flex justify-content-between align-items-start mb-1">
                  <span className="fw-bold">{product.productName}</span>
                  <span className="ration-grams">{grams} g</span>
                </div>
                <div className="mb-1" style={{ fontSize: 13, color: "#888" }}>
                  {product.categoryName || ''}
                </div>
                <div style={{ fontSize: 13, color: "#555" }}>
                  {product.description?.length > 40
                    ? product.description.slice(0, 40) + '…'
                    : product.description}
                </div>
              </div>
              <div className="d-flex flex-wrap mt-2 gap-2">
                <NutrientChip label="Calories" value={product.calories} />
                <NutrientChip label="Protein" value={product.protein} />
                <NutrientChip label="Fat" value={product.fat} />
                <NutrientChip label="Carbs" value={product.carbs} />
              </div>
            </div>
          </Col>
        ))}
      </Row>
      <div className="mt-2">
        <Button
          variant="outline-primary"
          size="sm"
          onClick={() => onRecipeClick(mealKey, meal, items)}
          disabled={recipeLoading}
        >
          {recipeLoading ? (
            <>
              <Spinner size="sm" animation="border" /> ...
            </>
          ) : (
            "Recipe"
          )}
        </Button>
        {recipe && (
          <Alert
            variant="info"
            className="mt-2 d-flex align-items-start"
            style={{ whiteSpace: "pre-line", fontSize: 15, position: "relative" }}
            dismissible
            onClose={() => onRecipeClose(mealKey)}
          >
            <div style={{ flex: 1 }}>{recipe}</div>
          </Alert>
        )}
      </div>
    </div>
  );
}

export default function RationModal({ show, onClose, ration }) {
  const [isLoading, setIsLoading] = useState(false);
  const [success, setSuccess] = useState('');
  const [error, setError] = useState('');
  const [recipes, setRecipes] = useState({});
  const [recipeLoading, setRecipeLoading] = useState({});
  
  const OPENAI_API_KEY = import.meta.env.VITE_OpenAi_key;

  const handleRecipeClose = (mealKey) => {
    setRecipes(prev => ({ ...prev, [mealKey]: null }));
  };
  
  const handleGenerateRecipe = async (mealKey, mealName, items) => {
    setRecipeLoading(prev => ({ ...prev, [mealKey]: true }));
    setRecipes(prev => ({ ...prev, [mealKey]: "" }));

    const productsList = items
      .map(({ product, grams }) => `${product?.productName || ""} (${grams} g)`)
      .join(", ");

    const prompt = `Generate a recipe for one meal (${mealName}) from the following ingredients: ${productsList}. Briefly tell me what to cook from this and how.`;

    try {
      const res = await fetch("https://api.openai.com/v1/chat/completions", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${OPENAI_API_KEY}`,
        },
        body: JSON.stringify({
          model: "gpt-3.5-turbo",
          messages: [
            { role: "system", content: "Ти експерт з приготування їжі." },
            { role: "user", content: prompt }
          ],
          max_tokens: 1000,
          temperature: 0.7,
        }),
      });

      const data = await res.json();
      const recipe =
        data?.choices?.[0]?.message?.content?.trim() ||
        "Failed to generate recipe.";
      setRecipes(prev => ({ ...prev, [mealKey]: recipe }));
    } catch (e) {
      console.log(e);
      setRecipes(prev => ({ ...prev, [mealKey]: "Recipe generation error." }));
    } finally {
      setRecipeLoading(prev => ({ ...prev, [mealKey]: false }));
    }
  };

  if (!ration) return null;

  const handleAddToCart = async () => {
    setIsLoading(true);
    setError('');
    setSuccess('');
    try {
      const mealTypes = ['breakfast', 'lunch', 'dinner'];
      const products = [];
      mealTypes.forEach(meal => {
        if (ration[meal] && Array.isArray(ration[meal])) {
          ration[meal].forEach(({ product, grams }) => {
            if (product && grams > 0) {
              products.push({
                productId: product.id,
                quantity: grams,
              });
            }
          });
        }
      });

      if (products.length === 0) {
        setError('There are no items to add to cart.');
        setIsLoading(false);
        return;
      }

      await addToCartBatch(products);
      setSuccess('Products successfully added to cart!');
      setTimeout(() => {
        setSuccess('');
        onClose();
      }, 1000);
    } catch (e) {
      setError(
        e.response?.data?.error ||
        'Unable to add items to cart. Please try again.'
      );
    } finally {
      setIsLoading(false);
    }
  };

  const summaryBlock = (
    <div className="ration-summary mt-4">
      <div className="fw-semibold mb-2">Total:</div>
      <div className="d-flex flex-wrap gap-3">
        <NutrientChip label="Calories" value={ration.totalCalories?.toFixed(2)} />
        <NutrientChip label="Protein" value={ration.totalProtein?.toFixed(2)} />
        <NutrientChip label="Fat" value={ration.totalFat?.toFixed(2)} />
        <NutrientChip label="Carbs" value={ration.totalCarbs?.toFixed(2)} />
        <span style={{ fontWeight: 500, fontSize: 15 }}>
          Price: {ration.totalPrice?.toFixed(2)} UAH
        </span>
      </div>
    </div>
  );

  return (
    <Modal show={show} onHide={onClose} centered size="lg">
      <Modal.Header closeButton>
        <Modal.Title>Recommended daily diet</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <div className="text-muted mb-3" style={{ fontSize: 13 }}>
          *nutritional values are indicated per 100g of product.
        </div>
        {['breakfast', 'lunch', 'dinner'].map(meal =>
          ration[meal] && ration[meal].length > 0
            ? (
              <MealCard
                meal={meal.charAt(0).toUpperCase() + meal.slice(1)}
                items={ration[meal]}
                mealKey={meal}
                onRecipeClick={handleGenerateRecipe}
                recipe={recipes[meal]}
                recipeLoading={!!recipeLoading[meal]}
                onRecipeClose={handleRecipeClose}
                key={meal}
              />
            )
            : null
        )}
        <hr />
        {summaryBlock}
        {error && <Alert variant="danger" className="mt-3">{error}</Alert>}
        {success && <Alert variant="success" className="mt-3">{success}</Alert>}
      </Modal.Body>
      <Modal.Footer>
        <Button
          variant="success"
          onClick={handleAddToCart}
          disabled={isLoading}
        >
          {isLoading ? <Spinner size="sm" animation="border" /> : "Add these items to cart"}
        </Button>
        <Button className="btn btn-secondary" onClick={onClose} disabled={isLoading}>
          Exit
        </Button>
      </Modal.Footer>
      <style>
        {`
        .ration-card {
          background: #f7fafc;
          border-radius: 12px;
          border: 1px solid #e5e7eb;
          transition: box-shadow 0.15s;
        }
        .ration-card:hover {
          box-shadow: 0 4px 20px rgba(0,0,0,0.09);
        }
        .ration-grams {
          background: #e0f7fa;
          padding: 2px 8px;
          border-radius: 8px;
          font-size: 13px;
          color: #1a454c;
          font-weight: 500;
        }
        .ration-summary {
          background: #f3f8fb;
          border-radius: 10px;
          padding: 12px 18px;
        }
        `}
      </style>
    </Modal>
  );
}

