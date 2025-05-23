import { useState, useEffect } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';

export default function EditSeller({ show, handleClose, sellerData, updateSellerData }) {
  // Коректні ключі згідно з бекендом
  const [formData, setFormData] = useState({
    StoreName: '',
    StoreDescription: '',
    ContactAddress: '',
    ContactPhone: '',
  });

  // Оновлюємо дані при відкритті модалки або зміні sellerData
  useEffect(() => {
    if (sellerData) {
      setFormData({
        StoreName: sellerData.storeName || '',
        StoreDescription: sellerData.storeDescription || '',
        ContactAddress: sellerData.contactAddress || '',
        ContactPhone: sellerData.contactPhone || '',
      });
    }
  }, [sellerData, show]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = async () => {
    await updateSellerData(formData); // не треба айді, бек сам знає через токен
    handleClose();
  };

  return (
    <Modal show={show} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title>Edit Seller Data</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form>
          <Form.Group controlId="formStoreName">
            <Form.Label>Store Name</Form.Label>
            <Form.Control
              type="text"
              name="StoreName"
              value={formData.StoreName}
              onChange={handleChange}
              placeholder="Enter store name"
            />
          </Form.Group>
          <Form.Group controlId="formDescription" className="mt-3">
            <Form.Label>Store Description</Form.Label>
            <Form.Control
              as="textarea"
              name="StoreDescription"
              value={formData.StoreDescription}
              onChange={handleChange}
              placeholder="Enter store description"
            />
          </Form.Group>
          <Form.Group controlId="formAddress" className="mt-3">
            <Form.Label>Contact Address</Form.Label>
            <Form.Control
              type="text"
              name="ContactAddress"
              value={formData.ContactAddress}
              onChange={handleChange}
              placeholder="Enter contact address"
            />
          </Form.Group>
          <Form.Group controlId="formPhone" className="mt-3">
            <Form.Label>Contact Phone</Form.Label>
            <Form.Control
              type="tel"
              name="ContactPhone"
              value={formData.ContactPhone}
              onChange={handleChange}
              placeholder="Enter contact phone"
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

