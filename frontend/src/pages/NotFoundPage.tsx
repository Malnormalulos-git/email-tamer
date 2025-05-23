import {DoNotDisturb as DoNotDisturbIcon} from '@mui/icons-material';
import {Typography, Button} from '@mui/material';
import {useNavigate} from 'react-router-dom';

import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';

import {HOME_ROUTE} from '../router/routes';

const NotFoundPage = () => {
    const navigate = useNavigate();
    const {t} = useScopedContextTranslator();

    return (
        <div style={{textAlign: 'center', marginTop: 80}}>
            <DoNotDisturbIcon style={{fontSize: 100, color: 'grey'}}/>
            <Typography variant='h5' color='textSecondary' style={{marginTop: 5}}>
                {t('header')}
            </Typography>
            <Button
                variant='contained'
                sx={{mt: 3, mb: 2}}
                onClick={() => navigate(HOME_ROUTE)}
            >
                {t('return')}
            </Button>
        </div>
    );
};

export default NotFoundPage;