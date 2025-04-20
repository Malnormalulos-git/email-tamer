import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {
    Box,
    FormControl,
    IconButton,
    InputLabel,
    MenuItem,
    Select,
    SelectChangeEvent,
    TextField,
    Typography
} from '@mui/material';
import {ArrowBackIos, ArrowForwardIos, ExpandMore} from '@mui/icons-material';

interface PagesHandlerProps {
    isByDescending: boolean;
    toggleIsByDescending: () => void;
    page: number;
    setPage: (page: number) => void;
    messagesPerPage: number;
    setMessagesPerPage: (numOfPages: number) => void;
    totalMessages?: number;
}

const PagesHandler = ({
    isByDescending,
    toggleIsByDescending,
    page,
    setPage,
    messagesPerPage,
    setMessagesPerPage,
    totalMessages = 0
}: PagesHandlerProps) => {
    const {t} = useScopedContextTranslator();

    const pageSizeOptions = [10, 20, 50, 100];

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
                <Typography variant='body2' sx={{mr: 1, display: {xs: 'none', sm: 'block'}}}>
                    {`${startMessage}-${endMessage} of ${totalMessages}`}
                </Typography>
                <Typography variant='body2' sx={{mr: 1, display: {xs: 'block', sm: 'none'}}}>
                    {`${page}/${totalPages}`}
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
    );
};

export default PagesHandler;