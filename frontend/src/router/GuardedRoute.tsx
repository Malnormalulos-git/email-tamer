import {FC, ReactElement} from 'react';
import {Navigate, useLocation} from 'react-router-dom';
import _ from 'lodash';

import {getUser, getUserActions} from '@store/AuthStore';

import {UserRole} from '@api/emailTamerApiSchemas.ts';

import {HOME_ROUTE, LOGIN_ROUTE} from './routes';

interface GuardedRouteProps {
    page: ReactElement,
    roles?: UserRole[]
}

export const GuardedRoute: FC<GuardedRouteProps> = (props) => {

    const {page, roles} = props;
    const location = useLocation();
    const {
        isUserAuthenticated: isAuthenticated,
    } = getUserActions();

    const user = getUser();

    const isUserAuthenticated = isAuthenticated();
    const isUserAuthorized = !_.isEmpty(roles) && user
        ? roles!.includes(user.role)
        : true;

    const isAccessAllowed = isUserAuthenticated && isUserAuthorized;

    if (isAccessAllowed) return <>{page}</>;
    if (!isUserAuthenticated) return <Navigate
        to={`${LOGIN_ROUTE}?redirectTo=${location.pathname + location.search}`}/>;
    if (isUserAuthenticated && !isUserAuthorized) return <Navigate to={HOME_ROUTE} replace/>;
};
