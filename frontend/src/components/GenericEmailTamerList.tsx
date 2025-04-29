import List, {ListProps} from '@mui/material/List';
import ListItemButton from '@mui/material/ListItemButton';
import ListItem from '@mui/material/ListItem';
import ListItemText from '@mui/material/ListItemText';
import ListItemIcon from '@mui/material/ListItemIcon';
import Divider from '@mui/material/Divider';
import Tooltip from '@mui/material/Tooltip';
import {ReactNode} from 'react';
import React from 'react';

interface ListItem {
    id: string;
    label: string | ReactNode;
    onClick: () => void;
    selected?: boolean;
    icon?: ReactNode;
    secondaryAction?: ReactNode;
    tooltip?: string;
    dense?: boolean;
    secondary?: ReactNode;
}

interface GenericEmailTamerListProps extends ListProps {
    items: ListItem[];
    showDivider?: boolean;
}

const GenericEmailTamerList = ({items, showDivider = false, ...props}: GenericEmailTamerListProps) => {
    return (
        <List {...props}>
            {items.map((item) => (
                <React.Fragment key={item.id}>
                    <ListItem
                        secondaryAction={item.secondaryAction}
                        disablePadding
                    >
                        <ListItemButton onClick={item.onClick} selected={item.selected} dense={item.dense}>
                            {item.icon && <ListItemIcon>{item.icon}</ListItemIcon>}
                            {item.tooltip ? (
                                <Tooltip title={item.tooltip} followCursor>
                                    <ListItemText
                                        primary={item.label}
                                        secondary={item.secondary}
                                        sx={{
                                            wordBreak: 'break-word',
                                            maxWidth: 'calc(100% - 80px)',
                                        }}
                                    />
                                </Tooltip>
                            ) : (
                                <ListItemText primary={item.label} secondary={item.secondary}/>
                            )}
                        </ListItemButton>
                    </ListItem>
                    {showDivider && <Divider/>}
                </React.Fragment>
            ))}
        </List>
    );
};

export default GenericEmailTamerList;