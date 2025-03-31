import {useMemo} from 'react';
import {useTranslation} from 'react-i18next';

import joinTranslatorKey from '../i18n/utils/joinTranslatorKey.ts';
import {useTranslationScopeContext} from '../i18n/contexts/TranslationScopeContext.tsx';

export type ScopedContextTranslatorFunction = {
    t: (key: string, options?: Record<string, any>) => string;
    scope: string;
};

export function useScopedContextTranslator(): ScopedContextTranslatorFunction {
    const {t, i18n} = useTranslation();
    const {scope} = useTranslationScopeContext();

    return useMemo(() => {
        const translator = (key: string, options?: Record<string, any>) =>
            t(joinTranslatorKey(scope, key), options);

        return {t: translator, scope};
    }, [scope, t, i18n.language]);
}

export default useScopedContextTranslator;