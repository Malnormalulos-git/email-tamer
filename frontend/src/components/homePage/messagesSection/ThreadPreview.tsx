import {MessagesThreadShortDto} from '@api/emailTamerApiSchemas.ts';
import {Divider, List, ListItem, ListItemButton, ListItemText} from '@mui/material';
import {THREAD_ID_PARAM_NAME, threadRoute} from '@router/routes.ts';
import {formatDateTime} from '@utils/formatDateTime.ts';
import * as React from 'react';
import {useNavigate} from 'react-router-dom';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';

interface ThreadPreviewProps {
    messagesThreads: MessagesThreadShortDto[];
}

const ThreadPreview = ({messagesThreads}: ThreadPreviewProps) => {
    const navigate = useNavigate();
    const {t} = useScopedContextTranslator();

    return (
        <List sx={{width: '100%'}}>
            {messagesThreads.map((thread) => (
                <React.Fragment key={thread.threadId}>
                    <ListItem disablePadding>
                        <ListItemButton onClick={() =>
                            navigate(threadRoute.getLink({
                                routeParams: {
                                    [THREAD_ID_PARAM_NAME]: encodeURIComponent(thread.threadId!.replace('.', '%2E'))
                                }
                            }))}>
                            <ListItemText
                                primary={`${thread.subject || t('noSubject')}
                                                ${thread?.length && thread.length > 1 ? ` (${thread.length})` : ''}`}
                                secondary={
                                    <>
                                        {thread.lastMessage?.participants?.join(', ')}
                                        {', '}
                                        {`${formatDateTime(thread.startDate!)}
                                                  ${thread.startDate! != thread.endDate!
                                            ? ` - ${formatDateTime(thread.endDate!)}`
                                            : ''}`}
                                        <br/>
                                        {thread.lastMessage?.textBody}
                                        {thread.lastMessage?.textBody?.length == 200 ? '...' : ''}
                                        <br/>
                                    </>
                                }
                            />
                        </ListItemButton>
                    </ListItem>
                    <Divider/>
                </React.Fragment>
            ))}
        </List>
    );
};

export default ThreadPreview;