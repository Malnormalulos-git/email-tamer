import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import {ButtonGroup} from '@mui/material';
import Button from '@mui/material/Button';
import AppBar from '@mui/material/AppBar';

import useScopedContextTranslator from '../i18n/hooks/useScopedTranslator.ts';
import i18n from '../i18n/i18n.ts';

const Header = () => {
    const {t} = useScopedContextTranslator();

    const changeLanguage = (lng: string) => {
        i18n.changeLanguage(lng);
    };

    return (
        <AppBar position='static'>
            <Toolbar>
                <Typography variant='h6' component='div' sx={{flexGrow: 1}}>
                    {t('title')}
                </Typography>
                <ButtonGroup variant='outlined' aria-label='Basic button group'>
                    <Button color='inherit' onClick={() => changeLanguage('en')}>EN</Button>
                    <Button color='inherit' onClick={() => changeLanguage('ua')}>УКР</Button>
                </ButtonGroup>
                <Button color='inherit'>{t('login')}</Button>
            </Toolbar>
        </AppBar>
    );
};

export default Header;