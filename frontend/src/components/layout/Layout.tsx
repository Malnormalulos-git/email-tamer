import {Outlet} from 'react-router-dom';

import Header from '@components/layout/Header.tsx';

import AppSnackbar from '@components/layout/AppSnackbar.tsx';

import {TranslationScopeProvider} from '../../i18n/contexts/TranslationScopeContext.tsx';


const Layout = () => {

    return (
        <>
            <TranslationScopeProvider scope='header'>
                <Header/>
            </TranslationScopeProvider>
            <Outlet/>
            <AppSnackbar/>
        </>
    );
};

export default Layout;