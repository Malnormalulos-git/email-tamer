import {useEffect, useRef, useState} from 'react';
import {Paper, InputBase, IconButton} from '@mui/material';
import {Search, Clear as ClearIcon} from '@mui/icons-material';
import {useNavigate} from 'react-router-dom';
import {useUrlParam} from '@hooks/useUrlParam.ts';
import {HOME_ROUTE} from '@router/routes.ts';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {SEARCH_PARAM} from '@router/urlParams.ts';

const SearchBar = () => {
    const searchTerm = useUrlParam(SEARCH_PARAM);
    const [searchValue, setSearchValue] = useState('');
    const navigate = useNavigate();
    const inputRef = useRef<HTMLInputElement | null>(null);
    const {t} = useScopedContextTranslator();

    useEffect(() => {
        setSearchValue(searchTerm || '');
    }, [searchTerm]);

    const handleSearch = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if (searchValue) navigate(`${HOME_ROUTE}?${SEARCH_PARAM}=${searchValue}`);
    };

    const handleClear = () => {
        inputRef.current?.focus();
        setSearchValue('');
    };

    return (
        <Paper
            component='form'
            onSubmit={handleSearch}
            sx={{
                p: 0.5,
                display: 'flex',
                alignItems: 'center',
                width: {xs: '100%', sm: '300px', md: '500px'},
                borderRadius: 3,
                transition: 'width 0.3s ease',
                '&:focus-within': {width: {sm: '350px', md: '550px'}},
                boxShadow: '0 1px 2px rgba(0,0,0,0.1)',
            }}
        >
            <InputBase
                inputRef={inputRef}
                sx={{ml: 1, flex: 1, fontSize: {xs: '0.875rem', sm: '1rem'}}}
                placeholder={t('searchBarPlaceholder')}
                value={searchValue}
                onChange={(e) => setSearchValue(e.target.value)}
            />
            {searchValue && (
                <IconButton
                    sx={{p: 1, color: 'text.secondary'}}
                    onClick={handleClear}
                >
                    <ClearIcon fontSize='small'/>
                </IconButton>
            )}
            <IconButton
                type='submit'
                sx={{p: 1, color: 'primary.main'}}
            >
                <Search fontSize='small'/>
            </IconButton>
        </Paper>
    );
};

export default SearchBar;