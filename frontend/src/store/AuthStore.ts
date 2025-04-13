import {create} from 'zustand';
import {devtools} from 'zustand/middleware';
import {z} from 'zod';

import {UserDto, UserRole} from '@api/emailTamerApiSchemas.ts';

import isTokenExpired from './isTokenExpired';

const userRolesSchema = z.nativeEnum(UserRole);

const userSchema = z.object({
    id: z.string(),
    email: z.string(),
    role: userRolesSchema,
});

type User = z.infer<typeof userSchema>;

type AuthState = {
    user: User | null;
    token: string | null;
    isAuthenticated: boolean;
    setUser: (user: UserDto | null) => void;
    setToken: (token: string | null) => void;
    logout: () => void;
};

const initialToken = localStorage.getItem('token') || null;
const isInitialTokenValid = initialToken && !isTokenExpired(initialToken);

const useAuthStore = create<AuthState>()(
    devtools(
        (set) => ({
            user: null,
            token: isInitialTokenValid ? initialToken : null,
            isAuthenticated: !!isInitialTokenValid,
            setUser: (userDto: UserDto | null) => {
                const user = userDto ? userSchema.parse(userDto) : null;
                set((state) => ({
                    user,
                    isAuthenticated: !!user && !!state.token && !isTokenExpired(state.token),
                }));
            },
            setToken: (token: string | null) => {
                if (token) {
                    localStorage.setItem('token', token);
                } else {
                    localStorage.removeItem('token');
                }
                set((state) => ({
                    token,
                    isAuthenticated: !!state.user && !!token && !isTokenExpired(token),
                }));
            },
            logout: () => {
                localStorage.removeItem('token');
                set({user: null, token: null, isAuthenticated: false});
            },
        }),
        {name: 'auth-store'}
    )
);

export default useAuthStore;