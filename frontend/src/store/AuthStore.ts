import {z} from 'zod';
import {createStore} from 'zustand/vanilla';
import {devtools} from 'zustand/middleware';
import _ from 'lodash';
import {useStore} from 'zustand';


import {UserDto, UserRole} from '@api/emailTamerApiSchemas.ts';

import {ExtractState} from './types';

const userRolesSchema = z.nativeEnum(UserRole);

const userSchema = z.object({
    id: z.string(),
    email: z.string(),
    role: userRolesSchema
});

type User = z.infer<typeof userSchema>;

type AuthStore = {
    user: User | null,

    actions: {
        setUser: (user: UserDto | null) => void,
        isUserAuthenticated: () => boolean,
        logOutUser: () => void,
        setAccessToken: (token: string | null) => void,
        getAccessToken: () => string | null
    }
}

const authStore = createStore<AuthStore>()(
    devtools((set, get) => ({
        user: null,
        actions: {
            getAccessToken: () => {
                return localStorage.getItem('token');
            },
            setAccessToken: (token: string | null) => {
                if (token) localStorage.setItem('token', token);
            },
            setUser: (userDto: UserDto | null) => {
                const user = userDto ? userSchema.parse(userDto) : null;
                set({user});
            },
            logOutUser: () => {
                set({user: undefined});
                localStorage.removeItem('token');
            },
            isUserAuthenticated: () => {
                const user = get().user;
                const token = localStorage.getItem('token');
                return !_.isEmpty(user) && Boolean(token);
            }
        }
    }),
    {
        name: 'auth-store'
    })
);

const userSelector = (state: ExtractState<typeof authStore>): User | null => state.user;
const actionsSelector = (state: ExtractState<typeof authStore>) => state.actions;

export const getUser = () => userSelector(authStore.getState());
export const getUserActions = () => actionsSelector(authStore.getState());

type UseStoreParams<U> = Parameters<typeof useStore<typeof authStore, U>>;

export function useAuthStore<U>(selector: UseStoreParams<U>[1]) {
    return useStore(authStore, selector);
}