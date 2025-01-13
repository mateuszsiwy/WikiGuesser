import React, { useState } from 'react';
import './LoginRegister.css';
import { FaUser, FaLock, FaEnvelope } from 'react-icons/fa'; 

function LoginRegister() {

    const [action, setAction] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const registerLink = () => {
        setAction(' active')
        setErrorMessage('');
    }
    const loginLink = () => {
        setAction('');
        setErrorMessage('');
    }

    const handleLoginSubmit = async (e) => {
        e.preventDefault();
        alert('Login form submitted');
        const response = await fetch(`http://localhost:5084/api/auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                email: e.target[0].value,
                password: e.target[1].value
            })
        });
        console.log(response);
    }

    const handleRegisterSubmit = async (e) => {
        e.preventDefault();
        console.log('Register form submitted');
        console.log(e.target[0].value);
        console.log(e.target[1].value);
        console.log(e.target[2].value);
        const response = await fetch(`http://localhost:5084/api/auth/register`, {
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
        } else {
            const result = await response.json();
            console.log(result);
            setErrorMessage(result[0].description);
            console.log(errorMessage);
            console.log('Registration failed');
        }
    }

    return (
        <>
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
                            <p>Don't have an account? <a href="#" onClick={registerLink }>Register</a></p>
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
                            <p>Already have an account? <a href="#" onClick={ loginLink }>Login</a></p>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        </>
    );
}

export default LoginRegister;