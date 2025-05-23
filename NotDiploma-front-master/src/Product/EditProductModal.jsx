import { useState, useEffect } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';

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

export default function EditProductModal({
    show,
    handleClose,
    selectedProduct,
    categories,
    handleSaveChanges
}) {
    const [formData, setFormData] = useState({
        ProductId: '',
        ProductName: '',
        Description: '',
        Price: 0,
        Calories: 0,
        Protein: 0,
        Fat: 0,
        Carbs: 0,
        CategoryId: '',
        Restrictions: [],
        IsVegan: false,
    });

    useEffect(() => {
        if (selectedProduct) {
            setFormData({
                ProductId: selectedProduct.id || '',
                ProductName: selectedProduct.productName || '',
                Description: selectedProduct.description || '',
                Price: selectedProduct.price || 0,
                Calories: selectedProduct.calories || 0,
                Protein: selectedProduct.protein || 0,
                Fat: selectedProduct.fat || 0,
                Carbs: selectedProduct.carbs || 0,
                CategoryId: selectedProduct.categoryId || '',
                Restrictions: selectedProduct.restrictions || [],
                IsVegan: selectedProduct.isVegan || false,
            });
        }
    }, [selectedProduct]);

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        if (type === 'checkbox' && name === 'IsVegan') {
            setFormData((prev) => ({
                ...prev,
                [name]: checked,
            }));
        } else if (type === 'checkbox' && ALL_RESTRICTIONS.includes(name)) {
            setFormData((prev) => {
                const newRestrictions = checked
                    ? [...prev.Restrictions, name]
                    : prev.Restrictions.filter((r) => r !== name);
                return { ...prev, Restrictions: newRestrictions };
            });
        } else {
            setFormData((prev) => ({
                ...prev,
                [name]: value,
            }));
        }
    };

    const handleSubmit = () => {
        handleSaveChanges(formData);
        handleClose();
    };

    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Edit Product</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group controlId="productName">
                        <Form.Label>Product Name</Form.Label>
                        <Form.Control
                            type="text"
                            name="ProductName"
                            value={formData.ProductName}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group controlId="description" className="mt-2">
                        <Form.Label>Description</Form.Label>
                        <Form.Control
                            type="text"
                            name="Description"
                            value={formData.Description}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group controlId="price" className="mt-2">
                        <Form.Label>Price</Form.Label>
                        <Form.Control
                            type="number"
                            name="Price"
                            value={formData.Price}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group controlId="calories" className="mt-2">
                        <Form.Label>Calories</Form.Label>
                        <Form.Control
                            type="number"
                            name="Calories"
                            value={formData.Calories}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group controlId="protein" className="mt-2">
                        <Form.Label>Protein</Form.Label>
                        <Form.Control
                            type="number"
                            name="Protein"
                            value={formData.Protein}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group controlId="fat" className="mt-2">
                        <Form.Label>Fat</Form.Label>
                        <Form.Control
                            type="number"
                            name="Fat"
                            value={formData.Fat}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group controlId="carbs" className="mt-2">
                        <Form.Label>Carbs</Form.Label>
                        <Form.Control
                            type="number"
                            name="Carbs"
                            value={formData.Carbs}
                            onChange={handleChange}
                        />
                    </Form.Group>
                    <Form.Group controlId="category" className="mt-2">
                        <Form.Label>Category</Form.Label>
                        <Form.Control
                            as="select"
                            name="CategoryId"
                            value={formData.CategoryId}
                            onChange={handleChange}
                        >
                            <option value="">Select Category</option>
                            {categories && categories.map((cat) => (
                                <option key={cat.id} value={cat.id}>
                                    {cat.categoryName}
                                </option>
                            ))}
                        </Form.Control>
                    </Form.Group>
                    <Form.Group controlId="restrictions" className="mt-2">
                        <Form.Label>Restrictions</Form.Label>
                        <div>
                            {ALL_RESTRICTIONS.map((restriction) => (
                                <Form.Check
                                    key={restriction}
                                    type="checkbox"
                                    name={restriction}
                                    label={restriction.charAt(0).toUpperCase() + restriction.slice(1)}
                                    checked={formData.Restrictions.includes(restriction)}
                                    onChange={handleChange}
                                    inline
                                />
                            ))}
                        </div>
                    </Form.Group>
                    <Form.Group controlId="isVegan" className="mt-2">
                        <Form.Check
                            type="checkbox"
                            label="Is Vegan"
                            name="IsVegan"
                            checked={formData.IsVegan}
                            onChange={handleChange}
                        />
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Cancel
                </Button>
                <Button variant="primary" onClick={handleSubmit}>
                    Save Changes
                </Button>
            </Modal.Footer>
        </Modal>
    );
}