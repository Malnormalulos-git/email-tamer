import i18n from 'i18next';
import {initReactI18next} from 'react-i18next';

import en from './locales/en.json';
import ua from './locales/ua.json';

export const defaultNS = 'translation';
export const resources = {
    en: {translation: en},
    ua: {translation: ua},
} as const;

i18n.use(initReactI18next).init({
    debug: true,
    fallbackLng: 'en',
    resources: resources,
    defaultNS,
    interpolation: {
        escapeValue: false,
    },
});

export default i18n;
