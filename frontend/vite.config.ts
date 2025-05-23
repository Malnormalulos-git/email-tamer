import { fileURLToPath, URL } from 'url';

import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import eslint from 'vite-plugin-eslint';


export default defineConfig({
    plugins: [react(), eslint()],
    envDir: './environment',
    resolve: {
        alias: [
            {
                find: '@pages',
                replacement: fileURLToPath(new URL('./src/pages', import.meta.url))
            },
            {
                find: '@components',
                replacement: fileURLToPath(new URL('./src/components', import.meta.url))
            },
            {
                find: '@router',
                replacement: fileURLToPath(new URL('./src/router', import.meta.url))
            },
            {
                find: '@api',
                replacement: fileURLToPath(new URL('./src/api', import.meta.url))
            },
            {
                find: '@store',
                replacement: fileURLToPath(new URL('./src/store', import.meta.url))
            },
            {
                find: '@hooks',
                replacement: fileURLToPath(new URL('./src/hooks', import.meta.url))
            },
            {
                find: '@utils',
                replacement: fileURLToPath(new URL('./src/utils', import.meta.url))
            }
        ]
    }
});
