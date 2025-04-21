import {StrictMode} from 'react';
import {createRoot} from 'react-dom/client';

import {ThemeProvider} from '@mui/material/styles';

import {I18nextProvider} from 'react-i18next';

import {CssBaseline} from '@mui/material';

import App from './App.tsx';
import theme from './theme.ts';


import i18n from './i18n/i18n.ts';


createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <I18nextProvider i18n={i18n}>
            <ThemeProvider theme={theme}>
                <CssBaseline>
                    <App/>
                </CssBaseline>
            </ThemeProvider>
        </I18nextProvider>
    </StrictMode>,
);
