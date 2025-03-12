import React, {useState} from 'react';
import {useNavigate} from 'react-router-dom';
import {FaEnvelope, FaLock, FaUser} from 'react-icons/fa';
import {jwtDecode} from 'jwt-decode';
import createSignalRConnection from '../../services/signalRService'; 
import './LoginRegister.css';

function LoginRegister({ setUsername }) {
    const navigate = useNavigate();
    const [action, setAction] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const [connection, setConnection] = useState(null);

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
        const API_URL = import.meta.env.VITE_API_URL;
        const response = await fetch(`${API_URL}/api/auth/login`, {
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
            alert("Wrong Credentials");
            const result = await response.json();
            console.log(result);
            setErrorMessage(result[0]?.description || 'Login failed');
        }
    };

    const handleRegisterSubmit = async (e) => {
        e.preventDefault();
        console.log('Register form submitted');
        setErrorMessage('');
        
        const API_URL = import.meta.env.VITE_API_URL;
        try {
            const response = await fetch(`${API_URL}/api/auth/register`, {
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
    
            const result = await response.json();
            
            if (response.ok) {
                console.log('Registration successful');
                alert('Registration successful');
                navigate('/login');
            } else {
                console.log('Registration failed:', result);
                
                if (Array.isArray(result) && result.length > 0) {
                    setErrorMessage(result[0].description || 'Registration failed');
                } else if (result.errors) {
                    const errorMsg = Object.values(result.errors).flat()[0];
                    setErrorMessage(errorMsg || 'Registration failed');
                } else if (result.message) {
                    setErrorMessage(result.message);
                } else {
                    setErrorMessage('Registration failed. Please try again.');
                }
            }
        } catch (error) {
            console.error('Registration error:', error);
            setErrorMessage('An unexpected error occurred. Please try changing the email address.');
        }
    };

    return (
        <div className="mainPage">
            <div className={`wrapper${action}`}>
                <div className="form-box login">
                    <form onSubmit={handleLoginSubmit}>
                        <h1>Login</h1>
                        <div className="input-box">
                            <input type="text" placeholder="Email" required />
                            <FaEnvelope className="icon" />
                        </div>
                        <div className="input-box">
                            <input type="password" placeholder="Password" required />
                            <FaLock className="icon" />
                        </div>
                        <p>{errorMessage}</p>
     
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
