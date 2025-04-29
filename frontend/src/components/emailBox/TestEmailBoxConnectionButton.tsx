import {Button, ButtonProps} from '@mui/material';
import {SettingsEthernet} from '@mui/icons-material';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {useTestConnection} from '@api/emailTamerApiComponents.ts';
import {getAppControlActions} from '@store/AppControlStore.ts';
import {useState} from 'react';
import {ConnectionFault} from '@api/emailTamerApiSchemas.ts';
import {UseFormReturn} from 'react-hook-form';

interface EmailBoxFormData {
    boxName?: string | null;
    userName?: string | null;
    email: string;
    password?: string | null;
    emailDomainConnectionHost: string;
    emailDomainConnectionPort: number;
    authenticateByEmail: boolean;
    useSSl: boolean;
    useDefaultImapPorts: boolean;
}

type TestEmailBoxConnectionButtonProps<T extends EmailBoxFormData> = ButtonProps & {
    boxId?: string;
    form: UseFormReturn<T>;
};

const TestEmailBoxConnectionButton = <T extends EmailBoxFormData>({
    form,
    boxId,
    disabled,
    ...restProps
}: TestEmailBoxConnectionButtonProps<T>) => {
    const {t} = useScopedContextTranslator();
    const {setSuccessNotification, setErrorNotification} = getAppControlActions();

    const [lastAttemptResult, setLastAttemptResult] = useState<'warning' | 'success' | undefined>(undefined);

    const {mutate: testConnection, isPending} = useTestConnection({
        onSuccess: () => {
            setSuccessNotification(t('testConnectionSuccess'));
            setLastAttemptResult('success');
        },
        onError: (error) => {
            const fault = error as any as ConnectionFault;
            const errorMessage = ConnectionFault !== undefined && fault !== ConnectionFault.Other
                ? t('testConnectionError') + t(`connectionFault.${fault}`)
                : t('testConnectionError');

            setErrorNotification(errorMessage);
            setLastAttemptResult('warning');
        },
    });

    const handleOnClick = () => {
        const formData = form.getValues();

        testConnection({
            body: {
                id: boxId,
                userName: !formData.authenticateByEmail ? formData.userName : null,
                email: formData.email,
                password: formData.password || null,
                emailDomainConnectionHost: formData.emailDomainConnectionHost,
                emailDomainConnectionPort: formData.emailDomainConnectionPort,
                authenticateByEmail: formData.authenticateByEmail,
                useSSl: formData.useSSl,
            },
        });
    };

    return (
        <Button
            onClick={handleOnClick}
            endIcon={<SettingsEthernet/>}
            disabled={disabled || isPending}
            variant='outlined'
            sx={{mt: 2, mr: 2}}
            fullWidth
            color={lastAttemptResult}
            {...restProps}
        >
            {isPending ? t('testConnection') + '...' : t('testConnection')}
        </Button>
    );
};

export default TestEmailBoxConnectionButton;