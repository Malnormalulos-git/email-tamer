import {Box, CircularProgress, CircularProgressProps} from '@mui/material';

type ContentLoadingProps = CircularProgressProps

const ContentLoading = ({size = 24, ...restProps} : ContentLoadingProps) => {
    return (
        <Box
            sx={{
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                width: '100%',
            }}>
            <CircularProgress size={size} {...restProps}/>
        </Box>
    );
};

export default ContentLoading;