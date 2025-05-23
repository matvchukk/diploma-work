import { Link, useNavigate } from 'react-router-dom';
import '../Login/Login.css';
import { useState } from 'react';
import { useAuth } from '../Contexts/AuthProvider';

export default function Registration() {
  const [form, setForm] = useState({
    name: '',
    email: '',
    password: '',
    confirmPassword: '',
    age: '',
    sex: '',
    weightKg: '',
    heightCm: '',
    goal: '',
    budgetPerWeek: '',
    isVegan: false,
    restrictions: ''
  });
  const [errorMessage, setErrorMessage] = useState('');
  const { register } = useAuth();
  const navigate = useNavigate();

  const handleChange = e => {
    const { name, value, type, checked } = e.target;
    setForm(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (
      !form.email || !form.password || !form.confirmPassword ||
      !form.name || !form.age || !form.sex || !form.weightKg ||
      !form.heightCm || !form.goal || !form.budgetPerWeek
    ) {
      setErrorMessage('Please fill in all required fields.');
      return;
    }

    if (form.password !== form.confirmPassword) {
      setErrorMessage('Passwords do not match.');
      return;
    }

    try {
      await register({
        Name: form.name,
        Email: form.email,
        Password: form.password,
        Age: parseInt(form.age),
        Sex: form.sex,
        WeightKg: parseFloat(form.weightKg),
        HeightCm: parseFloat(form.heightCm),
        Goal: form.goal,
        BudgetPerWeek: parseFloat(form.budgetPerWeek),
        IsVegan: form.isVegan,
        Restrictions: form.restrictions
          ? form.restrictions.split(',').map(r => r.trim()).filter(r => r.length > 0)
          : []
      });
      navigate("/"); // Redirect after successful registration
    } catch (error) {
      setErrorMessage('Registration failed. Please try again.' + error);
    }
  };

  return (
    <div className="box">
      <div className="lrheader">
        <header>Register</header>
      </div>
      {errorMessage && <p className="error-message">{errorMessage}</p>}
      <form onSubmit={handleSubmit}>
        <div className="input-box">
          <input
            type="text"
            className="input-field"
            placeholder="Full Name"
            name="name"
            value={form.name}
            onChange={handleChange}
            required
          />
        </div>
        <div className="input-box">
          <input
            type="text"
            className="input-field"
            placeholder="Email"
            name="email"
            value={form.email}
            onChange={handleChange}
            required
          />
        </div>
        <div className="input-box">
          <input
            type="password"
            className="input-field"
            placeholder="Password"
            name="password"
            value={form.password}
            onChange={handleChange}
            required
          />
        </div>
        <div className="input-box">
          <input
            type="password"
            className="input-field"
            placeholder="Confirm Password"
            name="confirmPassword"
            value={form.confirmPassword}
            onChange={handleChange}
            required
          />
        </div>
        <div className="input-box">
          <input
            type="number"
            className="input-field"
            placeholder="Age"
            name="age"
            min="1"
            value={form.age}
            onChange={handleChange}
            required
          />
        </div>
        <div className="input-box">
          <select
            className="input-field"
            name="sex"
            value={form.sex}
            onChange={handleChange}
            required
          >
            <option value="">Select Sex</option>
            <option value="male">Male</option>
            <option value="female">Female</option>
            <option value="other">Other</option>
          </select>
        </div>
        <div className="input-box">
          <input
            type="number"
            className="input-field"
            placeholder="Weight (kg)"
            name="weightKg"
            min="0"
            value={form.weightKg}
            onChange={handleChange}
            required
          />
        </div>
        <div className="input-box">
          <input
            type="number"
            className="input-field"
            placeholder="Height (cm)"
            name="heightCm"
            min="0"
            value={form.heightCm}
            onChange={handleChange}
            required
          />
        </div>
        <div className="input-box">
          <select
            className="input-field"
            name="goal"
            value={form.goal}
            onChange={handleChange}
            required
          >
            <option value="">Select Goal</option>
            <option value="lose">Lose weight</option>
            <option value="maintain">Maintain weight</option>
            <option value="gain">Gain weight</option>
          </select>
        </div>
        <div className="input-box">
          <input
            type="number"
            className="input-field"
            placeholder="Budget per week"
            name="budgetPerWeek"
            min="0"
            step="0.01"
            value={form.budgetPerWeek}
            onChange={handleChange}
            required
          />
        </div>
        <div className="input-box d-flex align-items-center">
          <label htmlFor="isVegan" style={{ marginRight: '10px', fontSize: 14 }}>Vegan?</label>
          <input
            type="checkbox"
            id="isVegan"
            name="isVegan"
            checked={form.isVegan}
            onChange={handleChange}
            style={{ width: '20px', height: '20px' }}
          />
        </div>
        <div className="input-box">
          <input
            type="text"
            className="input-field"
            placeholder="Restrictions (comma separated, optional)"
            name="restrictions"
            value={form.restrictions}
            onChange={handleChange}
          />
        </div>
        <div className="input-submit">
          <button className="submit-btn" id="submit" type="submit">Sign Up</button>
        </div>
      </form>
      <div className="sign-in-link">
        <p>Already have an account? <Link to="/login">Login</Link></p>
      </div>
    </div>
  );
}