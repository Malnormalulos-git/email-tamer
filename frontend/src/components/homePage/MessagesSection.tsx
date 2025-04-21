import {Typography} from '@mui/material';
import {useGetMessagesThreads} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {useReducer, useState} from 'react';
import {SEARCH_PARAM} from '@router/urlParams.ts';
import {useUrlParam} from '@hooks/useUrlParam';
import ThreadPreview from '@components/homePage/messagesSection/ThreadPreview.tsx';
import PagesHandler from '@components/homePage/messagesSection/PagesHandler.tsx';
import usePreferencesStore from '@store/PreferencesStore.ts';

interface MessagesSectionProps {
    selectedFolderId: string | null;
    emailBoxesIds: string[];
}

const MessagesSection = ({selectedFolderId, emailBoxesIds}: MessagesSectionProps) => {
    const [page, setPage] = useState(1);

    const {preferences, setMessagesPerPage: setMessagesPerPageToStore} = usePreferencesStore();
    const [messagesPerPage, setMessagesPerPage] = useState(preferences.messagesPerPage);

    const [isByDescending, toggleIsByDescending] = useReducer((state) => {
        setPage(1);
        return !state;
    }, true);

    const searchTerm = useUrlParam(SEARCH_PARAM);

    const isAnyEmaiBoxSelected = emailBoxesIds.length > 0;

    const {data: messagesThreads, isLoading} = useGetMessagesThreads({
        queryParams: {
            folderId: selectedFolderId ?? undefined,
            emailBoxesIds: emailBoxesIds.length > 0 ? emailBoxesIds.join(', ') : undefined,
            page,
            size: messagesPerPage,
            isByDescending: isByDescending,
            ...(searchTerm?.length !== undefined && searchTerm?.length > 0 && {searchTerm}),
        },
    },
    {
        enabled: isAnyEmaiBoxSelected,
    }
    );

    const {t} = useScopedContextTranslator();

    const handleSetMessagesPerPage = (messagesPerPage: number) => {
        setMessagesPerPage(messagesPerPage);
        setMessagesPerPageToStore(messagesPerPage);
    };

    return (
        <>
            <Typography variant='h6' gutterBottom>
                {t('messages')}
            </Typography>
            {isAnyEmaiBoxSelected && messagesThreads?.items?.length !== undefined && messagesThreads?.items?.length > 0
                ? <>
                    <PagesHandler
                        isByDescending={isByDescending}
                        toggleIsByDescending={toggleIsByDescending}
                        page={page}
                        setPage={setPage}
                        messagesPerPage={messagesPerPage}
                        setMessagesPerPage={handleSetMessagesPerPage}
                        totalMessages={messagesThreads?.total}
                    />
                    {isLoading
                        ? <ContentLoading/>
                        : <ThreadPreview messagesThreads={messagesThreads.items}/>
                    }
                </>
                : <Typography variant='body1'>
                    {t('noMessagesFound')}
                </Typography>
            }
        </>
    );
};

export default MessagesSection;