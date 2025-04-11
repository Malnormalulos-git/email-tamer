import {
    Box, Button,
    Checkbox,
    IconButton,
    List,
    ListItem,
    ListItemButton,
    ListItemIcon,
    ListItemText,
    Typography
} from '@mui/material';
import {useGetEmailBoxes} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {Add, MoreVert, Refresh} from '@mui/icons-material';
import {useEffect, useState} from 'react';
import AddEmailBoxDialogForm from '@components/forms/emailBox/AddEmailBoxDialogForm.tsx';

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
        const newEmailBoxesIds = [...emailBoxesIds ?? []];

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

    return (
        <>
            <Typography variant='h6'>
                {t('emailBoxes')}
            </Typography>
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
                <List sx={{width: '100%'}}>
                    {emailBoxes?.map((box) => (
                        <ListItem
                            key={box.id}
                            component='li'
                            secondaryAction={
                                <IconButton edge='end' onClick={() => console.log('more')}>
                                    <MoreVert/>
                                </IconButton>
                            }
                            disablePadding
                        >
                            <ListItemButton role={undefined} onClick={handleToggle(box.id!)} dense>
                                <ListItemIcon>
                                    <Checkbox
                                        edge='start'
                                        checked={emailBoxesIds.includes(box.id!)}
                                        tabIndex={-1}
                                        disableRipple
                                    />
                                </ListItemIcon>
                                <ListItemText id={box.id} primary={box.boxName} sx={{wordWrap: 'break-word'}}/>
                            </ListItemButton>
                        </ListItem>
                    ))}
                </List>
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