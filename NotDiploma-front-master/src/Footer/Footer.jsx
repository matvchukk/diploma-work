import './Footer.css'

export default function FooterComponent(){
    return(
        <footer className="footer">
            <div className="footer-left">
                <h2>AVET</h2>
                <p>We help you find the use of your assets</p>
                <div className="footer-icons">
                <a href="#"><img src="telegram-icon.png" alt="Telegram"></img></a>
                <a href="#"><img src="instagram-icon.png" alt="Instagram"></img></a>
                <a href="#"><img src="facebook-icon.png" alt="Facebook"></img></a>
                </div>
            </div>
            
            <div className="footer-right">
                <div className="footer-column">
                <h3>Information</h3>
                <ul>
                    <li><a href="#">About</a></li>
                    <li><a href="#">Product</a></li>
                    <li><a href="#">Blog</a></li>
                </ul>
                </div>
                <div className="footer-column">
                <h3>Company</h3>
                <ul>
                    <li><a href="#">Community</a></li>
                    <li><a href="#">Career</a></li>
                    <li><a href="#">Our story</a></li>
                </ul>
                </div>
                <div className="footer-column">
                <h3>Contact</h3>
                <ul>
                    <li><a href="#">Getting Started</a></li>
                    <li><a href="#">Pricing</a></li>
                    <li><a href="#">Resources</a></li>
                </ul>
                </div>
            </div>
        </footer>
    )
}