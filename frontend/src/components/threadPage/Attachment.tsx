import {useMutation} from '@tanstack/react-query';
import {fetchGetMessageAttachment} from '@api/emailTamerApiComponents';
import {Box, IconButton, Typography, CircularProgress} from '@mui/material';
import {Download} from '@mui/icons-material';
import {getAppControlActions} from '@store/AppControlStore.ts';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {getFileIconUrl} from '@utils/getFileIconUrl.ts';
import {saveAs} from 'file-saver';

interface AttachmentProps {
    messageId: string;
    fileName: string;
}

const Attachment = ({messageId, fileName}: AttachmentProps) => {
    const {setErrorNotification} = getAppControlActions();
    const {t} = useScopedContextTranslator();

    const {mutate: fetchAttachment, isPending: isAttachmentLoading} = useMutation({
        mutationFn: (variables: { messageId: string; fileName: string }) =>
            fetchGetMessageAttachment({
                queryParams: {messageId: variables.messageId, fileName: variables.fileName},
            }),
        onSuccess: (blob, variables) => {
            saveAs(blob, variables.fileName);
        },
        onError: () => {
            setErrorNotification(t('failDuringLoadAttachment'));
        },
    });

    const handleDownloadAttachment = () => {
        fetchAttachment({messageId, fileName});
    };

    return (
        <Box
            sx={{
                display: 'flex',
                alignItems: 'center',
                mt: 1,
                p: 1,
                borderRadius: 1,
                border: '1px solid',
                borderColor: 'grey.200',
                backgroundColor: 'background.paper',
                transition: 'all 0.2s',
                '&:hover': {
                    backgroundColor: 'grey.50',
                    boxShadow: 1,
                },
            }}
        >
            <img
                src={getFileIconUrl(fileName, 32)}
                alt={`${fileName} icon`}
                style={{width: 32, height: 32, marginRight: 8}}
            />
            <Typography
                variant='body2'
                sx={{
                    flexGrow: 1,
                    color: 'text.primary',
                    '&:hover': {
                        color: 'primary.main',
                        cursor: 'pointer',
                    },
                }}
                onClick={handleDownloadAttachment}
                title={fileName}
            >
                {fileName}
            </Typography>
            <IconButton
                onClick={handleDownloadAttachment}
                disabled={isAttachmentLoading}
                size='small'
                sx={{
                    ml: 1,
                    color: 'grey.600',
                    '&:hover': {
                        color: 'primary.main',
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