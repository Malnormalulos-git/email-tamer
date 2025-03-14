import { DoNotDisturb as DoNotDisturbIcon } from '@mui/icons-material';
import { Typography, Button }  from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';

import {HOME_ROUTE} from '../router/routes.ts';

const NotFoundPage = () => {
    const { t } = useTranslation();
    const navigate = useNavigate();

    return (
        <div style={{ textAlign: 'center', marginTop: 80 }}>
            <DoNotDisturbIcon style={{ fontSize: 100, color: 'grey' }} />
            <Typography variant='h5' color='textSecondary' style={{ marginTop: 5 }}>
                {t('pageNotFound.header')}
            </Typography>
            <Button
                variant='contained'
                sx={{ mt: 3, mb: 2 }}
                onClick={() => navigate(HOME_ROUTE)}
            >
                {t('pageNotFound.return')}
            </Button> 
        </div>
    );
};

export default NotFoundPage;

