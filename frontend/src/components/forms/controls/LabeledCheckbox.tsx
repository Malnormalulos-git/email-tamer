import {Checkbox, FormControlLabel} from '@mui/material';
import {Path, UseFormReturn} from 'react-hook-form';

interface LabeledCheckboxProps<T extends object> {
    label: string;
    id: Path<T>;
    form: UseFormReturn<T>;
    disabled?: boolean;
}

const LabeledCheckbox = <T extends object>({label, id, form, disabled}: LabeledCheckboxProps<T>) => {
    const {register, watch} = form;
    const checked = watch(id);

    return (
        <FormControlLabel
            sx={{userSelect: 'none'}}
            control={
                <Checkbox
                    {...register(id)}
                    checked={checked}
                    disabled={disabled}
                />
            }
            label={label}
        />
    );
};

export default LabeledCheckbox;