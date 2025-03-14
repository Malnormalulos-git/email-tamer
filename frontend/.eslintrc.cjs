
/* global module */
module.exports = {
    env: { browser: true, es2020: true },
    extends: [
        'eslint:recommended',
        'plugin:@typescript-eslint/recommended',
        'plugin:react/recommended',
        'plugin:react-hooks/recommended',
    ],
    parser: '@typescript-eslint/parser',
    parserOptions: { ecmaVersion: 'latest', sourceType: 'module' },
    plugins: ['react', 'react-hooks', 'react-refresh', 'import'],
    rules: {
        'react-refresh/only-export-components': 'warn',
        'react/react-in-jsx-scope': 'off',
        '@typescript-eslint/ban-types': 'off',
        '@typescript-eslint/no-explicit-any': 'off',
        'react-hooks/exhaustive-deps': 'off',
        'prefer-const': 'off',
        'quotes': ['warn', 'single'],
        '@typescript-eslint/no-non-null-assertion': 'off',
        'jsx-quotes': ['warn', 'prefer-single'],
        'indent': ['warn', 4],
        'max-len': ['warn', {'code': 130}],
        'semi': ['error', 'always'],
        'import/order': ['warn', {
            'groups': ['builtin', 'external', 'internal', 'parent', 
                'sibling', 'index', 'object', 'type'],
            'newlines-between': 'always-and-inside-groups'
        }]
    },
};
