import {Button, ButtonProps} from '@mui/material';
import {useNavigate} from 'react-router-dom';

interface NavigateButtonProps extends ButtonProps {
    children: React.ReactNode;
    route: string;
}

const NavigateButton = ({children, route, ...restProps}: NavigateButtonProps) => {
    const navigate = useNavigate();

    return (
        <Button
            fullWidth
            variant='text'
            sx={{mt: 1}}
            onClick={() => navigate(route)}
            {...restProps}
        >
            {children}
        </Button>
    );
};

export default NavigateButton;