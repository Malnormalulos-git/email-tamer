import {useSearchParams} from 'react-router-dom';

export const useGetUrlParam = (name: string) => {
    const [searchParams] = useSearchParams();
    const value = searchParams.get(name);
    return value ? decodeURIComponent(value) : null;
};

export const useSetUrlParam = () => {
    const [searchParams, setSearchParams] = useSearchParams();

    const setUrlParam = (name: string, value: string | number | boolean | null): void => {
        if (!name) return;

        const updatedParams = new URLSearchParams(searchParams.toString());

        if (value === null)
            updatedParams.delete(name);
        else
            updatedParams.set(name, encodeURIComponent(String(value)));

        setSearchParams(updatedParams);
    };

    return setUrlParam;
};