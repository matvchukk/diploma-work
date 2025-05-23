import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getReviews } from '../api/reviews';
import { useAuth } from '../Contexts/AuthProvider';

export default function ReviewsPage() {
    const { role, userId } = useAuth();
    const [reviews, setReviews] = useState([]);
    const navigate = useNavigate();

    async function fetchReviewsData() {
        try {
            const response = await getReviews({ userId });
            setReviews(Array.isArray(response.data) ? response.data : []);
            console.log(reviews);
        } catch (error) {
            console.error('Error fetching reviews:', error);
            setReviews([]);
        }
    }

    useEffect(() => {
        if (userId) {
            fetchReviewsData();
        }
        // eslint-disable-next-line
    }, [userId, role]);

    const renderStars = (rating) => (
        <>
            {[...Array(5)].map((star, index) => (
                <span key={index} className={index < rating ? "text-warning" : "text-secondary"}>
                    ★
                </span>
            ))}
        </>
    );

    const handleViewDetails = (productId) => {
        navigate(`/Product/${productId}`);
    };

    return (
        <div className="container mt-5" style={{ minHeight: '100vh' }}>
            <h2>User Reviews</h2>
            <div className="row">
                {reviews.length > 0 ? reviews.map(review => (
                    <div className="col-md-4 mb-4" key={review.id}>
                        <div className="card">
                            <div className="card-body">
                                <h5 className="card-title">
                                    <button
                                        onClick={() => handleViewDetails(review.productId)}
                                        className="btn btn-link text-decoration-none"
                                    >
                                        Product {review.productName}
                                    </button>
                                </h5>
                                <div className="card-text">
                                    <div className="mb-2">
                                        {renderStars(review.rating)}
                                    </div>
                                    <p>{review.comment}</p>
                                    <small className="text-muted">
                                        {new Date(review.createdDate).toLocaleDateString()}
                                    </small>
                                </div>
                                <div className="mt-2">
                                    <span className="badge bg-light text-dark">
                                        Reviewer: {review.userName}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                )) : (
                    <div className="col-12">
                        <p>No reviews found.</p>
                    </div>
                )}
            </div>
        </div>
    );
}