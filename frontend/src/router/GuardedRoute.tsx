import {FC, ReactElement} from 'react';
import {Navigate, useLocation} from 'react-router-dom';
import _ from 'lodash';


import {UserRole} from '@api/emailTamerApiSchemas.ts';

import useAuthStore from '@store/AuthStore.ts';

import {REDIRECT_TO_PARAM} from '@router/urlParams.ts';

import {DEMO_ROUTE, LOGIN_ROUTE} from './routes';

interface GuardedRouteProps {
    page: ReactElement;
    roles?: UserRole[];
}

export const GuardedRoute: FC<GuardedRouteProps> = (props) => {
    const {page, roles} = props;
    const location = useLocation();

    const {isAuthenticated, user} = useAuthStore((state) => ({
        isAuthenticated: state.isAuthenticated,
        user: state.user,
    }));

    const isUserAuthorized = !_.isEmpty(roles) && user ? roles?.includes(user.role) : true;

    const isAccessAllowed = isAuthenticated && isUserAuthorized;

    if (isAccessAllowed) return <>{page}</>;
    if (!isAuthenticated)
        return <Navigate to={`${DEMO_ROUTE}?${REDIRECT_TO_PARAM}=${location.pathname + location.search}`}/>;
    if (isAuthenticated && !isUserAuthorized) return <Navigate to={DEMO_ROUTE} replace/>;
};