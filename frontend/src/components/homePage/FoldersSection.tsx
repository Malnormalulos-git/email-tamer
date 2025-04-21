import {Typography} from '@mui/material';
import {useGetFolders} from '@api/emailTamerApiComponents.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import GenericEmailTamerList from '@components/GenericEmailTamerList.tsx';

interface FoldersSectionProps {
    selectedFolderId: string | null;
    setSelectedFolderId: (selectedFolderId: string | null) => void;
}

const FoldersSection = ({selectedFolderId, setSelectedFolderId}: FoldersSectionProps) => {
    const {data: folders, isLoading} = useGetFolders({});
    const {t} = useScopedContextTranslator();

    const items = [
        {
            id: 'allFoldersIndex',
            label: t('all'),
            onClick: () => setSelectedFolderId(null),
            selected: selectedFolderId === null,
        },
        ...(folders?.map((folder) => ({
            id: folder.id!,
            label: folder.name!,
            onClick: () => setSelectedFolderId(folder.id!),
            selected: selectedFolderId === folder.id,
        })) || []),
    ];

    return (
        <>
            <Typography variant='h6' gutterBottom>
                {t('folders')}
            </Typography>
            {isLoading
                ? <ContentLoading/>
                : <GenericEmailTamerList items={items}/>
            }
        </>
    );
};

export default FoldersSection;