import {Stack, Switch, Typography} from '@mui/material';
import {Path, UseFormReturn} from 'react-hook-form';

interface DoubleLabeledSwitchProps<T extends object> {
    leftLabel: string;
    rightLabel: string;
    id: Path<T>;
    form: UseFormReturn<T>;
    disabled: boolean;
}

const DoubleLabeledSwitch = <T extends object>({
    leftLabel,
    rightLabel,
    id,
    form,
    disabled
}: DoubleLabeledSwitchProps<T>) => {
    const {register, watch} = form;
    const value = watch(id) as boolean;

    return (
        <Stack direction='row' component='label' alignItems='center' mt={1}>
            <Typography
                sx={{
                    userSelect: 'none',
                    '&:hover': {
                        cursor: 'pointer',
                    }
                }}
                color={value ? 'textSecondary' : 'textPrimary'}
            >{leftLabel}</Typography>
            <Switch
                {...register(id)}
                disabled={disabled}
            />
            <Typography
                sx={{
                    userSelect: 'none',
                    '&:hover': {
                        cursor: 'pointer',
                    }
                }}
                color={value ? 'textPrimary' : 'textSecondary'}
            >{rightLabel}</Typography>
        </Stack>
    );
};

export default DoubleLabeledSwitch;