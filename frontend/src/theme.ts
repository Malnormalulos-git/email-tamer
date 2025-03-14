import { createTheme } from '@mui/material/styles';
import { red } from '@mui/material/colors';

const theme = createTheme({
    palette: {
        primary: {
            main: '#26ff7d'
        },
        secondary: {
            main: '#3facb0',
            contrastText: '#FFFFFF'
        },
        error: {
            main: red.A400,
        }
    }
});

export default theme;