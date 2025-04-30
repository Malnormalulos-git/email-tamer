import {Typography} from '@mui/material';
import {useGetMessagesThreads} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {useEffect, useReducer, useState} from 'react';
import {IS_BY_DESCENDING_PARAM, PAGE_PARAM, SEARCH_PARAM} from '@router/urlParams.ts';
import ThreadPreview from '@components/homePage/messagesSection/ThreadPreview.tsx';
import PagesHandler from '@components/homePage/messagesSection/PagesHandler.tsx';
import usePreferencesStore from '@store/PreferencesStore.ts';
import {getUrlParam, setUrlParam} from '@utils/urlUtils.ts';

interface MessagesSectionProps {
    selectedFolderId: string | null;
    emailBoxesIds: string[];
}

const MessagesSection = ({selectedFolderId, emailBoxesIds}: MessagesSectionProps) => {
    const pageParam = Number(getUrlParam(PAGE_PARAM || '1'));

    const [page, setPage] = useState(pageParam);
    const handleSetPage = (page: number) => {
        setUrlParam(PAGE_PARAM, page);
        setPage(page);
    };
    useEffect(() => {
        if(page !== 1)
            handleSetPage(1);
    }, [selectedFolderId, emailBoxesIds]);

    const {preferences, setMessagesPerPage: setMessagesPerPageToStore} = usePreferencesStore();
    const [messagesPerPage, setMessagesPerPage] = useState(preferences.messagesPerPage);

    const isByDescParam = Boolean(JSON.parse(getUrlParam(IS_BY_DESCENDING_PARAM) || 'true'));

    const [isByDescending, toggleIsByDescending] = useReducer((state) => {
        handleSetPage(1);
        setUrlParam(IS_BY_DESCENDING_PARAM, !state);
        return !state;
    }, isByDescParam);

    const searchTerm = getUrlParam(SEARCH_PARAM);

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
    });

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
                        setPage={handleSetPage}
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