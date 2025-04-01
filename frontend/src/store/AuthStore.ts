import {create} from 'zustand';
import {devtools} from 'zustand/middleware';
import {z} from 'zod';

import {UserDto, UserRole} from '@api/emailTamerApiSchemas.ts';

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

const useAuthStore = create<AuthState>()(
    devtools(
        (set) => ({
            user: null,
            token: initialToken,
            isAuthenticated: !!initialToken,
            setUser: (userDto: UserDto | null) => {
                const user = userDto ? userSchema.parse(userDto) : null;
                set((state) => ({
                    user,
                    isAuthenticated: !!user && !!state.token,
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
                    isAuthenticated: !!state.user && !!token,
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