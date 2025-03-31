import {Box, Typography, Button, Container, Grid, Card, CardContent} from '@mui/material';
import {useNavigate} from 'react-router-dom';
import {REGISTER_ROUTE} from '@router/routes';

import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';

import multiTaskingImage from '../../public/multitasking-man.svg';

const DemoPage = () => {
    const {t} = useScopedContextTranslator();
    const navigate = useNavigate();

    return (
        <Container maxWidth='lg' sx={{py: 6}}>
            <Box textAlign='center' mb={6}>
                <Typography variant='h3' component='h1' gutterBottom>
                    {t('title')}
                </Typography>
                <Typography variant='h6' color='text.secondary'>
                    {t('subtitle')}
                </Typography>
            </Box>
            <Grid container spacing={4} alignItems='center' mb={6}>
                <Grid item xs={12} md={6}>
                    <Box
                        component='img'
                        src={multiTaskingImage}
                        alt={t('imageAlt')}
                        sx={{width: '100%', maxWidth: 500, mx: 'auto', display: 'block'}}
                    />
                </Grid>
                <Grid item xs={12} md={6}>
                    <Typography variant='h5' gutterBottom>
                        {t('targetAudienceTitle')}
                    </Typography>
                    <Typography variant='body1' color='text.secondary' paragraph>
                        {t('targetAudienceDescription')}
                    </Typography>
                </Grid>
            </Grid>

            <Box textAlign='center' mb={6}>
                <Typography variant='h4' gutterBottom>
                    {t('featuresTitle')}
                </Typography>
                <Grid container spacing={3} justifyContent='center'>
                    {[
                        {
                            title: t('features.backupTitle'),
                            description: t('features.backupDescription'),
                        },
                        {
                            title: t('features.accessTitle'),
                            description: t('features.accessDescription'),
                        },
                        {
                            title: t('features.searchTitle'),
                            description: t('features.searchDescription'),
                        },
                    ].map((feature, index) => (
                        <Grid item xs={12} sm={6} md={4} key={index}>
                            <Card sx={{height: '100%', display: 'flex', flexDirection: 'column'}}>
                                <CardContent>
                                    <Typography variant='h6' gutterBottom>
                                        {feature.title}
                                    </Typography>
                                    <Typography variant='body2' color='text.secondary'>
                                        {feature.description}
                                    </Typography>
                                </CardContent>
                            </Card>
                        </Grid>
                    ))}
                </Grid>
            </Box>

            <Box textAlign='center'>
                <Button
                    variant='contained'
                    color='primary'
                    size='large'
                    onClick={() => navigate(REGISTER_ROUTE)}
                >
                    {t('ctaButton')}
                </Button>
            </Box>
        </Container>
    );
};

export default DemoPage;