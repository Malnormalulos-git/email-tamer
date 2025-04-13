import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {getAppControlActions} from '@store/AppControlStore';
import {useState} from 'react';
import {useDeleteEmailBox} from '@api/emailTamerApiComponents.ts';
import {Button, MenuItem, Typography} from '@mui/material';
import EmailTamerDialog from '@components/forms/EmailTamerDialog';
import {EmailBoxDto} from '@api/emailTamerApiSchemas.ts';
import {formatString} from '@utils/formatString.ts';

interface DeleteEmailBoxMenuItemProps {
    box: EmailBoxDto;
    refetch: () => void;
    onCloseMenu: () => void;
}

const DeleteEmailBoxMenuItem = ({box, refetch, onCloseMenu}: DeleteEmailBoxMenuItemProps) => {
    const {t} = useScopedContextTranslator();
    const {setErrorNotification, setSuccessNotification} = getAppControlActions();
    const [openConfirmDialog, setOpenConfirmDialog] = useState(false);

    const {mutate: deleteEmailBox, isPending} = useDeleteEmailBox({
        onSuccess: () => {
            setSuccessNotification(t('success'));
            refetch();
            onCloseMenu();
            setOpenConfirmDialog(false);
        },
        onError: () => {
            setErrorNotification(t('error'));
        },
    });

    const handleOpenConfirmDialog = () => {
        setOpenConfirmDialog(true);
    };

    const handleCloseConfirmDialog = () => {
        setOpenConfirmDialog(false);
    };

    const handleDelete = () => {
        deleteEmailBox({
            pathParams: {id: box.id!},
        });
    };

    return (
        <>
            <MenuItem onClick={handleOpenConfirmDialog}>
                {t('delete')}
            </MenuItem>
            <EmailTamerDialog
                title={t('confirmDelete')}
                open={openConfirmDialog}
                onClose={handleCloseConfirmDialog}
                dialogActions={
                    <>
                        <Button
                            variant='outlined'
                            onClick={handleCloseConfirmDialog}
                            disabled={isPending}
                            fullWidth
                            sx={{mt: 2}}
                        >
                            {t('cancel')}
                        </Button>
                        <Button
                            color='error'
                            onClick={handleDelete}
                            disabled={isPending}
                            fullWidth
                            variant='contained'
                            sx={{mt: 2}}
                        >
                            {t('delete')}
                        </Button>
                    </>
                }
            >
                <Typography>
                    {formatString(t('confirmDeleteMessage'), box.boxName!)}
                </Typography>
            </EmailTamerDialog>
        </>
    );
};

export default DeleteEmailBoxMenuItem;