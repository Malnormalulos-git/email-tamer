import {useForm} from 'react-hook-form';
import {z} from 'zod';
import {zodResolver} from '@hookform/resolvers/zod';
import {useEffect} from 'react';
import {Stack} from '@mui/material';

import EmailTamerDialog from '@components/forms/EmailTamerDialog.tsx';
import TextInputControl from '@components/forms/controls/TextInputControl.tsx';
import PasswordInputControl from '@components/forms/controls/PasswordInputControl.tsx';
import SubmitButton from '@components/forms/controls/SubmitButton.tsx';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {useCreateEmailBox, useTestConnection} from '@api/emailTamerApiComponents.ts';
import {getAppControlActions} from '@store/AppControlStore.ts';
import DoubleLabeledSwitch from '@components/forms/controls/DoubleLabeledSwitch.tsx';
import LabeledCheckbox from '@components/forms/controls/LabeledCheckbox.tsx';
import Fieldset from '@components/forms/Fieldset.tsx';
import {ConnectionFault} from '@api/emailTamerApiSchemas.ts';
import TestEmailBoxConnectionButton from '@components/emailBox/TestEmailBoxConnectionButton.tsx';

interface AddEmailBoxDialogFormProps {
    open: boolean;
    onClose: () => void;
    refetch: () => void;
}

const createEmailBoxSchema = (t: (key: string) => string) =>
    z.object({
        boxName: z.string().optional(),
        userName: z.string().optional(),
        email: z
            .string()
            .min(1, {message: t('validation.emailRequired')})
            .email({message: t('validation.invalidEmail')}),
        password: z
            .string()
            .min(1, {message: t('validation.passwordRequired')}),
        emailDomainConnectionHost: z
            .string()
            .min(1, {message: t('validation.hostRequired')}),
        emailDomainConnectionPort: z
            .preprocess(
                (val) => (typeof val === 'string' ? parseInt(val, 10) : val),
                z
                    .number({invalid_type_error: t('validation.portRequired')})
                    .int()
                    .min(1, {message: t('validation.invalidPort')})
            ),
        authenticateByEmail: z.boolean(),
        useSSl: z.boolean(),
        useDefaultImapPorts: z.boolean(),
    }).refine((data) => data.authenticateByEmail || !!data.userName, {
        message: t('validation.userNameRequired'),
        path: ['userName'],
    });

type EmailBoxFormData = z.infer<ReturnType<typeof createEmailBoxSchema>>;

const AddEmailBoxDialogForm = ({open, onClose, refetch}: AddEmailBoxDialogFormProps) => {
    const {t} = useScopedContextTranslator();
    const {setErrorNotification, setSuccessNotification} = getAppControlActions();

    const form = useForm<EmailBoxFormData>({
        resolver: zodResolver(createEmailBoxSchema(t)),
        defaultValues: {
            boxName: '',
            userName: '',
            email: '',
            password: '',
            emailDomainConnectionHost: '',
            emailDomainConnectionPort: 993,
            authenticateByEmail: false,
            useSSl: true,
            useDefaultImapPorts: true,
        },
    });

    const {watch, setValue, formState: {isValid}} = form;
    const useDefaultImapPorts = watch('useDefaultImapPorts');
    const useSSl = watch('useSSl');
    const authenticateByEmail = watch('authenticateByEmail');

    useEffect(() => {
        if (useDefaultImapPorts) {
            setValue('emailDomainConnectionPort', useSSl ? 993 : 143);
        }
    }, [useDefaultImapPorts, useSSl, setValue]);

    const {mutate: createEmailBox, isPending: isCreating} = useCreateEmailBox({
        onSuccess: () => {
            setSuccessNotification(t('addSuccess'));
            form.reset();
            refetch();
            onClose();
        },
        onError: (error) => {
            if (error?.status === 409) {
                const emailBox = form.getValues('email');
                setErrorNotification(t('error.alreadyExist') + emailBox);
            } else {
                setErrorNotification(t('addError'));
            }
        },
    });

    const {mutate: testConnection, isPending: isTesting} = useTestConnection({
        onSuccess: () => {
            setSuccessNotification(t('testConnectionSuccess'));
            const data = form.getValues();
            createEmailBox({
                body: {
                    boxName: data.boxName || null,
                    userName: !data.authenticateByEmail ? data.userName : null,
                    email: data.email,
                    password: data.password,
                    emailDomainConnectionHost: data.emailDomainConnectionHost,
                    emailDomainConnectionPort: data.emailDomainConnectionPort,
                    authenticateByEmail: data.authenticateByEmail,
                    useSSl: data.useSSl,
                },
            });
        },
        onError: (error) => {
            const fault = error as any as ConnectionFault;
            const errorMessage = ConnectionFault !== undefined && fault !== ConnectionFault.Other
                ? t('testConnectionError') + t(`connectionFault.${fault}`)
                : t('testConnectionError');
            setErrorNotification(errorMessage);
        },
    });

    const onSubmit = (data: EmailBoxFormData) => {
        testConnection({
            body: {
                userName: !data.authenticateByEmail ? data.userName : null,
                email: data.email,
                password: data.password,
                emailDomainConnectionHost: data.emailDomainConnectionHost,
                emailDomainConnectionPort: data.emailDomainConnectionPort,
                authenticateByEmail: data.authenticateByEmail,
                useSSl: data.useSSl,
            },
        });
    };

    const isPending = isTesting || isCreating;

    return (
        <EmailTamerDialog
            title={t('addEmailBox')}
            open={open}
            onClose={onClose}
            slotProps={{
                paper: {
                    component: 'form',
                    onSubmit: form.handleSubmit(onSubmit),
                },
            }}
            dialogActions={
                <>
                    <TestEmailBoxConnectionButton
                        form={form as any}
                        disabled={isPending || !isValid}
                    />
                    <SubmitButton disabled={isPending}>
                        {isPending ? <ContentLoading size={24}/> : t('addSubmitButton')}
                    </SubmitButton>
                </>
            }
        >
            <Fieldset disabled={isPending}>
                <TextInputControl
                    label={t('boxName')}
                    form={form}
                    id='boxName'
                />
                <TextInputControl
                    disabled={authenticateByEmail}
                    label={t('userName')}
                    form={form}
                    id='userName'
                />
                <TextInputControl
                    label={t('email')}
                    type='email'
                    form={form}
                    id='email'
                />
                <PasswordInputControl
                    label={t('password')}
                    form={form}
                    id='password'
                />
                <TextInputControl
                    label={t('emailDomainConnectionHost')}
                    form={form}
                    id='emailDomainConnectionHost'
                />
                <TextInputControl
                    disabled={useDefaultImapPorts}
                    label={t('emailDomainConnectionPort')}
                    type='number'
                    form={form}
                    id='emailDomainConnectionPort'
                />
                <Stack gap={1} ml={3} mt={1}>
                    <DoubleLabeledSwitch
                        leftLabel={t('authenticateByUsername')}
                        rightLabel={t('authenticateByEmail')}
                        id={'authenticateByEmail'}
                        form={form}
                    />
                    <LabeledCheckbox
                        label={t('useSSl')}
                        id={'useSSl'}
                        form={form}
                    />
                    <LabeledCheckbox
                        label={t('useDefaultImapPorts')}
                        id={'useDefaultImapPorts'}
                        form={form}
                    />
                </Stack>
            </Fieldset>
        </EmailTamerDialog>
    );
};

export default AddEmailBoxDialogForm;