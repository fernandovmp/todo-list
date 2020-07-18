import React, { useState } from 'react';
import { FormInput } from './FormInput';
import logo from '../../assets/done_outline-black.svg';
import './styles.css';

export const Login: React.FC = () => {
    const [isLogin, setIsLogin] = useState(true);
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
    };

    return (
        <div className="login-page">
            <div className="login-container">
                <header className="title-container">
                    <img src={logo} alt="logo" /> <strong>Todo List</strong>
                </header>
                <form className="login-form" onSubmit={handleSubmit}>
                    <FormInput
                        id="username"
                        label="Username"
                        type="text"
                        onValueChange={setUsername}
                    />
                    <FormInput
                        id="password"
                        label="Password"
                        type="password"
                        onValueChange={setPassword}
                    />
                    {!isLogin && (
                        <FormInput
                            id="confirmPassword"
                            label="Confirm password"
                            type="password"
                            onValueChange={setConfirmPassword}
                        />
                    )}
                    <button className="submit-button" type="submit">
                        {isLogin ? 'LogIn' : 'Sign Up'}
                    </button>
                </form>
                <p
                    className="switch-login-sign-text"
                    onClick={() => setIsLogin(!isLogin)}
                >
                    {isLogin
                        ? 'New here? Create an account'
                        : 'Already have an account?'}
                </p>
            </div>
        </div>
    );
};
