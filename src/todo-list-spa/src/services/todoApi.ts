import { User, TodoItem } from '../models';

interface AuthenticateResult {
    token: string;
    user: User;
}

type ResultStatus =
    | 'success'
    | 'error'
    | 'not found'
    | 'unauthorized'
    | 'forbid';

interface ITodoApiResult<T> {
    status: ResultStatus;
    data?: T;
}

export interface ITodoApi {
    signUp(
        username: string,
        password: string,
        confirmPassword: string
    ): Promise<ITodoApiResult<User>>;
    authenticate(
        username: string,
        password: string
    ): Promise<ITodoApiResult<AuthenticateResult>>;
    createTodo(todo: TodoItem): Promise<ITodoApiResult<TodoItem>>;
    deleteTodo(id: number): Promise<ITodoApiResult<void>>;
    updateTodo(todo: TodoItem): Promise<ITodoApiResult<void>>;
    getTodos(): Promise<ITodoApiResult<TodoItem[]>>;
}

const todoApiPromise = <T>(
    status: ResultStatus,
    data?: T
): ITodoApiResult<T> => ({
    status,
    data,
});

export const todoApiFactory = (token?: string): ITodoApi => {
    const baseUrl = process.env.REACT_APP_API_URL;

    const url = (url: string) => `${baseUrl}/${url}`;

    const headers = (withAuthorization: boolean) => {
        const contentType = ['Content-Type', 'application/json'];
        let headers = [contentType];
        if (withAuthorization && token) {
            headers = [...headers, ['Authorization', `Bearer ${token}`]];
        }
        return headers;
    };

    return {
        async authenticate(username: string, password: string) {
            const bodyData = {
                username,
                password,
            };
            const response = await fetch(url('v1/users/login'), {
                method: 'POST',
                headers: headers(false),
                body: JSON.stringify(bodyData),
                credentials: 'include',
            });

            if (response.status === 404) return todoApiPromise('not found');
            if (response.status !== 200) return todoApiPromise('error');

            const data = await response.json();
            return todoApiPromise('success', data);
        },
        async createTodo(todo: TodoItem) {
            const response = await fetch(url('v1/todoitems'), {
                method: 'POST',
                headers: headers(true),
                body: JSON.stringify(todo),
                credentials: 'include',
            });
            if (response.status !== 201) return todoApiPromise('error');
            const data = await response.json();
            return todoApiPromise('success', data);
        },
        async deleteTodo(id: number) {
            const response = await fetch(url(`v1/todoitems/${id}`), {
                method: 'DELETE',
                headers: headers(true),
                credentials: 'include',
            });
            if (response.status === 403) return todoApiPromise('forbid');
            if (response.status >= 400) return todoApiPromise('error');
            return todoApiPromise('success');
        },
        async getTodos() {
            const response = await fetch(url(`v1/todoitems`), {
                method: 'GET',
                headers: headers(true),
                credentials: 'include',
            });
            if (response.status !== 200) return todoApiPromise('error');
            const data = await response.json();
            return todoApiPromise('success', data);
        },
        async signUp(
            username: string,
            password: string,
            confirmPassword: string
        ) {
            const bodyData = {
                username,
                password,
                confirmPassword,
            };
            const response = await fetch(url('v1/users'), {
                method: 'POST',
                headers: headers(false),
                body: JSON.stringify(bodyData),
                credentials: 'include',
            });
            if (response.status !== 201) return todoApiPromise('error');
            const data = await response.json();
            return todoApiPromise('success', data);
        },
        async updateTodo(todo: TodoItem) {
            const response = await fetch(url(`v1/todoitems/${todo.id}`), {
                method: 'PUT',
                headers: headers(true),
                body: JSON.stringify(todo),
                credentials: 'include',
            });
            if (response.status === 403) return todoApiPromise('forbid');
            if (response.status >= 400) return todoApiPromise('error');
            return todoApiPromise('success');
        },
    };
};
