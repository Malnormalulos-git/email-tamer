import {Snackbar, Alert} from '@mui/material';
import {useState, useEffect} from 'react';
import _ from 'lodash';
import {useAppControlStore, getNotification} from '@store/AppControlStore';

const AppSnackbar = () => {
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const notification = useAppControlStore(getNotification);

    useEffect(() => {
        if (!_.isEmpty(notification)) {
            setSnackbarOpen(true);
        }
    }, [notification]);

    return (
        <Snackbar open={snackbarOpen} onClose={() => setSnackbarOpen(false)} autoHideDuration={4000}>
            <Alert severity={notification?.type} variant='filled' sx={{width: '100%'}}>
                {notification?.message}
            </Alert>
        </Snackbar>
    );
};

export default AppSnackbar;