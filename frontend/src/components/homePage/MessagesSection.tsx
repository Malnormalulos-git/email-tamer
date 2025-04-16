import {
    Box,
    List,
    ListItem,
    ListItemButton,
    ListItemText,
    Typography,
    IconButton,
    TextField,
    MenuItem,
    Select,
    InputLabel,
    FormControl, Divider, SelectChangeEvent,
} from '@mui/material';
import {useGetMessagesThreads} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {useReducer, useState} from 'react';
import {ArrowBackIos, ArrowForwardIos, ExpandMore} from '@mui/icons-material';
import * as React from 'react';
import {formatDate} from '@utils/formatDateTime.ts';
import {SEARCH_PARAM} from '@router/urlParams.ts';
import { useUrlParam } from '@hooks/useUrlParam';

interface MessagesSectionProps {
    selectedFolderId: string | null;
    emailBoxesIds: string[];
}

const MessagesSection = ({selectedFolderId, emailBoxesIds}: MessagesSectionProps) => {
    const [page, setPage] = useState(1);
    const [messagesPerPage, setMessagesPerPage] = useState(20);
    const pageSizeOptions = [10, 20, 50, 100];
    const [isByDescending, toggleIsByDescending] = useReducer((state) => {
        setPage(1);
        return !state;
    }, true);

    const searchTerm = useUrlParam(SEARCH_PARAM);
    
    const isAnyEmaiBoxSelected = emailBoxesIds.length > 0;

    const {data: messagesThreads, isLoading} = useGetMessagesThreads({
        queryParams: {
            folderId: selectedFolderId ?? undefined,
            emailBoxesIds: emailBoxesIds.length > 0 ? emailBoxesIds.join(', ') : undefined,
            page,
            size: messagesPerPage,
            isByDescending: isByDescending,
            searchTerm: searchTerm ?? undefined,
        },
    },
    {
        enabled: isAnyEmaiBoxSelected,
    }
    );

    const {t} = useScopedContextTranslator();

    const totalMessages = messagesThreads?.total ?? 0;
    const totalPages = Math.ceil(totalMessages / messagesPerPage);
    const startMessage = (page - 1) * messagesPerPage + 1;
    const endMessage = Math.min(page * messagesPerPage, totalMessages);

    const handlePreviousPage = () => {
        if (page > 1) {
            setPage(page - 1);
        }
    };

    const handleNextPage = () => {
        if (page < totalPages) {
            setPage(page + 1);
        }
    };

    const handlePageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newPage = parseInt(event.target.value, 10);
        if (!isNaN(newPage) && newPage >= 1 && newPage <= totalPages) {
            setPage(newPage);
        }
    };

    const handlePageSizeChange = (event: SelectChangeEvent<number>) => {
        const newSize = event.target.value as number;
        setMessagesPerPage(newSize);
        setPage(1);
    };

    return (
        <>
            <Typography variant='h6' gutterBottom>
                {t('messages')}
            </Typography>
            {isAnyEmaiBoxSelected && messagesThreads?.items?.length !== undefined && messagesThreads?.items?.length > 0
                ? <>
                    <Box
                        display='flex'
                        justifyContent='flex-end'
                        alignItems='center'
                        sx={{mb: 2}}
                    >
                        <IconButton onClick={toggleIsByDescending}>
                            <ExpandMore
                                sx={{
                                    transform: isByDescending ? 'rotate(0deg)' : 'rotate(180deg)',
                                    transition: 'transform 0.3s ease',
                                }}
                            />
                        </IconButton>
                        <Box display='flex' alignItems='center'>
                            <TextField
                                label={t('page')}
                                type='number'
                                value={page}
                                onChange={handlePageChange}
                                size='small'
                                sx={{width: 70, mr: 1}}
                                inputProps={{
                                    min: 1,
                                    max: totalPages
                                }}
                            />
                        </Box>
                        <Box display='flex' alignItems='center'>
                            <FormControl size='small' sx={{mr: 2}}>
                                <InputLabel>{t('messagesPerPage')}</InputLabel>
                                <Select
                                    value={messagesPerPage}
                                    onChange={handlePageSizeChange}
                                    label={t('messagesPerPage')}
                                >
                                    {pageSizeOptions.map((size) => (
                                        <MenuItem key={size} value={size}>
                                            {size}
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                        </Box>
                        <Box display='flex' alignItems='center'>
                            <Typography variant='body2' sx={{mr: 1}}>
                                {`${startMessage}-${endMessage} of ${totalMessages}`}
                            </Typography>
                            <IconButton
                                onClick={handlePreviousPage}
                                disabled={page === 1}
                                size='small'
                            >
                                <ArrowBackIos fontSize='small'/>
                            </IconButton>
                            <IconButton
                                onClick={handleNextPage}
                                disabled={page === totalPages || totalPages === 0}
                                size='small'
                            >
                                <ArrowForwardIos fontSize='small'/>
                            </IconButton>
                        </Box>
                    </Box>
                    {isLoading
                        ? <ContentLoading/>
                        : <List sx={{width: '100%'}}>
                            {messagesThreads?.items?.map((thread) => (
                                <React.Fragment key={thread.threadId}>
                                    <ListItem key={thread.threadId} disablePadding>
                                        <ListItemButton>
                                            <ListItemText
                                                primary={thread.subject || t('noSubject')}
                                                secondary={
                                                    <>
                                                        {thread.lastMessage?.participants?.join(', ')}
                                                        {', '}
                                                        {`${formatDate(thread.startDate!)}
                                                  ${thread.startDate! != thread.endDate!
                                                            ? ` - ${formatDate(thread.endDate!)}`
                                                            : ''}`}
                                                        <br/>
                                                        {thread.lastMessage?.textBody}
                                                        {thread.lastMessage?.textBody?.length == 200 ? '...' : ''}
                                                        <br/>
                                                    </>
                                                }
                                            />
                                        </ListItemButton>
                                    </ListItem>
                                    <Divider/>
                                </React.Fragment>
                            ))}
                        </List>
                    }
                </>
                : <Typography variant='body1'>
                    {t('noMessagesFound')}
                </Typography>
            }
        </>
    );
};

export default MessagesSection;