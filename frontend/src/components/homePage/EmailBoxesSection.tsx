import {Checkbox, IconButton, Typography, Box, Button} from '@mui/material';
import {useGetEmailBoxes} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {Add, Refresh} from '@mui/icons-material';
import {useEffect, useState} from 'react';
import AddEmailBoxDialogForm from '@components/emailBox/AddEmailBoxDialogForm.tsx';
import EmailBoxMoreMenu from '@components/emailBox/moreMenu/EmailBoxMoreMenu.tsx';
import {formatDateTime} from '@utils/formatDateTime.ts';

import GenericEmailTamerList from '@components/GenericEmailTamerList.tsx';

import {TranslationScopeProvider} from '../../i18n/contexts/TranslationScopeContext.tsx';

interface EmailBoxesSectionProps {
    emailBoxesIds: string[];
    setEmailBoxesIds: (setEmailBoxesIds: string[]) => void;
}

const EmailBoxesSection = ({emailBoxesIds, setEmailBoxesIds}: EmailBoxesSectionProps) => {
    const {data: emailBoxes, isLoading, refetch} = useGetEmailBoxes({});
    const {t} = useScopedContextTranslator();
    const [openAddEmailBoxDialog, setOpenAddEmailBoxDialog] = useState(false);

    useEffect(() => {
        if (emailBoxes) {
            setEmailBoxesIds(emailBoxes.map((emailBox) => emailBox.id!));
        }
    }, [emailBoxes, setEmailBoxesIds]);

    const handleToggle = (boxId: string) => () => {
        const currentIndex = emailBoxesIds?.indexOf(boxId);
        const newEmailBoxesIds = [...(emailBoxesIds ?? [])];

        if (currentIndex === -1 || currentIndex === undefined) {
            newEmailBoxesIds.push(boxId);
        } else {
            newEmailBoxesIds.splice(currentIndex, 1);
        }

        setEmailBoxesIds(newEmailBoxesIds);
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
        secondaryAction: <EmailBoxMoreMenu box={box} refetch={refetch} edge='end'/>,
        tooltip: box.lastSyncAt !== null ? `${t('lastSyncAt')} ${formatDateTime(box.lastSyncAt!)}` : t('notSynced'),
        dense: true,
    })) || [];

    return (
        <>
            <Typography variant='h6'>{t('emailBoxes')}</Typography>
            <Box
                display='flex'
                justifyContent='space-between'
                alignItems='center'
                sx={{m: 1, ml: 0.5}}
            >
                <Checkbox
                    checked={emailBoxesIds.length === emailBoxes?.length && emailBoxes?.length > 0}
                    onChange={handleSelectAll}
                    sx={{mr: 1}}
                />
                <Box>
                    <IconButton onClick={() => console.log('sync all')} sx={{mr: 1}}>
                        <Refresh/>
                    </IconButton>
                    <Button
                        variant='contained'
                        size='small'
                        startIcon={<Add/>}
                        onClick={() => setOpenAddEmailBoxDialog(true)}
                    >
                        {t('add')}
                    </Button>
                </Box>
            </Box>
            {isLoading ? (
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