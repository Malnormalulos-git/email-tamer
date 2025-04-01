import {Link, useNavigate} from 'react-router-dom';
import {
    AppBar,
    Toolbar,
    Typography,
    Button,
    IconButton,
    MenuItem,
    Select,
    SelectChangeEvent,
    Stack,
} from '@mui/material';
import {DEMO_ROUTE, HOME_ROUTE, LOGIN_ROUTE, REGISTER_ROUTE} from '@router/routes.ts';
import {Logout as LogoutIcon} from '@mui/icons-material';


import Logo from '@components/Logo.tsx';

import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';

import useAuthStore from '@store/AuthStore.ts';

import i18n from '../../i18n/i18n.ts';

const Header = () => {
    const {t} = useScopedContextTranslator();
    const navigate = useNavigate();

    const {isAuthenticated, logout} = useAuthStore((state) => ({
        isAuthenticated: state.isAuthenticated,
        logout: state.logout,
    }));

    const handleLanguageChange = (event: SelectChangeEvent | string) => {
        if (typeof event === 'object' && 'target' in event) {
            const newLang = event.target.value as string;
            i18n.changeLanguage(newLang);
            localStorage.setItem('language', newLang);
        }
    };

    const handleLogout = () => {
        logout();
        navigate(HOME_ROUTE);
    };

    return (
        <AppBar position='static'>
            <Toolbar
                sx={{
                    display: 'flex',
                    flexDirection: 'row',
                    alignItems: 'center',
                }}
            >
                <Stack
                    direction='row'
                    alignItems='center'
                    component={Link}
                    to={isAuthenticated ? HOME_ROUTE : DEMO_ROUTE}
                    sx={{textDecoration: 'none', color: 'inherit', flexGrow: 1}}
                >
                    <Logo sx={{width: 35, height: 35, marginRight: 1}}/>
                    <Typography variant='h6'>{t('title')}</Typography>
                </Stack>
                <Select
                    value={i18n.language}
                    onChange={handleLanguageChange}
                    variant='standard'
                    size='small'
                    sx={{
                        mr: 2,
                        color: 'inherit'
                    }}
                >
                    <MenuItem value='en'>
                        <img
                            src='https://flagsapi.com/GB/flat/24.png'
                            alt='English'
                            style={{marginRight: 8, verticalAlign: 'middle'}}
                        />
                        EN
                    </MenuItem>
                    <MenuItem value='ua'>
                        <img
                            src='https://flagsapi.com/UA/flat/24.png'
                            alt='Ukrainian'
                            style={{marginRight: 8, verticalAlign: 'middle'}}
                        />
                        УКР
                    </MenuItem>
                </Select>
                {isAuthenticated ? (
                    <IconButton color='inherit' onClick={handleLogout} aria-label='logout'>
                        <LogoutIcon/>
                    </IconButton>
                ) : (
                    <Stack direction='row-reverse' alignItems='center'>
                        <Button color='inherit' component={Link} to={LOGIN_ROUTE}>
                            {t('login')}
                        </Button>
                        /
                        <Button color='inherit' component={Link} to={REGISTER_ROUTE}>
                            {t('register')}
                        </Button>
                    </Stack>
                )}
            </Toolbar>
        </AppBar>
    );
};

export default Header;