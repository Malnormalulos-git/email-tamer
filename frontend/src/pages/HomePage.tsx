import {Grid2} from '@mui/material';
import {useState} from 'react';
import FoldersSection from '@components/homePage/FoldersSection.tsx';

import EmailBoxesSection from '@components/homePage/EmailBoxesSection.tsx';

import MessagesSection from '@components/homePage/MessagesSection.tsx';

import {TranslationScopeProvider} from '../i18n/contexts/TranslationScopeContext.tsx';

const HomePage = () => {
    const [selectedFolderId, setSelectedFolderId] = useState<string | null>(null);
    const [selectedEmailBoxesIds, setSelectedEmailBoxesIds] = useState<string[]>([]);

    return (
        <Grid2 container sx={{height: 'calc(100vh - 64px)', mt: 2}}>
            <Grid2 size={2} sx={{borderRight: '1px solid #e0e0e0', p: 2}}>
                <TranslationScopeProvider scope='foldersSection'>
                    <FoldersSection
                        selectedFolderId={selectedFolderId}
                        setSelectedFolderId={setSelectedFolderId}
                    />
                </TranslationScopeProvider>
            </Grid2>

            <Grid2 size={6} sx={{p: 2, overflowY: 'auto'}}>
                <TranslationScopeProvider scope='messagesSection'>
                    <MessagesSection
                        selectedFolderId={selectedFolderId}
                        emailBoxesIds={selectedEmailBoxesIds}
                    />
                </TranslationScopeProvider>
            </Grid2>

            <Grid2 size={4} sx={{borderLeft: '1px solid #e0e0e0', p: 2}}>
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