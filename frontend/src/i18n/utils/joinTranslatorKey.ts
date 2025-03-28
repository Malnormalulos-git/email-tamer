const separator = '.';

export function joinTranslatorKey(...scopes: string[]): string {
    return scopes.filter(Boolean).join(separator);
}

export default joinTranslatorKey;