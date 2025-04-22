import {Box, BoxProps} from '@mui/material';

interface FieldsetProps extends BoxProps {
    children: React.ReactNode;
    disabled?: boolean;
}

const Fieldset = ({children, disabled = false, ...restProps}: FieldsetProps) => {
    return (
        <Box
            component='fieldset'
            disabled={disabled}
            sx={{border: 'none', p: 0, m: 0}}
            {...restProps}
        >
            {children}
        </Box>
    );
};

export default Fieldset;