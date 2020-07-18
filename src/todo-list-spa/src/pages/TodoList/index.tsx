import React, { useState } from 'react';
import logo from '../../assets/done_outline-black.svg';
import close from '../../assets/close.svg';
import add from '../../assets/add-black-36dp.svg';
import './styles.css';
import { TodoItem } from '../../models';

interface ITodoItemComponentProps {
    todoItem: TodoItem;
    onDelete: (todoItem: TodoItem) => void;
}

export const TodoItemComponent: React.FC<ITodoItemComponentProps> = ({
    todoItem,
    onDelete,
}) => {
    const [completed, setCompleted] = useState(todoItem.completed);

    const handleCompleted = () => {
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
    const [idCount, setIdCount] = useState(0);

    const handleCreateTodo = () => {
        const newTodo: TodoItem = {
            id: idCount,
            title: todoInput,
            completed: false,
        };
        setIdCount(idCount + 1);
        setTodoInput('');
        setTodos([...todos, newTodo]);
    };

    const handleDeleteTodo = (todoItem: TodoItem) => {
        setTodos(todos.filter((todo) => todo.id != todoItem.id));
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
