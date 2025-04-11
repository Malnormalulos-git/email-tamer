import {Grid2} from '@mui/material';
import {useState} from 'react';
import FoldersSection from '@components/homePage/FoldersSection.tsx';

import EmailBoxesSection from '@components/homePage/EmailBoxesSection.tsx';

import MessagesSection from '@components/homePage/MessagesSection.tsx';

import {HEADER_HEIGHT} from '@utils/constants.ts';

import {TranslationScopeProvider} from '../i18n/contexts/TranslationScopeContext.tsx';

const HomePage = () => {
    const [selectedFolderId, setSelectedFolderId] = useState<string | null>(null);
    const [selectedEmailBoxesIds, setSelectedEmailBoxesIds] = useState<string[]>([]);

    return (
        <Grid2 container>
            <Grid2
                size={2}
                sx={{
                    borderRight: '1px solid #e0e0e0',
                    p: 2,
                    position: 'sticky',
                    top: HEADER_HEIGHT,
                    height: `calc(100vh - ${HEADER_HEIGHT}px)`,
                    overflowY: 'auto',
                }}
            >
                <TranslationScopeProvider scope='foldersSection'>
                    <FoldersSection
                        selectedFolderId={selectedFolderId}
                        setSelectedFolderId={setSelectedFolderId}
                    />
                </TranslationScopeProvider>
            </Grid2>

            <Grid2
                size={7}
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
                size={3}
                sx={{
                    borderLeft: '1px solid #e0e0e0',
                    p: 2,
                    position: 'sticky',
                    top: HEADER_HEIGHT,
                    height: `calc(100vh - ${HEADER_HEIGHT}px)`,
                    overflowY: 'auto',
                }}
            >
                <TranslationScopeProvider scope='emailBoxesSection'>
                    <EmailBoxesSection
                        emailBoxesIds={selectedEmailBoxesIds}
                        setEmailBoxesIds={setSelectedEmailBoxesIds}
                    />
                </TranslationScopeProvider>
            </Grid2>
        </Grid2>
    );
};

export default HomePage;