import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { FaUser, FaLock, FaEnvelope } from 'react-icons/fa';
import { jwtDecode } from 'jwt-decode';
import createSignalRConnection from '../../services/signalRService'; // Import funkcji SignalR
import './LoginRegister.css';

function LoginRegister({ setUsername }) {
    const navigate = useNavigate();
    const [action, setAction] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const [connection, setConnection] = useState(null); // Przechowujemy połączenie SignalR

    const registerLink = () => {
        setAction(' active');
        setErrorMessage('');
    };

    const loginLink = () => {
        setAction('');
        setErrorMessage('');
    };

    const handleLoginSubmit = async (e) => {
        e.preventDefault();
        console.log('Login form submitted');

        const response = await fetch('http://localhost:5084/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                email: e.target[0].value,
                password: e.target[1].value
            })
        });

        if (response.ok) {
            console.log('Login successful');
            const result = await response.json();
            console.log(result);

            const decodedToken = jwtDecode(result.token);
            console.log('Decoded Token:', decodedToken);

            setUsername(result.username);
            localStorage.setItem('token', result.token);
            localStorage.setItem('username', result.username);

            // Tworzenie połączenia SignalR
            const newConnection = createSignalRConnection(result.token);
            setConnection(newConnection);

            newConnection
                .start()
                .then(() => console.log("✅ SignalR połączenie nawiązane"))
                .catch(err => console.error("❌ Błąd połączenia SignalR:", err));

            navigate('/');
        } else {
            const result = await response.json();
            console.log(result);
            setErrorMessage(result[0]?.description || 'Login failed');
        }
    };

    const handleRegisterSubmit = async (e) => {
        e.preventDefault();
        console.log('Register form submitted');

        const response = await fetch('http://localhost:5084/api/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                username: e.target[0].value,
                email: e.target[1].value,
                password: e.target[2].value
            })
        });

        if (response.ok) {
            console.log('Registration successful');
            alert('Registration successful');
            navigate('/login');
        } else {
            const result = await response.json();
            console.log(result);
            setErrorMessage(result[0]?.description || 'Registration failed');
        }
    };

    return (
        <div className="mainPage">
            <div className={`wrapper${action}`}>
                <div className="form-box login">
                    <form onSubmit={handleLoginSubmit}>
                        <h1>Login</h1>
                        <div className="input-box">
                            <input type="text" placeholder="Username" required />
                            <FaUser className="icon" />
                        </div>
                        <div className="input-box">
                            <input type="password" placeholder="Password" required />
                            <FaLock className="icon" />
                        </div>
                        <p>{errorMessage}</p>
                        <div className="remember-forgot">
                            <label><input type="checkbox" />Remember me</label>
                            <a href="#">Forgot Password</a>
                        </div>
                        <button type="submit">Login</button>
                        <div className="register-link">
                            <p>Don't have an account? <a href="#" onClick={registerLink}>Register</a></p>
                        </div>
                    </form>
                </div>

                <div className="form-box register">
                    <form onSubmit={handleRegisterSubmit}>
                        <h1>Register</h1>
                        <div className="input-box">
                            <input type="text" placeholder="Username" required />
                            <FaUser className="icon" />
                        </div>
                        <div className="input-box">
                            <input type="email" placeholder="Email" required />
                            <FaEnvelope className="icon" />
                        </div>
                        <div className="input-box">
                            <input type="password" placeholder="Password" required />
                            <FaLock className="icon" />
                        </div>
                        <p>{errorMessage}</p>
                        <div className="remember-forgot">
                            <label><input type="checkbox" />I agree to the terms & conditions</label>
                            <a href="#">Forgot Password</a>
                        </div>
                        <button type="submit">Register</button>
                        <div className="register-link">
                            <p>Already have an account? <a href="#" onClick={loginLink}>Login</a></p>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}

export default LoginRegister;
