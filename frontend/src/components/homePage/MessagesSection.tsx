import {Typography} from '@mui/material';
import {useGetMessagesThreads} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {useEffect, useRef, useState} from 'react';
import {IS_BY_DESCENDING_PARAM, PAGE_PARAM, SEARCH_PARAM} from '@router/urlParams.ts';
import ThreadPreview from '@components/homePage/messagesSection/ThreadPreview.tsx';
import PagesHandler from '@components/homePage/messagesSection/PagesHandler.tsx';
import usePreferencesStore from '@store/PreferencesStore.ts';
import {useGetUrlParam, useSetUrlParam} from '@hooks/useUrlParam.ts';

interface MessagesSectionProps {
    selectedFolderId: string | null;
    emailBoxesIds: string[];
}

const MessagesSection = ({selectedFolderId, emailBoxesIds}: MessagesSectionProps) => {
    const pageParam = Number(useGetUrlParam(PAGE_PARAM) || '1');
    const searchTerm = useGetUrlParam(SEARCH_PARAM);
    const isByDescParam = Boolean(JSON.parse(useGetUrlParam(IS_BY_DESCENDING_PARAM) || 'true'));

    const [page, setPage] = useState(pageParam);
    const [isByDescending, setIsByDescending] = useState(isByDescParam);
    const {preferences, setMessagesPerPage: setMessagesPerPageToStore} = usePreferencesStore();
    const [messagesPerPage, setMessagesPerPage] = useState(preferences.messagesPerPage);

    const setUrlParam = useSetUrlParam();

    const prevDepsRef = useRef({
        selectedFolderId,
        emailBoxesIds: [...emailBoxesIds],
        searchTerm,
        isByDescending,
        messagesPerPage
    });

    const handleSetPage = (newPage: number) => {
        setPage(newPage);
        setUrlParam(PAGE_PARAM, newPage);
    };

    const toggleIsByDescending = () => {
        const newValue = !isByDescending;
        setIsByDescending(newValue);
        setUrlParam(IS_BY_DESCENDING_PARAM, newValue);
    };

    useEffect(() => {
        const prevDeps = prevDepsRef.current;
        const depsChanged =
            prevDeps.selectedFolderId !== selectedFolderId ||
            JSON.stringify(prevDeps.emailBoxesIds) !== JSON.stringify(emailBoxesIds) ||
            prevDeps.searchTerm !== searchTerm ||
            prevDeps.isByDescending !== isByDescending ||
            prevDeps.messagesPerPage !== messagesPerPage;

        if (depsChanged && page !== 1) {
            handleSetPage(1);
        }

        prevDepsRef.current = {
            selectedFolderId,
            emailBoxesIds: [...emailBoxesIds],
            searchTerm,
            isByDescending,
            messagesPerPage
        };
    }, [selectedFolderId, emailBoxesIds, searchTerm, isByDescending, messagesPerPage, page]);

    useEffect(() => {
        if (isByDescParam !== isByDescending) {
            setIsByDescending(isByDescParam);
        }
    }, [isByDescParam]);

    useEffect(() => {
        if (pageParam !== page) {
            setPage(pageParam);
        }
    }, [pageParam]);

    const isAnyEmailBoxSelected = emailBoxesIds.length > 0;

    const handleSetMessagesPerPage = (newMessagesPerPage: number) => {
        setMessagesPerPage(newMessagesPerPage);
        setMessagesPerPageToStore(newMessagesPerPage);
    };

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
        enabled: isAnyEmailBoxSelected,
    });

    const {t} = useScopedContextTranslator();

    return (
        <>
            <Typography variant='h6' gutterBottom>
                {t('messages')}
            </Typography>
            {isAnyEmailBoxSelected && messagesThreads?.items?.length !== undefined && messagesThreads?.items?.length > 0
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