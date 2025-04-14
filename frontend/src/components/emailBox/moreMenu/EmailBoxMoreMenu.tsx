import {MoreVert} from '@mui/icons-material';
import {IconButton, IconButtonProps, Menu} from '@mui/material';
import {EmailBoxDto} from '@api/emailTamerApiSchemas.ts';
import {useState} from 'react';
import * as React from 'react';

import EditEmailBoxMenuItem from '@components/emailBox/moreMenu/EditEmailBoxMenuItem.tsx';
import DeleteEmailBoxMenuItem from '@components/emailBox/moreMenu/DeleteEmailBoxMenuItem.tsx';

import {TranslationScopeProvider} from '../../../i18n/contexts/TranslationScopeContext.tsx';

interface EmailBoxMoreMenuProps extends IconButtonProps {
    box: EmailBoxDto;
    refetch: () => void;
}

const EmailBoxMoreMenu = ({box, refetch, ...restProps}: EmailBoxMoreMenuProps) => {
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);

    const handleClick = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    return (
        <>
            <IconButton onClick={handleClick} {...restProps}>
                <MoreVert/>
            </IconButton>
            {open && <Menu
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
            >
                <TranslationScopeProvider scope='moreMenu'>
                    <EditEmailBoxMenuItem
                        box={box}
                        refetch={refetch}
                        onCloseMenu={handleClose}
                    />
                    <TranslationScopeProvider scope='deleteItem'>
                        <DeleteEmailBoxMenuItem
                            box={box}
                            refetch={refetch}
                            onCloseMenu={handleClose}
                        />
                    </TranslationScopeProvider>
                </TranslationScopeProvider>
            </Menu>}
        </>
    );
};

export default EmailBoxMoreMenu;