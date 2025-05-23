import { useState, useEffect } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import { getAllCategories } from '../api/categories';

const DEFAULT_STATE = {
    productName: '',
    description: '',
    price: '',
    calories: '',
    protein: '',
    fat: '',
    carbs: '',
    categoryId: '',
    restrictions: [], // масив
    isVegan: false,
};

const ALL_RESTRICTIONS = [
  "gluten",        
  "lactose",       
  "fructose",      
  "grains",        
  "seafood",   
  "eggs",          
  "fish",          
  "peanuts",       
  "soy",           
  "dairy",         
  "nuts",     
  "celery",        
  "mustard",       
  "sesame",        
  "lupin"         
];

const CreateProductModal = ({ show, handleClose, handleCreateProduct }) => {
    const [newProductData, setNewProductData] = useState(DEFAULT_STATE);
    const [categories, setCategories] = useState([]);

    // Підтягуємо категорії
    useEffect(() => {
        async function fetchCategories() {
            try {
                const response = await getAllCategories();
                setCategories(response.data);
            } catch (error) {
                console.error('Failed to fetch categories:', error);
            }
        }
        fetchCategories();
    }, []);

    const handleInputChange = (event) => {
        const { name, value, type, checked } = event.target;
        if (type === "checkbox" && name === "isVegan") {
            setNewProductData(prev => ({ ...prev, isVegan: checked }));
        } else if (type === "checkbox" && name.startsWith("restrictions-")) {
            // Перемикач алергенів
            const restriction = name.split("-")[1];
            setNewProductData(prev => ({
                ...prev,
                restrictions: checked
                    ? [...prev.restrictions, restriction]
                    : prev.restrictions.filter(r => r !== restriction)
            }));
        } else {
            setNewProductData(prev => ({
                ...prev,
                [name]: value
            }));
        }
    };

    const handleSubmit = () => {
        // Перетворення до правильних типів для бекенду
        const payload = {
            ...newProductData,
            price: parseFloat(newProductData.price),
            calories: parseFloat(newProductData.calories),
            protein: parseFloat(newProductData.protein),
            fat: parseFloat(newProductData.fat),
            carbs: parseFloat(newProductData.carbs),
            categoryId: newProductData.categoryId,
            restrictions: newProductData.restrictions,
            isVegan: !!newProductData.isVegan
        };
        handleCreateProduct(payload);
        setNewProductData(DEFAULT_STATE);
        handleClose();
    };

    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Create New Product</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group>
                        <Form.Label>Category</Form.Label>
                        <Form.Select
                            name="categoryId"
                            value={newProductData.categoryId}
                            onChange={handleInputChange}
                        >
                            <option value="">Select Category</option>
                            {categories.map((category) => (
                                <option key={category.id || category.categoryID} value={category.id || category.categoryID}>
                                    {category.categoryName}
                                </option>
                            ))}
                        </Form.Select>
                    </Form.Group>

                    <Form.Group>
                        <Form.Label>Product Name</Form.Label>
                        <Form.Control
                            type="text"
                            name="productName"
                            value={newProductData.productName}
                            onChange={handleInputChange}
                            placeholder="Enter product name"
                        />
                    </Form.Group>

                    <Form.Group>
                        <Form.Label>Description</Form.Label>
                        <Form.Control
                            type="text"
                            name="description"
                            value={newProductData.description}
                            onChange={handleInputChange}
                            placeholder="Enter product description"
                        />
                    </Form.Group>

                    <Form.Group>
                        <Form.Label>Price</Form.Label>
                        <Form.Control
                            type="number"
                            name="price"
                            value={newProductData.price}
                            onChange={handleInputChange}
                            placeholder="Enter price per 100g"
                        />
                    </Form.Group>

                    <Form.Group>
                        <Form.Label>Calories</Form.Label>
                        <Form.Control
                            type="number"
                            name="calories"
                            value={newProductData.calories}
                            onChange={handleInputChange}
                            placeholder="kcal per 100g"
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Protein (g)</Form.Label>
                        <Form.Control
                            type="number"
                            name="protein"
                            value={newProductData.protein}
                            onChange={handleInputChange}
                            placeholder="Protein per 100g"
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Fat (g)</Form.Label>
                        <Form.Control
                            type="number"
                            name="fat"
                            value={newProductData.fat}
                            onChange={handleInputChange}
                            placeholder="Fat per 100g"
                        />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label>Carbs (g)</Form.Label>
                        <Form.Control
                            type="number"
                            name="carbs"
                            value={newProductData.carbs}
                            onChange={handleInputChange}
                            placeholder="Carbs per 100g"
                        />
                    </Form.Group>

                    <Form.Group>
                        <Form.Label>Restrictions (Allergens)</Form.Label>
                        <div>
                            {ALL_RESTRICTIONS.map(r => (
                                <Form.Check
                                    key={r}
                                    type="checkbox"
                                    label={r}
                                    name={`restrictions-${r}`}
                                    checked={newProductData.restrictions.includes(r)}
                                    onChange={handleInputChange}
                                />
                            ))}
                        </div>
                    </Form.Group>

                    <Form.Group>
                        <Form.Check
                            type="checkbox"
                            name="isVegan"
                            label="Is Vegan"
                            checked={newProductData.isVegan}
                            onChange={handleInputChange}
                        />
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Cancel
                </Button>
                <Button variant="primary" onClick={handleSubmit}>
                    Create Product
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default CreateProductModal;
