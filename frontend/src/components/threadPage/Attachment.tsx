import {useMutation} from '@tanstack/react-query';
import {fetchGetMessageAttachment} from '@api/emailTamerApiComponents';
import {Box, IconButton, Typography, CircularProgress} from '@mui/material';
import {Download} from '@mui/icons-material';
import {getAppControlActions} from '@store/AppControlStore.ts';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {getFileIconUrl} from '@utils/getFileIconUrl.ts';
import {saveAs} from 'file-saver';
import {AttachmentDto} from '@api/emailTamerApiSchemas.ts';

interface AttachmentProps {
    messageId: string;
    attachment: AttachmentDto;
}

const Attachment = ({messageId, attachment}: AttachmentProps) => {
    const {setErrorNotification} = getAppControlActions();
    const {t} = useScopedContextTranslator();

    const {mutate: fetchAttachment, isPending: isAttachmentLoading} = useMutation({
        mutationFn: (variables: { messageId: string; attachment: AttachmentDto }) =>
            fetchGetMessageAttachment({
                queryParams: {messageId: variables.messageId, attachmentId: variables.attachment.id!},
            }),
        onSuccess: (blob, variables) => {
            saveAs(blob, variables.attachment.fileName!);
        },
        onError: () => {
            setErrorNotification(t('failDuringLoadAttachment'));
        },
    });

    const handleDownloadAttachment = () => {
        fetchAttachment({messageId, attachment});
    };

    return (
        <Box
            sx={{
                display: 'flex',
                alignItems: 'center',
                mt: 1,
                p: 0.5,
                borderRadius: 2,
                border: '1px solid',
                borderColor: 'grey.200',
                backgroundColor: 'background.paper',
                transition: 'all 0.2s',
                '&:hover': {
                    backgroundColor: 'grey.50',
                    boxShadow: '0 1px 3px rgba(0,0,0,0.1)',
                },
            }}
        >
            <img
                src={getFileIconUrl(attachment.fileName!, 32)}
                alt={`${attachment.fileName!} icon`}
                style={{width: 24, height: 24, marginRight: 8}}
            />
            <Typography
                variant='body2'
                sx={{
                    wordBreak: 'break-word',
                    flexGrow: 1,
                    color: 'text.primary',
                    '&:hover': {
                        color: 'primary.main',
                        cursor: 'pointer',
                    },
                }}
                onClick={handleDownloadAttachment}
                title={attachment.fileName!}
            >
                {attachment.fileName!}
            </Typography>
            <IconButton
                onClick={handleDownloadAttachment}
                disabled={isAttachmentLoading}
                size='small'
                sx={{
                    ml: 1,
                    color: 'primary.main',
                    '&:hover': {
                        backgroundColor: 'grey.100',
                    },
                }}
            >
                {isAttachmentLoading ? (
                    <CircularProgress size={16}/>
                ) : (
                    <Download fontSize='small'/>
                )}
            </IconButton>
        </Box>
    );
};

export default Attachment;