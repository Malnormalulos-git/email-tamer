import {AppBar, Box, Grid2, IconButton, SwipeableDrawer, Toolbar} from '@mui/material';
import {useReducer, useState, useEffect} from 'react';
import FoldersSection from '@components/homePage/FoldersSection.tsx';
import EmailBoxesSection from '@components/homePage/EmailBoxesSection.tsx';
import MessagesSection from '@components/homePage/MessagesSection.tsx';
import {HEADER_HEIGHT} from '@utils/constants.ts';
import {Email as EmailIcon, Folder as FolderIcon} from '@mui/icons-material';
import {getUrlParam} from '@utils/urlUtils.ts';
import {SELECTED_BOXES_IDS_PARAM, SELECTED_FOLDER_ID_PARAM} from '@router/urlParams.ts';

import {arraysEqual} from '@utils/arraysEqual.ts';

import {useGetUrlParam, useSetUrlParam} from '@hooks/useUrlParam.ts';

import {TranslationScopeProvider} from '../i18n/contexts/TranslationScopeContext.tsx';

const APPBAR_HEIGHT = 50;

const HomePage = () => {
    const [foldersDrawerOpen, toggleFoldersDrawer] = useReducer((state) => !state, false);
    const [emailBoxesDrawerOpen, toggleEmailBoxesDrawer] = useReducer((state) => !state, false);
    const setUrlParam = useSetUrlParam();

    const folderIdParam = useGetUrlParam(SELECTED_FOLDER_ID_PARAM);
    const [selectedFolderId, setSelectedFolderId] = useState<string | null>(folderIdParam);
    const handleSetSelectedFolderId = (folderId: string | null) => {
        setUrlParam(SELECTED_FOLDER_ID_PARAM, folderId);
        setSelectedFolderId(folderId);
    };

    const emailBoxesIdsParam = useGetUrlParam(SELECTED_BOXES_IDS_PARAM)?.split(',').filter(id => id) || [];
    const [selectedEmailBoxesIds, setSelectedEmailBoxesIds] = useState<string[]>(emailBoxesIdsParam);

    useEffect(() => {
        const emailBoxesIdsParam = getUrlParam(SELECTED_BOXES_IDS_PARAM)?.split(',').filter(id => id) || [];
        if (!arraysEqual(emailBoxesIdsParam, selectedEmailBoxesIds)) {
            setSelectedEmailBoxesIds(emailBoxesIdsParam);
        }
    }, []);

    const handleSetSelectedEmailBoxesIds = (emailBoxesIds: string[]) => {
        const newIds = emailBoxesIds.length > 0 ? emailBoxesIds.join(',') : null;
        const currentIds = getUrlParam(SELECTED_BOXES_IDS_PARAM);
        if (newIds !== currentIds) {
            setUrlParam(SELECTED_BOXES_IDS_PARAM, newIds);
        }
        if (!arraysEqual(selectedEmailBoxesIds, emailBoxesIds)) {
            setSelectedEmailBoxesIds(emailBoxesIds);
        }
    };

    return (
        <>
            <AppBar
                position='fixed'
                color='default'
                sx={{
                    top: HEADER_HEIGHT,
                    display: {xs: 'block', md: 'none'},
                    boxShadow: 'none',
                    borderBottom: '1px solid #e0e0e0',
                    height: APPBAR_HEIGHT,
                    zIndex: 1000,
                }}
            >
                <Toolbar variant='dense' sx={{justifyContent: 'space-between'}}>
                    <IconButton edge='start' onClick={toggleFoldersDrawer}>
                        <FolderIcon/>
                    </IconButton>
                    <IconButton edge='end' onClick={toggleEmailBoxesDrawer}>
                        <EmailIcon/>
                    </IconButton>
                </Toolbar>
            </AppBar>

            <Grid2 container mt={{xs: `${APPBAR_HEIGHT}px`, md: 0}}>
                <Grid2
                    size={{md: 2, lg: 2}}
                    sx={{
                        borderRight: '1px solid #e0e0e0',
                        p: 2,
                        position: 'sticky',
                        top: HEADER_HEIGHT,
                        height: `calc(100vh - ${HEADER_HEIGHT}px)`,
                        overflowY: 'auto',
                        display: {xs: 'none', md: 'block'},
                    }}
                >
                    <TranslationScopeProvider scope='foldersSection'>
                        <FoldersSection
                            selectedFolderId={selectedFolderId}
                            setSelectedFolderId={handleSetSelectedFolderId}
                        />
                    </TranslationScopeProvider>
                </Grid2>

                <Grid2
                    size={{md: 6, lg: 7}}
                    sx={{
                        p: 2,
                        overflowY: 'auto'
                    }}
                >
                    <TranslationScopeProvider scope='messagesSection'>
                        <MessagesSection
                            selectedFolderId={selectedFolderId}
                            emailBoxesIds={selectedEmailBoxesIds}
                        />
                    </TranslationScopeProvider>
                </Grid2>

                <Grid2
                    size={{md: 4, lg: 3}}
                    sx={{
                        borderLeft: '1px solid #e0e0e0',
                        p: 2,
                        position: 'sticky',
                        top: HEADER_HEIGHT,
                        height: `calc(100vh - ${HEADER_HEIGHT}px)`,
                        overflowY: 'auto',
                        display: {xs: 'none', md: 'block'},
                    }}
                >
                    <TranslationScopeProvider scope='emailBoxesSection'>
                        <EmailBoxesSection
                            emailBoxesIds={selectedEmailBoxesIds}
                            setEmailBoxesIds={handleSetSelectedEmailBoxesIds}
                        />
                    </TranslationScopeProvider>
                </Grid2>
            </Grid2>

            <SwipeableDrawer
                anchor='left'
                open={foldersDrawerOpen}
                onClose={toggleFoldersDrawer}
                onOpen={toggleFoldersDrawer}
                sx={{
                    '& .MuiDrawer-paper': {
                        width: {xs: 250, sm: 350},
                        boxSizing: 'border-box',
                        top: HEADER_HEIGHT,
                        height: `calc(100% - ${HEADER_HEIGHT}px)`
                    },
                    display: {sm: 'block', md: 'none'}
                }}
                variant='temporary'
            >
                <Box sx={{p: 2}}>
                    <TranslationScopeProvider scope='foldersSection'>
                        <FoldersSection
                            selectedFolderId={selectedFolderId}
                            setSelectedFolderId={(id) => {
                                handleSetSelectedFolderId(id);
                                toggleFoldersDrawer();
                            }}
                        />
                    </TranslationScopeProvider>
                </Box>
            </SwipeableDrawer>
            <SwipeableDrawer
                anchor='right'
                open={emailBoxesDrawerOpen}
                onClose={toggleEmailBoxesDrawer}
                onOpen={toggleEmailBoxesDrawer}
                sx={{
                    '& .MuiDrawer-paper': {
                        width: {xs: 250, sm: 350},
                        boxSizing: 'border-box',
                        top: HEADER_HEIGHT,
                        height: `calc(100% - ${HEADER_HEIGHT}px)`,
                        display: {sm: 'block', md: 'none'}
                    },
                }}
                variant='temporary'
            >
                <Box sx={{p: 2}}>
                    <TranslationScopeProvider scope='emailBoxesSection'>
                        <EmailBoxesSection
                            emailBoxesIds={selectedEmailBoxesIds}
                            setEmailBoxesIds={handleSetSelectedEmailBoxesIds}
                        />
                    </TranslationScopeProvider>
                </Box>
            </SwipeableDrawer>
        </>
    );
};

export default HomePage;