import { createContext, useContext, ReactNode } from 'react';

import joinTranslatorKey from '../utils/joinTranslatorKey';

interface TranslationScopeContextProps {
    scope: string;
}

const TranslationScopeContext = createContext<TranslationScopeContextProps>({
    scope: '',
});

export function TranslationScopeProvider({
    scope,
    children,
    rewriteScope = false
}: {
    scope: string;
    children: ReactNode;
    rewriteScope?: boolean;
}) {
    const parentScope = useContext(TranslationScopeContext).scope;
    const fullScope = parentScope ? joinTranslatorKey(parentScope, scope) : scope;

    return (
        <TranslationScopeContext.Provider value={{ scope: rewriteScope ? scope : fullScope }}>
            {children}
        </TranslationScopeContext.Provider>
    );
}

export function useTranslationScopeContext() {
    return useContext(TranslationScopeContext);
}