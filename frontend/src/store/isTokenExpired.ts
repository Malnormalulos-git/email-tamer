import {jwtDecode} from 'jwt-decode';

interface JwtPayload {
    exp: number;
}

const isTokenExpired = (token: string | null): boolean => {
    if (!token) return true;
    try {
        const decoded = jwtDecode<JwtPayload>(token);
        const currentTime = Date.now() / 1000;
        return decoded.exp < currentTime;
    } catch (error) {
        return true;
    }
};

export default isTokenExpired;