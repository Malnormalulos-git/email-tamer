import {AppBar, Toolbar, Stack, Box, Typography} from '@mui/material';
import {Link} from 'react-router-dom';
import {DEMO_ROUTE, HOME_ROUTE} from '@router/routes.ts';
import Logo from '@components/Logo.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import useAuthStore from '@store/AuthStore.ts';
import {HEADER_HEIGHT} from '@utils/constants.ts';
import SearchBar from '@components/layout/header/SearchBar.tsx';
import SearchToggle from '@components/layout/header/SearchToggle.tsx';
import LanguageSelector from '@components/layout/header/LanguageSelector.tsx';
import AuthControls from '@components/layout/header/AuthControls.tsx';

const Header = () => {
    const {t} = useScopedContextTranslator();
    const {isAuthenticated} = useAuthStore((state) => ({
        isAuthenticated: state.isAuthenticated,
    }));

    return (
        <AppBar position='sticky'>
            <Toolbar
                sx={{
                    height: HEADER_HEIGHT,
                    display: 'flex',
                    alignItems: 'center',
                }}
            >
                <Stack
                    direction='row'
                    alignItems='center'
                    component={Link}
                    to={isAuthenticated ? HOME_ROUTE : DEMO_ROUTE}
                    sx={{
                        textDecoration: 'none',
                        color: 'inherit',
                        mr: {xs: 1, sm: 2},
                        transition: 'transform 0.2s',
                        '&:hover': {transform: 'scale(1.03)'},
                    }}
                >
                    <Logo sx={{width: {xs: 30, sm: 35}, height: {xs: 30, sm: 35}, mr: 1}}/>
                    <Typography
                        variant='h6'
                        sx={{fontSize: {xs: '1rem', sm: '1.25rem'}, fontWeight: 500}}
                    >
                        {t('title')}
                    </Typography>
                </Stack>

                {isAuthenticated && (
                    <Box sx={{flexGrow: 1, mx: {sm: 2}, display: {xs: 'none', sm: 'block'}}}>
                        <SearchBar/>
                    </Box>
                )}

                <Stack
                    direction='row'
                    alignItems='center'
                    spacing={{xs: 0.5, sm: 1}}
                    sx={{ml: 'auto'}}
                >
                    {isAuthenticated && <SearchToggle/>}

                    <Stack
                        direction='row'
                        alignItems='center'
                        spacing={0.5}
                        sx={{display: {xs: 'flex', md: 'none'}}}
                    >
                        <LanguageSelector/>
                        <AuthControls/>
                    </Stack>

                    <Stack
                        direction='row'
                        alignItems='center'
                        spacing={1}
                        sx={{display: {xs: 'none', md: 'flex'}}}
                    >
                        <LanguageSelector/>
                        <AuthControls/>
                    </Stack>
                </Stack>
            </Toolbar>
        </AppBar>
    );
};

export default Header;