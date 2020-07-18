import React, { useState } from 'react';
import close from '../../../assets/close.svg';
import { TodoItem } from '../../../models';
import './styles.css';

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
