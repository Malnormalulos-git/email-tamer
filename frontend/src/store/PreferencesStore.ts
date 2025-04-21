import {create} from 'zustand';
import {devtools} from 'zustand/middleware';
import {z} from 'zod';

const preferencesSchema = z.object({
    language: z.string(),
    messagesPerPage: z.number().min(1).max(100),
});

type Preferences = z.infer<typeof preferencesSchema>;

type PreferencesState = {
    preferences: Preferences;
    setLanguage: (language: string) => void;
    setMessagesPerPage: (messagesPerPage: number) => void;
    setPreferences: (preferences: Partial<Preferences>) => void;
    resetPreferences: () => void;
};

const initialPreferences: Preferences = {
    language: 'en',
    messagesPerPage: 20
};

const savedPreferences = localStorage.getItem('preferences');
const parsedPreferences = savedPreferences
    ? preferencesSchema.safeParse(JSON.parse(savedPreferences))
    : null;

const savePreferences = (preferences: Preferences) => {
    localStorage.setItem('preferences', JSON.stringify(preferences));
};

const usePreferencesStore = create<PreferencesState>()(
    devtools(
        (set) => ({
            preferences: parsedPreferences?.success ? parsedPreferences.data : initialPreferences,
            setLanguage: (language) =>
                set((state) => {
                    const newPreferences = preferencesSchema.parse({
                        ...state.preferences,
                        language: language,
                    });
                    savePreferences(newPreferences);
                    return {preferences: newPreferences};
                }),
            setMessagesPerPage: (messagesPerPage) =>
                set((state) => {
                    const newPreferences = preferencesSchema.parse({
                        ...state.preferences,
                        messagesPerPage: messagesPerPage,
                    });
                    savePreferences(newPreferences);
                    return {preferences: newPreferences};
                }),
            setPreferences: (newPreferences) =>
                set((state) => {
                    const updatedPreferences = preferencesSchema.parse({
                        ...state.preferences,
                        ...newPreferences,
                    });
                    localStorage.setItem('preferences', JSON.stringify(updatedPreferences));
                    return {preferences: updatedPreferences};
                }),
            resetPreferences: () => {
                localStorage.removeItem('preferences');
                set({preferences: initialPreferences});
            },
        }),
        {name: 'preferences-store'}
    )
);

export default usePreferencesStore;