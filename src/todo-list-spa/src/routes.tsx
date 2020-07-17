import React from 'react';
import { BrowserRouter, Route } from 'react-router-dom';
import { Login, TodoList } from './pages';

export const Routes: React.FC = () => {
    return (
        <BrowserRouter>
            <Route path="/login" exact component={Login} />
            <Route path="/" exact component={TodoList} />
        </BrowserRouter>
    );
};
