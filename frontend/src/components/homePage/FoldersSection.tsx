import {List, ListItemButton, ListItemText, Typography} from '@mui/material';
import {useGetFolders} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';

interface FoldersSectionProps {
    selectedFolderId: string | null;
    setSelectedFolderId: (selectedFolderId: string | null) => void;
}

const FoldersSection = ({selectedFolderId, setSelectedFolderId}: FoldersSectionProps) => {
    const {data: folders, isLoading} = useGetFolders({});
    const {t} = useScopedContextTranslator();

    return (
        <>
            <Typography variant='h6' gutterBottom>
                {t('folders')}
            </Typography>
            {isLoading
                ? <ContentLoading/>
                : <List component='nav' sx={{width: '100%'}}>
                    <ListItemButton
                        key={'allFoldersIndex'}
                        selected={selectedFolderId === null}
                        onClick={() => setSelectedFolderId(null)}
                    >
                        <ListItemText primary={t('all')}/>
                    </ListItemButton>
                    {folders?.map((folder) => {
                        return (
                            <ListItemButton
                                key={folder.id}
                                selected={selectedFolderId === folder.id}
                                onClick={() => setSelectedFolderId(folder.id!)}
                            >
                                <ListItemText primary={folder.name}/>
                            </ListItemButton>
                        );
                    })}
                </List>
            }
        </>
    );
};

export default FoldersSection;