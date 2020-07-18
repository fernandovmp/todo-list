import React, { useState, useEffect } from 'react';
import logo from '../../assets/done_outline-black.svg';
import close from '../../assets/close.svg';
import add from '../../assets/add-black-36dp.svg';
import './styles.css';
import { TodoItem } from '../../models';
import { todoApiFactory } from '../../services/todoApi';
import { useHistory } from 'react-router-dom';

interface ITodoItemComponentProps {
    todoItem: TodoItem;
    onDelete: (todoItem: TodoItem) => void;
    onMarkAsCompleted: (todoItem: TodoItem) => void;
}

export const TodoItemComponent: React.FC<ITodoItemComponentProps> = ({
    todoItem,
    onDelete,
    onMarkAsCompleted,
}) => {
    const [completed, setCompleted] = useState(todoItem.completed);

    const handleCompleted = () => {
        onMarkAsCompleted({ ...todoItem, completed: !completed });
        setCompleted(!completed);
    };

    const handleDelete = () => {
        onDelete(todoItem);
    };

    return (
        <div className="todo-item-container">
            <input
                className="completed-checkbox"
                type="checkbox"
                checked={completed}
                onChange={handleCompleted}
            />
            <p className="todo-item-title">{todoItem.title}</p>
            <img
                className="delete-button"
                src={close}
                alt="Delete"
                onClick={() => handleDelete()}
            />
        </div>
    );
};

export const TodoList: React.FC = () => {
    const [todos, setTodos] = useState<TodoItem[]>([]);
    const [todoInput, setTodoInput] = useState('');
    const [token] = useState(() => {
        return window.sessionStorage.getItem('token') ?? '';
    });
    const history = useHistory();

    useEffect(() => {
        const { getTodos } = todoApiFactory(token);
        getTodos().then((result) => {
            if (result.status !== 'success') {
                history.push('/login');
                return;
            }
            setTodos(result.data!);
        });
    }, [history, token]);

    const handleCreateTodo = async () => {
        const { createTodo } = todoApiFactory(token);
        const todo: TodoItem = {
            id: 0,
            title: todoInput,
            completed: false,
        };
        setTodoInput('');

        const response = await createTodo(todo);

        if (
            response.status === 'unauthorized' ||
            response.status === 'forbid'
        ) {
            history.push('/login');
            return;
        }

        if (response.status !== 'success') return;

        const newTodo = response.data!;
        setTodos([...todos, newTodo]);
    };

    const handleDeleteTodo = async (todoItem: TodoItem) => {
        const { deleteTodo } = todoApiFactory(token);
        const response = await deleteTodo(todoItem.id);

        if (
            response.status === 'unauthorized' ||
            response.status === 'forbid'
        ) {
            history.push('/login');
            return;
        }

        if (response.status !== 'success') return;
        setTodos(todos.filter((todo) => todo.id !== todoItem.id));
    };

    const handleCompleteTodo = async (todoItem: TodoItem) => {
        const { updateTodo } = todoApiFactory(token);
        const response = await updateTodo(todoItem);
        if (
            response.status === 'unauthorized' ||
            response.status === 'forbid'
        ) {
            history.push('/login');
        }
    };

    return (
        <div className="todolist-page">
            <header className="todolist-title-container">
                <img src={logo} alt="logo" /> <strong>Todo List</strong>
            </header>
            <main className="todo-list-main">
                <ul>
                    {todos.map((todo) => (
                        <li key={todo.id}>
                            <TodoItemComponent
                                todoItem={todo}
                                onDelete={handleDeleteTodo}
                                onMarkAsCompleted={handleCompleteTodo}
                            />
                        </li>
                    ))}
                </ul>
            </main>
            <div className="new-todo-input">
                <input
                    type="text"
                    placeholder="new todo..."
                    onChange={(e) => setTodoInput(e.target.value)}
                    value={todoInput}
                />
                <button onClick={() => handleCreateTodo()}>
                    <img src={add} alt="Create" />
                </button>
            </div>
        </div>
    );
};
