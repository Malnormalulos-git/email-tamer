import {MessagesThreadShortDto} from '@api/emailTamerApiSchemas.ts';
import {THREAD_ID_PARAM_NAME, threadRoute} from '@router/routes.ts';
import {formatDateTime} from '@utils/formatDateTime.ts';
import {useNavigate} from 'react-router-dom';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import GenericEmailTamerList from '@components/GenericEmailTamerList.tsx';

interface ThreadPreviewProps {
    messagesThreads: MessagesThreadShortDto[];
}

const ThreadPreview = ({messagesThreads}: ThreadPreviewProps) => {
    const navigate = useNavigate();
    const {t} = useScopedContextTranslator();

    const items = messagesThreads.map((thread) => ({
        id: thread.threadId!,
        label: `${thread.subject || t('noSubject')}${thread?.length && thread.length > 1 ? ` (${thread.length})` : ''}`,
        onClick: () =>
            navigate(
                threadRoute.getLink({
                    routeParams: {
                        [THREAD_ID_PARAM_NAME]: encodeURIComponent(thread.threadId!.replace('.', '%2E')),
                    },
                })
            ),
        secondary: (
            <>
                {thread.lastMessage?.participants?.join(', ')}
                {', '}
                {`${formatDateTime(thread.startDate!)}${thread.startDate! != thread.endDate! 
                    ? ` - ${formatDateTime(thread.endDate!)}` 
                    : ''}`}
                <br/>
                {thread.lastMessage?.textBody}
                {thread.lastMessage?.textBody?.length == 200 ? '...' : ''}
                <br/>
            </>
        ),
    }));

    return <GenericEmailTamerList items={items} showDivider sx={{width: '100%'}}/>;
};

export default ThreadPreview;