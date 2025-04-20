import {useReducer} from 'react';
import {useGetMessageDetails} from '@api/emailTamerApiComponents';
import {EmailAddress, MessageDetailsDto, MessageDto} from '@api/emailTamerApiSchemas';
import useScopedContextTranslator from '@hooks/useScopedTranslator';
import {
    Box,
    Collapse,
    Divider,
    ListItem,
    ListItemButton,
    Stack,
    Typography,
} from '@mui/material';
import {formatDateTime} from '@utils/formatDateTime';
import {ExpandMore} from '@mui/icons-material';
import ContentLoading from '@components/ContentLoading';
import Attachment from '@components/threadPage/Attachment.tsx';

interface MessageProps {
    messageId: string;
    messageShort?: MessageDto;
    messageDetailed?: MessageDetailsDto;
}

const Message = ({messageId, messageShort, messageDetailed}: MessageProps) => {
    const {t} = useScopedContextTranslator();
    const [isExpanded, toggleIsExpanded] = useReducer((state) => !state, messageDetailed != undefined);

    const {data: fetchedMessage, isLoading: isDetailsLoading} = useGetMessageDetails(
        {
            queryParams: {
                messageId: encodeURIComponent(messageId),
            },
        },
        {
            enabled: messageDetailed == undefined && isExpanded,
        }
    );

    const message = messageDetailed || fetchedMessage || messageShort;

    const formatAddress = (addresses: EmailAddress[] | null | undefined) => {
        return addresses?.map((address) => address?.name
            ? `${address.name} (${address.address})`
            : address.address ?? '')
            .join(', ') || t('unknown');
    };

    if (!message && isDetailsLoading) {
        return <ContentLoading size={32}/>;
    }

    if (!message) {
        return null;
    }

    const isMessageDetails = (msg: MessageDto | MessageDetailsDto): msg is MessageDetailsDto => {
        return (msg as MessageDetailsDto).from !== undefined;
    };

    return (
        <>
            <ListItem disablePadding>
                <ListItemButton onClick={toggleIsExpanded}>
                    <Stack sx={{width: '100%'}}>
                        <Stack
                            direction='row'
                            sx={{
                                justifyContent: 'space-between',
                                alignItems: 'center'
                            }}
                        >
                            <Typography>
                                {message.subject || t('noSubject')}
                            </Typography>
                            <Typography variant='body2' color='text.secondary'>
                                {formatDateTime(message.date!)}
                            </Typography>
                        </Stack>
                        {!isExpanded && <Typography variant='body2' color='text.secondary'>
                            {message.textBody?.substring(0, 100)}
                            {message.textBody?.substring(0, 100).length === 100 ? '...' : ''}
                        </Typography>}
                    </Stack>
                    <ExpandMore
                        sx={{
                            transform: isExpanded ? 'rotate(180deg)' : 'rotate(0deg)',
                            transition: 'transform 0.3s ease',
                            ml: 1
                        }}
                    />
                </ListItemButton>
            </ListItem>
            <Collapse in={isExpanded} timeout='auto' unmountOnExit>
                <Box sx={{pl: 4, pr: 2, backgroundColor: 'background.default'}}>
                    {isDetailsLoading ? (
                        <ContentLoading size={32}/>
                    ) : isMessageDetails(message) ? (
                        <>
                            <Typography variant='body2' color='text.secondary'>
                                {t('from')} {formatAddress(message.from)}
                            </Typography>
                            <Typography variant='body2' color='text.secondary'>
                                {t('to')} {formatAddress(message.to)}
                            </Typography>
                            {message.htmlBody && message.textBody ? (
                                <Box
                                    sx={{my: 2, border: '1px solid #e0e0e0', p: 2, borderRadius: 2}}
                                    dangerouslySetInnerHTML={{__html: message.htmlBody}}
                                />
                            ) : (
                                <Typography sx={{my: 2, whiteSpace: 'pre-wrap'}}>
                                    {message.textBody || t('emptyBody')}
                                </Typography>
                            )}
                        </>
                    ) : null}
                </Box>
                {isMessageDetails(message) && message.attachmentFilesNames && message.attachmentFilesNames.length > 0 ? (
                    <Box sx={{mt: 2}}>
                        <Typography  variant='body2' color='text.secondary'>{t('attachments')}:</Typography>
                        {message.attachmentFilesNames.map((fileName: string) => (
                            <Attachment
                                key={fileName}
                                messageId={messageId}
                                fileName={fileName}
                            />
                        ))}
                    </Box>
                ) : null}
            </Collapse>
            <Divider sx={{mt: isExpanded ? 1 : 0}}/>
        </>
    );
};

export default Message;