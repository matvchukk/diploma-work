import 'bootstrap/dist/css/bootstrap.min.css';

function ReviewCard({ reviews }) {

    const renderStars = (rating) => {
        return (
            <>
                {[...Array(5)].map((star, index) => (
                    <span key={index} className={index < rating ? "text-warning" : "text-secondary"}>
                        ★
                    </span>
                ))}
            </>
        );
    };

    return (
        <div className="container my-4">
            <h3>Reviews</h3>
            <div className="row">
                {reviews.map(review => (
                    <div className="col-md-6 mb-4" key={review.Id}>
                        <div className="card" style={{ maxWidth: '100%', padding: '1rem' }}>
                            <div className="card-body">
                                <h5 className="card-title">
                                    <button 
                                        onClick={() => window.location.href = `/Product/${review.productId}`} 
                                        className="btn btn-link text-decoration-none">
                                        Product {review.productId}
                                    </button>
                                </h5>
                                <div className="card-text">
                                    <div className="mb-2">
                                        {renderStars(review.rating)}
                                    </div>
                                    <p>{review.comment}</p>
                                    <small className="text-muted">
                                        {new Date(review.reviewDate).toLocaleDateString()}
                                    </small>
                                </div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default ReviewCard;
