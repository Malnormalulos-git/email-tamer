import {TextField, TextFieldProps} from '@mui/material';
import {UseFormReturn, Path} from 'react-hook-form';
import _ from 'lodash';

type TextInputProps<T extends object> = TextFieldProps & {
    label: string;
    id: Path<T>;
    form: UseFormReturn<T>;
};

const TextInputControl = <T extends object>({label, id, form, ...restProps}: TextInputProps<T>) => {
    const {register, formState: {errors}} = form;

    return (
        <TextField
            fullWidth
            label={label}
            margin='normal'
            helperText={_.get(errors, id as string)?.message}
            error={Boolean(_.get(errors, id as string))}
            {...register(id)}
            {...restProps}
        />
    );
};

export default TextInputControl;