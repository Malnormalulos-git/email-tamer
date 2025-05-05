export const getUrlParam = (name: string): string | null => {
    if (!name) return null;
    const urlParams = new URLSearchParams(window.location.search);
    const value = urlParams.get(name);
    return value ? decodeURIComponent(value) : null;
};

