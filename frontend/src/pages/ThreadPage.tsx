import {useNavigate, useParams} from 'react-router-dom';
import {NOT_FOUND_ROUTE, THREAD_ID_PARAM_NAME} from '@router/routes.ts';
import {useGetMessagesThread} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import {Box, List, Typography} from '@mui/material';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import Message from '@components/threadPage/Message';

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

    return (
        <Box sx={{p: 3, maxWidth: '1200px', margin: '0 auto'}}>
            <Typography
                variant='h4'
                sx={{wordWrap: 'break-word', mb: 2}}
            >
                {thread.subject || t('noThreadSubject')}
            </Typography>
            <List sx={{width: '100%'}}>
                {thread.messages?.length !== undefined && thread.messages?.length > 0 &&
                    thread.messages?.map((message) =>
                        <Message messageId={message.id!} messageShort={message} key={message.id}/>
                    )}
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