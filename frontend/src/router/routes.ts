import _ from 'lodash';

export const HOME_ROUTE = '/';
export const DEMO_ROUTE = '/demo';
export const LOGIN_ROUTE = '/login';
export const REGISTER_ROUTE = '/register';
export const NOT_FOUND_ROUTE = '/not-found';

interface GetLinkProps {
    routeParams?: { [key: string]: string | number },
    queryArgs?: { [key: string]: string | number },
}

interface ComposedRoute {
    template: string,
    getLink: (props: GetLinkProps) => string,
    matches: (comparisonRoute: string) => boolean
}

const createComposedRoute = (template: string): ComposedRoute => {
    const regex = /:\w+(?=\/|$)/g;
    const templateRegex = new RegExp(`^${template.replace(regex, '\\w+')}$`);

    const getLink = (props: GetLinkProps) => {
        const {routeParams, queryArgs} = props;
        let link = '';
        if (!_.isEmpty(routeParams)) {
            link = template.replace(regex, (match) => {
                const param = _.get(routeParams, match.substring(1));
                if (param) {
                    return encodeURIComponent(String(param).replace('.', '%2E'));
                } else {
                    throw Error('Wrong route parameter');
                }
            });
        }
        if (!_.isEmpty(queryArgs)) {
            const search = `?${Object.entries(queryArgs).reduce((str, [key, value]) => `${str}${key}=${value}&`, '')}`.slice(0, -1);
            link += search;
        }
        return link;
    };

    const matches = (comparisonRoute: string) => {
        return !!_.trimEnd(comparisonRoute, '/').match(templateRegex);
    };

    return {
        template,
        getLink,
        matches
    };
};

export const THREAD_ID_PARAM_NAME = 'threadId';

const THREAD_ROUTE_TEMPLATE = `/thread/:${THREAD_ID_PARAM_NAME}`;
export const threadRoute = createComposedRoute(THREAD_ROUTE_TEMPLATE);