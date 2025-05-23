import { useEffect, useState } from 'react';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import { useNavigate } from 'react-router-dom';
import List from 'react-bootstrap/ListGroup';
import { getAllCategories } from '../api/categories'; // <-- власний API-запит

export default function CategoryList() {
    const [categories, setCategories] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await getAllCategories();
                setCategories(response.data);
            } catch (error) {
                console.error("Error fetching categories:", error);
            }
        };

        fetchData();
    }, []);

    const handleItemClick = (id) => {
        navigate(`/Category/${id}`);
    };

    return (
        <List>
            {categories.map(category => (
                <ListGroupItem 
                    key={category.id} 
                    onClick={() => handleItemClick(category.id)}
                    action
                >
                    <p>{category.categoryName}</p>
                </ListGroupItem>
            ))}
        </List>
    );
}
