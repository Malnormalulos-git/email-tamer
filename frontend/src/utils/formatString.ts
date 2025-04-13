export function formatString (stringToFormat : string, ...args: number[] | string[]): string {
    return stringToFormat.replace(/{(\d+)}/g, function(match, number) {
        return typeof args[number] != 'undefined'
            ? args[number].toString()
            : match;
    });
}