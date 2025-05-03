export const arraysEqual = <T>(a: T[] | null | undefined, b: T[] | null | undefined): boolean => {
    if (a == null || b == null) {
        return a === b;
    }

    if (a.length !== b.length) {
        return false;
    }

    return a.every((val, index) => val === b[index]);
};