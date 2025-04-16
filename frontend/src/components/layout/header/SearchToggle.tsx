import {useReducer} from 'react';
import {Box, IconButton} from '@mui/material';
import {Search as SearchIcon} from '@mui/icons-material';
import SearchBar from '@components/layout/header/SearchBar.tsx';
import {HEADER_HEIGHT} from '@utils/constants.ts';

const SearchToggle = () => {
    const [searchOpen, toggleSearch] = useReducer((state) => !state, false);

    return (
        <Box sx={{display: {xs: 'block', sm: 'none'}}}>
            <IconButton color='inherit' onClick={toggleSearch}>
                <SearchIcon/>
            </IconButton>
            <Box
                sx={{
                    position: 'absolute',
                    top: HEADER_HEIGHT,
                    left: 0,
                    right: 0,
                    p: 1,
                    bgcolor: 'background.paper',
                    boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                    display: searchOpen ? 'block' : 'none',
                    zIndex: 1000,
                }}
            >
                <SearchBar/>
            </Box>
        </Box>
    );
};

export default SearchToggle;