export const getUrlParam = (name: string): string | null => {
    if (!name) return null;
    const urlParams = new URLSearchParams(window.location.search);
    const value = urlParams.get(name);
    return value ? decodeURIComponent(value) : null;
};

export const setUrlParam = (name: string, value: string | number | boolean | null): void => {
    if (!name)
        return;

    const params = new URLSearchParams(window.location.search);

    if (value === null)
        params.delete(name);
    else
        params.set(name, encodeURIComponent(value));

    window.history.pushState({}, '', `${window.location.pathname}?${params.toString()}`);
};