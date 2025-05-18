import {Box, Button, Checkbox, Fab, Stack, Typography} from '@mui/material';
import {useBackUpEmailBoxesMessages, useGetEmailBoxes, useGetEmailBoxesStatuses} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {Add, Cloud, CloudDownload, CloudSync, Warning} from '@mui/icons-material';
import {useEffect, useState} from 'react';
import AddEmailBoxDialogForm from '@components/emailBox/AddEmailBoxDialogForm.tsx';
import EmailBoxMoreMenu from '@components/emailBox/moreMenu/EmailBoxMoreMenu.tsx';
import {formatDateTime} from '@utils/formatDateTime.ts';
import GenericEmailTamerList from '@components/GenericEmailTamerList.tsx';
import Tooltip from '@mui/material/Tooltip';
import {BackupStatus} from '@api/emailTamerApiSchemas.ts';

import {arraysEqual} from '@utils/arraysEqual.ts';

import {useQueryClient} from '@tanstack/react-query';

import {TranslationScopeProvider} from '../../i18n/contexts/TranslationScopeContext.tsx';

interface EmailBoxesSectionProps {
    emailBoxesIds: string[];
    setEmailBoxesIds: (setEmailBoxesIds: string[]) => void;
}

const EmailBoxesSection = ({emailBoxesIds, setEmailBoxesIds}: EmailBoxesSectionProps) => {
    const {data: emailBoxes, isLoading, refetch} = useGetEmailBoxes({});
    const queryClient = useQueryClient();

    const {
        data: emailBoxesStatuses,
        isLoading: isStatusesLoading,
        refetch: refetchStatuses
    } = useGetEmailBoxesStatuses({},
        {
            enabled: emailBoxes !== undefined && emailBoxes?.length > 0,
            refetchInterval: 5000
        }
    );

    useEffect(() => {
        queryClient.invalidateQueries();
    }, [emailBoxesStatuses]);

    const {mutate: backupBoxes, isPending: isBackupPending} = useBackUpEmailBoxesMessages({
        onSettled: () => {
            refetchStatuses();
        },
    });

    const isBackuping = isBackupPending || isStatusesLoading ||
        emailBoxesStatuses?.find(s => s.backupStatus == BackupStatus.Queued ||
            s.backupStatus == BackupStatus.InProgress) != undefined;

    const handleBackupButtonClick = () => {
        if (emailBoxesIds.length > 0) {
            backupBoxes({
                queryParams: {
                    emailBoxesIds: emailBoxesIds.join(', ')
                }
            });
            setTimeout(refetchStatuses, 500);
        }
    };

    const {t} = useScopedContextTranslator();
    const [openAddEmailBoxDialog, setOpenAddEmailBoxDialog] = useState(false);

    useEffect(() => {
        if (emailBoxes && emailBoxes.length > 0 && emailBoxesIds.length === 0) {
            const newEmailBoxesIds = emailBoxes.map((emailBox) => emailBox.id!);
            if (!arraysEqual(emailBoxesIds, newEmailBoxesIds)) {
                setEmailBoxesIds(newEmailBoxesIds);
            }
        } else if (emailBoxes && emailBoxes.length === 0 && emailBoxesIds.length > 0) {
            setEmailBoxesIds([]);
        }
    }, [emailBoxes]);

    const handleToggle = (boxId: string) => () => {
        const currentIndex = emailBoxesIds.indexOf(boxId);
        if (currentIndex === -1) {
            setEmailBoxesIds([...emailBoxesIds, boxId]);
        } else {
            setEmailBoxesIds(emailBoxesIds.filter(id => id !== boxId));
        }
    };

    const handleSelectAll = () => {
        if (emailBoxes) {
            if (emailBoxesIds.length === emailBoxes.length) {
                setEmailBoxesIds([]);
            } else {
                setEmailBoxesIds(emailBoxes.map((emailBox) => emailBox.id!));
            }
        }
    };

    const handleCloseAddEmailBoxDialog = () => {
        setOpenAddEmailBoxDialog(false);
    };

    const getBackupStatus = (boxId: string) => {
        const status = emailBoxesStatuses?.find((status) => status.id === boxId)?.backupStatus;
        return status || BackupStatus.Idle;
    };

    const renderStatusIcon = (boxId: string, connectionFault?: string) => {
        if (connectionFault) {
            return (
                <Tooltip title={t(`connectionFault.${connectionFault}`)} followCursor>
                    <Warning color='warning'/>
                </Tooltip>
            );
        }

        const status = getBackupStatus(boxId);
        switch (status) {
        case BackupStatus.InProgress:
            return (
                <Tooltip title={t('backupStatus.inProgress')}>
                    <CloudSync color='secondary'/>
                </Tooltip>
            );
        case BackupStatus.Queued:
            return (
                <Tooltip title={t('backupStatus.queued')}>
                    <Cloud color='secondary'/>
                </Tooltip>
            );
        case BackupStatus.Failed:
            return (
                <Tooltip title={t('backupStatus.failed')}>
                    <Warning color='error'/>
                </Tooltip>
            );
        case BackupStatus.Idle:
        default:
            return null;
        }
    };

    const items = emailBoxes?.map((box) => ({
        id: box.id!,
        label: box.boxName!,
        onClick: handleToggle(box.id!),
        icon: (
            <Checkbox
                edge='start'
                checked={emailBoxesIds.includes(box.id!)}
                tabIndex={-1}
                disableRipple
            />
        ),
        secondaryAction: (
            <Stack direction='row' alignItems='center' spacing={0}>
                {renderStatusIcon(box.id!, box.connectionFault)}
                <EmailBoxMoreMenu box={box} refetch={refetch}/>
            </Stack>
        ),
        tooltip: box.lastSyncAt !== null
            ? `${t(box.connectionFault ? 'lastSuccessfulSyncAt' : 'lastSyncAt')} ${formatDateTime(box.lastSyncAt!)}`
            : t('notSynced'),
        dense: true,
    })) || [];

    return (
        <>
            <Typography variant='h6'>{t('emailBoxes')}</Typography>
            <Box
                display='flex'
                justifyContent={items.length > 0 ? 'space-between' : 'end'}
                alignItems='center'
                sx={{m: 1, ml: 0.5}}
            >
                {items.length > 0 && (
                    <Checkbox
                        checked={emailBoxesIds.length === emailBoxes?.length && emailBoxes?.length > 0}
                        onChange={handleSelectAll}
                        sx={{mr: 1}}
                    />
                )}
                <Box display='flex'>
                    {items.length > 0 && (<>
                        <Button
                            variant='contained'
                            size='small'
                            startIcon={<CloudDownload/>}
                            onClick={handleBackupButtonClick}
                            disabled={isBackuping}
                            sx={{
                                mr: 1,
                                display: {xs: 'none', sm: 'flex'}
                            }}
                        >
                            {t('backup')}
                        </Button>
                        <Fab
                            size='small'
                            color='primary'
                            onClick={handleBackupButtonClick}
                            disabled={isBackuping}
                            sx={{
                                mr: 1,
                                display: {xs: 'flex', sm: 'none'}
                            }}
                        >
                            <CloudDownload/>
                        </Fab>
                    </>)}
                    <Button
                        variant='contained'
                        size='small'
                        startIcon={<Add/>}
                        onClick={() => setOpenAddEmailBoxDialog(true)}
                        sx={{display: {xs: 'none', sm: 'flex'}}}
                    >
                        {t('add')}
                    </Button>
                    <Fab
                        size='small'
                        color='primary'
                        onClick={() => setOpenAddEmailBoxDialog(true)}
                        sx={{
                            display: {xs: 'flex', sm: 'none'}
                        }}
                    >
                        <Add/>
                    </Fab>
                </Box>
            </Box>
            {isLoading || isStatusesLoading ? (
                <ContentLoading/>
            ) : (
                <GenericEmailTamerList items={items} sx={{width: '100%'}}/>
            )}
            <TranslationScopeProvider scope='emailBoxForm'>
                <AddEmailBoxDialogForm
                    open={openAddEmailBoxDialog}
                    onClose={handleCloseAddEmailBoxDialog}
                    refetch={refetch}
                />
            </TranslationScopeProvider>
        </>
    );
};

export default EmailBoxesSection;