import {Visibility, VisibilityOff} from '@mui/icons-material';
import {IconButton, InputAdornment, TextField, TextFieldProps} from '@mui/material';
import {useState} from 'react';

import {Path, UseFormReturn} from 'react-hook-form';
import _ from 'lodash';

type PasswordInputControlProps<T extends object> = TextFieldProps & {
    label: string;
    id: Path<T>;
    form: UseFormReturn<T>;
    showPassword?: boolean;
    handleClickShowPassword?: () => void;
};

const PasswordInputControl = <T extends object>({
    label,
    id,
    form,
    showPassword: externalShowPassword,
    handleClickShowPassword: externalHandleClickShowPassword,
    ...restProps
}: PasswordInputControlProps<T>) => {

    const [internalShowPassword, setInternalShowPassword] = useState(false);

    const {register, formState: {errors}} = form;

    const showPassword = externalShowPassword === undefined ? internalShowPassword : externalShowPassword;
    const handleClickShowPassword = externalHandleClickShowPassword || (() => {
        setInternalShowPassword(!internalShowPassword);
    });

    return (
        <TextField
            fullWidth
            label={label}
            margin='normal'
            helperText={_.get(errors, id as string)?.message}
            error={Boolean(_.get(errors, id as string))}
            type={showPassword ? 'text' : 'password'}
            {...register(id)}
            {...restProps}
            InputProps={{
                endAdornment: (
                    <InputAdornment position='end'>
                        <IconButton
                            aria-label='toggle password visibility'
                            edge='end'
                            onClick={handleClickShowPassword}
                        >
                            {showPassword ? <Visibility/> : <VisibilityOff/>}
                        </IconButton>
                    </InputAdornment>
                )
            }}
        />
    );
};

export default PasswordInputControl;