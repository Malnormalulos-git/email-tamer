import {createBrowserRouter, createRoutesFromElements, Navigate, Route} from 'react-router-dom';

import NotFoundPage from '@pages/NotFoundPage.tsx';

import Layout from '@components/layout/Layout.tsx';

import HomePage from '@pages/HomePage.tsx';

import LoginPage from '@pages/LoginPage.tsx';

import RegisterPage from '@pages/RegisterPage.tsx';

import {TranslationScopeProvider} from '../i18n/contexts/TranslationScopeContext.tsx';

import {HOME_ROUTE, LOGIN_ROUTE, NOT_FOUND_ROUTE, REGISTER_ROUTE} from './routes';

export const router = createBrowserRouter(
    createRoutesFromElements(
        <Route element={<Layout/>}>
            <Route path={HOME_ROUTE} element={<HomePage/>}/>
            <Route path={LOGIN_ROUTE} element={
                <TranslationScopeProvider scope='loginPage'>
                    <LoginPage/>
                </TranslationScopeProvider>}/>
            <Route path={REGISTER_ROUTE} element={
                <TranslationScopeProvider scope='registerPage'>
                    <RegisterPage/>
                </TranslationScopeProvider>}/>
            <Route
                path={NOT_FOUND_ROUTE}
                element={
                    <TranslationScopeProvider scope='notFoundPage'>
                        <NotFoundPage/>
                    </TranslationScopeProvider>
                }
            />
            <Route path='*' element={<Navigate to={HOME_ROUTE} replace/>}/>
        </Route>
    )
);

