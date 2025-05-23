import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Form, ListGroup } from 'react-bootstrap';
import { searchItems } from '../api/search'; // <-- імпортуємо функцію пошуку (реалізуй цю функцію в api/search.js)

export default function SearchElement() {
    // const { user, isAuthenticated } = useAuth(); // Якщо треба отримати юзера/роль
    const navigate = useNavigate();
    const [searchTerm, setSearchTerm] = useState('');
    const [results, setResults] = useState([]);

    useEffect(() => {
        const delayDebounce = setTimeout(() => {
            if (searchTerm) {
                makeRequest(searchTerm);
            } else {
                setResults([]);
            }
        }, 500);

        return () => clearTimeout(delayDebounce);
    }, [searchTerm]);

    async function makeRequest(word) {
        try {
            const response = await searchItems(word); // <-- тут твій api-запит
            setResults(response || []);
        } catch (error) {
            console.error('Error fetching search results:', error);
            setResults([]);
        }
    }

    function handleItemClick(type, id) {
        setSearchTerm('');
        navigate(`/${type}/${id}`);
    }

    return (
        <div>
            <Form.Control
                type="text"
                placeholder="Search..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
            />
            {searchTerm && results.length > 0 && (
                <ListGroup className="mt-2">
                    {results.map((item) => (
                        <ListGroup.Item
                            key={item.id}
                            action
                            onClick={() => handleItemClick(item.type, item.id)}
                        >
                            {item.name} ({item.type})
                        </ListGroup.Item>
                    ))}
                </ListGroup>
            )}
        </div>
    );
}