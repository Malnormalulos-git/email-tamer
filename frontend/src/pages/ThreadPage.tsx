import {useParams} from 'react-router-dom';
import {THREAD_ID_PARAM_NAME} from '@router/routes.ts';

const ThreadPage = () => {
    const {[THREAD_ID_PARAM_NAME]: threadId} = useParams();
    const decodedThreadId = threadId ? decodeURIComponent(threadId).replace('%2E', '.') : '';

    return <>{decodedThreadId}</>;
};

export default ThreadPage;