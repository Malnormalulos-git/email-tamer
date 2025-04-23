import {useNavigate, useParams} from 'react-router-dom';
import {HOME_ROUTE, NOT_FOUND_ROUTE, THREAD_ID_PARAM_NAME} from '@router/routes.ts';
import {useGetMessagesThread} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import {Box, IconButton, List, Stack, Typography} from '@mui/material';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import Message from '@components/threadPage/Message';
import {ArrowBack} from '@mui/icons-material';
import {formatDateTime} from '@utils/formatDateTime.ts';

const ThreadPage = () => {
    const {[THREAD_ID_PARAM_NAME]: threadId} = useParams();
    const decodedThreadId = threadId ? decodeURIComponent(threadId).replace('%2E', '.') : '';
    const {t} = useScopedContextTranslator();
    const navigate = useNavigate();

    const {data: thread, isLoading: isThreadLoading, error: threadError} = useGetMessagesThread(
        {
            queryParams: {
                messageId: encodeURIComponent(decodedThreadId!),
            },
        },
        {
            enabled: decodedThreadId.length > 0,
        }
    );

    if (isThreadLoading) {
        return <ContentLoading sx={{mt: 10}}/>;
    }

    if (threadError !== null || thread?.lastMessage === undefined) {
        navigate(NOT_FOUND_ROUTE);
        return null;
    }

    const handleBackButtonClick = () => {
        if (history.length) {
            history.back();
            return;
        }
        navigate(HOME_ROUTE);
    };

    const messagesCount = (thread.messages?.length ?? 0) + 1;
    const dateRange =
        thread.messages && thread.messages.length > 0 && thread.lastMessage
            ? `${formatDateTime(thread.messages[0].date!)} - ${formatDateTime(thread.lastMessage.date!)}`
            : thread.lastMessage
                ? formatDateTime(thread.lastMessage.date!)
                : '';

    return (
        <Box sx={{p: {xs: 2, sm: 3}, maxWidth: '1200px', margin: '0 auto'}}>
            <Box sx={{
                position: 'sticky',
                top: 0,
                zIndex: 1,
                backgroundColor: 'background.paper',
                py: 2,
                boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
            }}>
                <Stack direction='row' alignItems='center' spacing={2}>
                    <IconButton onClick={handleBackButtonClick} sx={{p: 1}} title={t('back')}>
                        <ArrowBack fontSize='medium' sx={{color: 'primary.main'}}/>
                    </IconButton>
                    <Box>
                        <Typography
                            variant='h5'
                            sx={{wordWrap: 'break-word'}}>
                            {thread.subject || t('noThreadSubject')}
                        </Typography>
                        <Typography variant='body2' color='text.secondary'>
                            {messagesCount} {t('messages')} • {dateRange}
                        </Typography>
                    </Box>
                </Stack>
            </Box>
            <List sx={{width: '100%', maxHeight: '70vh', overflowY: 'auto', mt: 2}}>
                {thread.messages?.length && thread.messages.length > 0 ? (
                    thread.messages.map((message) => (
                        <Message messageId={message.id!} messageShort={message} key={message.id}/>
                    ))
                ) : null}
                {thread.lastMessage && (
                    <Message
                        messageId={thread.lastMessage.id!}
                        messageDetailed={thread.lastMessage}
                        key={thread.lastMessage.id}
                    />
                )}
            </List>
        </Box>
    );
};

export default ThreadPage;