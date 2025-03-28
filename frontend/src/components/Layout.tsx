import {Outlet} from 'react-router-dom';

import Header from '@components/Header.tsx';

import {TranslationScopeProvider} from '../i18n/contexts/TranslationScopeContext.tsx';


const Layout = () => {

    return (
        <>
            <TranslationScopeProvider scope='header'>
                <Header/>
            </TranslationScopeProvider>
            <Outlet/>
        </>
    );
};

export default Layout;