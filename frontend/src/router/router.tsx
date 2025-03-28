import {createBrowserRouter, createRoutesFromElements, Navigate, Route} from 'react-router-dom';

import NotFoundPage from '@pages/NotFoundPage.tsx';

import Layout from '@components/Layout.tsx';

import HomePage from '@pages/HomePage.tsx';

import {TranslationScopeProvider} from '../i18n/contexts/TranslationScopeContext.tsx';

import {HOME_ROUTE, NOT_FOUND_ROUTE} from './routes';

export const router = createBrowserRouter(
    createRoutesFromElements(
        <Route element={<Layout/>}>
            {/*<Route path={LOGIN_ROUTE} element={<LoginPage/>}/>*/}
            {/*<Route path={REGISTER_ROUTE} element={<RegisterPage/>}/>*/}
            <Route path={HOME_ROUTE} element={<HomePage/>}/>
            <Route path={NOT_FOUND_ROUTE} element={
                <TranslationScopeProvider scope='pageNotFound'>
                    <NotFoundPage/>
                </TranslationScopeProvider>}/>
            <Route path='*' element={<Navigate to={HOME_ROUTE} replace/>}/>
        </Route>
    )
);

