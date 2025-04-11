import {Dialog, DialogActions, DialogContent, DialogTitle, IconButton} from '@mui/material';
import {Close} from '@mui/icons-material';
import * as React from 'react';

interface EmailTamerDialogProps extends DialogProps {
    title?: string;
    children: React.ReactNode;
    dialogActions?: React.ReactNode;
}

const EmailTamerDialog = ({title, onClose, children, dialogActions, open, ...otherProps}: EmailTamerDialogProps) => {
    return (
        <Dialog onClose={onClose} open={open} {...otherProps}>
            {title && <DialogTitle sx={{m: 0, p: 2}}>{title}</DialogTitle>}
            <IconButton
                onClick={onClose}
                sx={(theme) => ({
                    position: 'absolute',
                    right: 8,
                    top: 8,
                    color: theme.palette.grey[500],
                })}
            >
                <Close/>
            </IconButton>
            <DialogContent dividers>{children}</DialogContent>
            {dialogActions && <DialogActions>{dialogActions}</DialogActions>}
        </Dialog>
    );
};

export default EmailTamerDialog;