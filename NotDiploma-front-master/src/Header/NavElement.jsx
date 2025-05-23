import "./Nav.css";
import { Link } from 'react-router-dom';
import { useState } from 'react';


export default function NavElement({ text, to, onClick, children }) {
    const [open, setOpen] = useState(false);

    // Якщо є children — це дропдаун
    if (children) {
        const handleClick = (e) => {
            e.preventDefault();
            setOpen((prev) => !prev);
            if (onClick) onClick();
        };

        return (
            <div className="profile-dropdown">
                <div className="drop-btn">
                    {/* "to" може бути, але якщо є children — не переходимо по посиланню */}
                    <span onClick={handleClick} style={{ cursor: 'pointer' }}>{text}</span>
                </div>
                {open && <div className="dropdown">{children}</div>}
            </div>
        );
    }

    // Якщо children нема — це звичайний лінк
    return (
        <div className="profile-dropdown">
            <div className="drop-btn">
                <Link to={to} onClick={onClick}>{text}</Link>
            </div>
        </div>
    );
}



export function DropDownItem({ text, to, onClick, closeMenu }) {
    if (to) {
        return (
            <div className='menu-item' onClick={closeMenu}>
                <Link to={to} style={{ textDecoration: 'none', color: 'inherit' }}>{text}</Link>
            </div>
        );
    }
    return (
        <div className='menu-item' onClick={() => { onClick && onClick(); closeMenu && closeMenu(); }}>
            {text}
        </div>
    );
}