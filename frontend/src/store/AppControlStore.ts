import {createStore} from 'zustand/vanilla';
import {devtools} from 'zustand/middleware';
import {useStore} from 'zustand';

import {ExtractState} from './types';

enum NotificationType {
    Error = 'error',
    Warning = 'warning',
    Info = 'info',
    Success = 'success'
}

type Notification = {
    message: string,
    type: NotificationType
}

type AppControlStore = {
    notification: Notification | null,

    actions: {
        setWarningNotification: (message: string) => void,
        setErrorNotification: (message: string) => void,
        setInfoNotification: (message: string) => void,
        setSuccessNotification: (message: string) => void,
    }
}

const appControlStore = createStore<AppControlStore>()(
    devtools((set) => ({
        notification: null,
        isForescreenLoading: false,
        actions: {
            setWarningNotification: (message: string) => {
                set({
                    notification: {
                        message,
                        type: NotificationType.Warning
                    }
                });
            },
            setErrorNotification: (message: string) => {
                set({
                    notification: {
                        message,
                        type: NotificationType.Error
                    }
                });
            },
            setInfoNotification: (message: string) => {
                set({
                    notification: {
                        message,
                        type: NotificationType.Info
                    }
                });
            },
            setSuccessNotification: (message: string) => {
                set({
                    notification: {
                        message,
                        type: NotificationType.Success
                    }
                });
            }
        }
    }),
    {
        name: 'app-control-store'
    })
);


const notificationSelector = (state: ExtractState<typeof appControlStore>) => state.notification;
const actionsSelector = (state: ExtractState<typeof appControlStore>) => state.actions;

export const getNotification = () => notificationSelector(appControlStore.getState());
export const getAppControlActions = () => actionsSelector(appControlStore.getState());

type UseStoreParams<U> = Parameters<typeof useStore<typeof appControlStore, U>>;

export function useAppControlStore<U>(selector: UseStoreParams<U>[1]) {
    return useStore(appControlStore, selector);
}