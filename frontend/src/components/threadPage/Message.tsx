import {useReducer} from 'react';
import {useGetMessageDetails} from '@api/emailTamerApiComponents';
import {EmailAddress, MessageDetailsDto, MessageDto} from '@api/emailTamerApiSchemas';
import useScopedContextTranslator from '@hooks/useScopedTranslator';
import {
    Avatar,
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
            <ListItem disablePadding sx={{
                mb: 2,
                borderRadius: 2,
                backgroundColor: 'background.paper',
                boxShadow: '0 1px 3px rgba(0,0,0,0.2)',
                '&:hover': {backgroundColor: 'grey.50'}
            }}>
                <ListItemButton onClick={toggleIsExpanded}>
                    <Stack direction='row' spacing={1}
                        sx={{width: '100%', alignItems: 'center', p: 1}}>
                        <Avatar sx={{
                            width: 40,
                            height: 40,
                            display: {xs: 'none', sm: 'flex'}
                        }}/>
                        <Stack sx={{flexGrow: 1}}>
                            <Stack direction='row' justifyContent='space-between' alignItems='center'>
                                <Typography variant='subtitle1' fontWeight='medium'
                                    sx={{fontSize: {xs: '0.9rem', sm: '1rem'}}}>
                                    {message.subject || t('noSubject')}
                                </Typography>
                                <Typography variant='caption' color='text.secondary'
                                    sx={{fontSize: {xs: '0.75rem', sm: '0.875rem'}}}>
                                    {formatDateTime(message.date!)}
                                </Typography>
                            </Stack>
                            {!isExpanded && (
                                <Typography variant='body2' color='text.secondary'
                                    sx={{mt: 0.5, fontSize: {xs: '0.8rem', sm: '0.875rem'}}}>
                                    {message.textBody?.substring(0, 100)}
                                    {message.textBody?.substring(0, 100).length === 100 ? '...' : ''}
                                </Typography>
                            )}
                        </Stack>
                        <ExpandMore
                            sx={{
                                transform: isExpanded ? 'rotate(180deg)' : 'rotate(0deg)',
                                transition: 'transform 0.3s ease',
                                color: 'primary.main',
                            }}
                        />
                    </Stack>
                </ListItemButton>
            </ListItem>
            <Collapse in={isExpanded} timeout={500} unmountOnExit>
                <Box sx={{p: 2, backgroundColor: 'grey.100', borderRadius: '0 0 8px 8px'}}>
                    {isDetailsLoading ? (
                        <ContentLoading size={32}/>
                    ) : isMessageDetails(message) ? (
                        <>
                            <Typography variant='body2' color='text.secondary' sx={{mb: 1}}>
                                {t('from')} {formatAddress(message.from)}
                            </Typography>
                            <Typography variant='body2' color='text.secondary' sx={{mb: 1}}>
                                {t('to')} {formatAddress(message.to)}
                            </Typography>
                            {message.htmlBody && message.textBody ? (
                                <Box
                                    sx={{my: 2, border: '1px solid', borderColor: 'grey.300', p: 2, borderRadius: 2}}
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
                    <Box sx={{mt: 2, p: 2}}>
                        <Typography variant='body2' fontWeight='medium' sx={{mb: 1}}>
                            {t('attachments')}
                        </Typography>
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