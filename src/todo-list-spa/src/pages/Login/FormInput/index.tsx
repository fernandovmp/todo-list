import React, { useState } from 'react';
import visibilityOn from '../../../assets/visibility-24px.svg';
import visibilityOff from '../../../assets/visibility_off-24px.svg';
import './styles.css';

interface IFormInputProps {
    id: string;
    label: string;
    type: 'text' | 'password';
    onValueChange(value: string): void;
}

export const FormInput: React.FC<IFormInputProps> = ({
    id,
    label,
    type,
    onValueChange,
}) => {
    const [actualType, setActualType] = useState(type);
    return (
        <label className="form-input" htmlFor={id}>
            {label}
            <input
                id={id}
                type={actualType}
                onChange={(e) => onValueChange(e.target.value)}
            />
            {type === 'password' && (
                <div className="password-visibility-container">
                    <img
                        className="password-visibility-button"
                        src={
                            actualType === 'password'
                                ? visibilityOn
                                : visibilityOff
                        }
                        alt={actualType === 'password' ? 'Show' : 'Hide'}
                        onClick={() =>
                            setActualType(
                                actualType === 'password' ? 'text' : 'password'
                            )
                        }
                    />
                </div>
            )}
        </label>
    );
};
