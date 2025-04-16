import {useState} from 'react';
import {Avatar, Menu, MenuItem, ListItemIcon, IconButton} from '@mui/material';
import {useNavigate} from 'react-router-dom';
import {HOME_ROUTE, LOGIN_ROUTE, REGISTER_ROUTE} from '@router/routes.ts';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import useAuthStore from '@store/AuthStore.ts';
import {HowToReg as RegIcon, LoginOutlined as LoginIcon, Logout as LogoutIcon} from '@mui/icons-material';

interface AuthControlsProps {
    onLogout?: () => void;
}

const AuthControls = ({onLogout}: AuthControlsProps) => {
    const {t} = useScopedContextTranslator();
    const navigate = useNavigate();
    const {isAuthenticated, logout} = useAuthStore((state) => ({
        isAuthenticated: state.isAuthenticated,
        logout: state.logout,
    }));
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);

    const handleClick = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const handleLogout = () => {
        logout();
        navigate(HOME_ROUTE);
        handleClose();
        if (onLogout) onLogout();
    };

    const handleLogin = () => {
        navigate(LOGIN_ROUTE);
        handleClose();
    };

    const handleRegister = () => {
        navigate(REGISTER_ROUTE);
        handleClose();
    };

    return (
        <>
            <IconButton
                onClick={handleClick}
                sx={{p: 1}}
            >
                <Avatar sx={{width: 32, height: 32}}/>
            </IconButton>
            {open && <Menu
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
                onClick={handleClose}
            >
                {isAuthenticated
                    ? <MenuItem onClick={handleLogout}>
                        <ListItemIcon>
                            <LogoutIcon fontSize='small'/>
                        </ListItemIcon>
                        {t('logout')}
                    </MenuItem>
                    : <>
                        <MenuItem onClick={handleLogin}>
                            <ListItemIcon>
                                <LoginIcon fontSize='small'/>
                            </ListItemIcon>
                            {t('login')}
                        </MenuItem>
                        <MenuItem onClick={handleRegister}>
                            <ListItemIcon>
                                <RegIcon fontSize='small'/>
                            </ListItemIcon>
                            {t('register')}
                        </MenuItem>
                    </>
                }
            </Menu>}
        </>
    );
};

export default AuthControls;