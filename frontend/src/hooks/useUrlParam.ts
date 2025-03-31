import {useLocation} from 'react-router-dom';

type UseUrlParam = (name: string) => string | null;

export const useUrlParam: UseUrlParam = (name) => {
    const location = useLocation();
    const queryParams = new URLSearchParams(location.search);
    return queryParams.get(name);
};