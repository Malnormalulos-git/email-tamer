import {Button, ButtonProps} from '@mui/material';

interface SubmitButtonProps extends ButtonProps {
    children: React.ReactNode;
}

const SubmitButton = ({children, ...restProps}: SubmitButtonProps) => {
    return (
        <Button
            type='submit'
            fullWidth
            variant='contained'
            sx={{mt: 2}}
            {...restProps}
        >
            {children}
        </Button>
    );
};

export default SubmitButton;