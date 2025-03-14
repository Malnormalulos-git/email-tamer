import {router} from '@router/router';
import {QueryClient, QueryClientProvider} from '@tanstack/react-query';
import {RouterProvider} from 'react-router-dom';


const queryClient = new QueryClient();

export default function App() {
    return (
        <QueryClientProvider client={queryClient}>
            <RouterProvider router={router}/>
        </QueryClientProvider>
    );
}
