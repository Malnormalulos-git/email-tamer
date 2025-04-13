import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {useState} from 'react';
import {MenuItem} from '@mui/material';
import EditEmailBoxDialogForm from '@components/emailBox/EditEmailBoxDialogForm.tsx';
import {EmailBoxDto} from '@api/emailTamerApiSchemas.ts';

import {TranslationScopeProvider} from '../../../i18n/contexts/TranslationScopeContext.tsx';

interface EditEmailBoxMenuItemProps {
    box: EmailBoxDto;
    refetch: () => void;
    onCloseMenu: () => void;
}

const EditEmailBoxMenuItem = ({box, refetch, onCloseMenu}: EditEmailBoxMenuItemProps) => {
    const {t} = useScopedContextTranslator();
    const [openEditDialog, setOpenEditDialog] = useState(false);

    const handleOpenEditDialog = () => {
        setOpenEditDialog(true);
    };

    const handleCloseEditDialog = () => {
        setOpenEditDialog(false);
        onCloseMenu();
    };

    return (
        <>
            <MenuItem onClick={handleOpenEditDialog}>
                {t('edit')}
            </MenuItem>
            <TranslationScopeProvider scope='homePage.emailBoxesSection.emailBoxForm' rewriteScope>
                <EditEmailBoxDialogForm
                    open={openEditDialog}
                    onClose={handleCloseEditDialog}
                    refetch={refetch}
                    boxId={box.id!}
                />
            </TranslationScopeProvider>
        </>
    );
};

export default EditEmailBoxMenuItem;