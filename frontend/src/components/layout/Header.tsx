import {Link, useNavigate} from 'react-router-dom';
import {
    AppBar,
    Toolbar,
    Typography,
    Button,
    IconButton, MenuItem, Select, SelectChangeEvent, Stack,
} from '@mui/material';
import {HOME_ROUTE, LOGIN_ROUTE, REGISTER_ROUTE} from '@router/routes.ts';
import {Logout as LogoutIcon} from '@mui/icons-material';

import {getUserActions} from '@store/AuthStore.ts';

import useScopedContextTranslator from '../../i18n/hooks/useScopedTranslator.ts';
import i18n from '../../i18n/i18n.ts';

const Header = () => {
    const {t} = useScopedContextTranslator();
    const navigate = useNavigate();
    const {logOutUser, isUserAuthenticated} = getUserActions();
    const isAuthenticated = isUserAuthenticated();

    const handleLanguageChange = (event: SelectChangeEvent | string) => {
        if (typeof event === 'object' && 'target' in event) {
            const newLang = event.target.value as string;
            i18n.changeLanguage(newLang);
            localStorage.setItem('language', newLang);
        }
    };

    const handleLogout = () => {
        logOutUser();
        navigate(HOME_ROUTE);
    };

    return (
        <AppBar position='static'>
            <Toolbar>
                <Typography
                    variant='h6'
                    component={Link}
                    to={HOME_ROUTE}
                    sx={{flexGrow: 1, textDecoration: 'none', color: 'inherit'}}
                >
                    {t('title')}
                </Typography>
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