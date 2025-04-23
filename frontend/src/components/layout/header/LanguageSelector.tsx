import {useState} from 'react';
import {IconButton, Menu, MenuItem} from '@mui/material';

import usePreferencesStore from '@store/PreferencesStore.ts';

import i18n from '../../../i18n/i18n.ts';

interface LanguageSelectorProps {
    onLanguageChange?: () => void;
}

const LanguageSelector = ({onLanguageChange}: LanguageSelectorProps) => {
    const [langMenuAnchor, setLangMenuAnchor] = useState<null | HTMLElement>(null);
    const open = Boolean(langMenuAnchor);

    const {setLanguage} = usePreferencesStore();

    const handleLangMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
        setLangMenuAnchor(event.currentTarget);
    };

    const handleLangMenuClose = () => {
        setLangMenuAnchor(null);
    };

    const handleLanguageChange = (lang: string) => {
        i18n.changeLanguage(lang);
        setLanguage(lang);
        handleLangMenuClose();
        if (onLanguageChange) onLanguageChange();
    };

    return (
        <>
            <IconButton
                onClick={handleLangMenuOpen}
            >
                <img
                    src={
                        i18n.language === 'en'
                            ? 'https://flagsapi.com/GB/flat/24.png'
                            : 'https://flagsapi.com/UA/flat/24.png'
                    }
                    alt={i18n.language}
                />
            </IconButton>
            {open && <Menu
                anchorEl={langMenuAnchor}
                open={open}
                onClose={handleLangMenuClose}
            >
                <MenuItem onClick={() => handleLanguageChange('en')}>
                    <img
                        src='https://flagsapi.com/GB/flat/24.png'
                        alt='English'
                        style={{marginRight: 8}}
                    />
                    EN
                </MenuItem>
                <MenuItem onClick={() => handleLanguageChange('ua')}>
                    <img
                        src='https://flagsapi.com/UA/flat/24.png'
                        alt='Ukrainian'
                        style={{marginRight: 8}}
                    />
                    УКР
                </MenuItem>
            </Menu>}
        </>
    );
};

export default LanguageSelector;