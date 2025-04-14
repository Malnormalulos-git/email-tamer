import {useForm} from 'react-hook-form';
import {z} from 'zod';
import {zodResolver} from '@hookform/resolvers/zod';
import {useEffect, useState} from 'react';
import {Box, Checkbox, Typography} from '@mui/material';

import EmailTamerDialog from '@components/forms/EmailTamerDialog.tsx';
import TextInputControl from '@components/forms/controls/TextInputControl.tsx';
import PasswordInputControl from '@components/forms/controls/PasswordInputControl.tsx';
import SubmitButton from '@components/forms/controls/SubmitButton.tsx';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {useCreateEmailBox} from '@api/emailTamerApiComponents.ts';
import {getAppControlActions} from '@store/AppControlStore.ts';

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
            .number({invalid_type_error: t('validation.portRequired')})
            .int()
            .min(1, {message: t('validation.invalidPort')}),
        authenticateByEmail: z.boolean(),
        useSSl: z.boolean(),
        useDefaultImapPorts: z.boolean(),
    }).superRefine((data, ctx) => {
        if (!data.authenticateByEmail && !data.userName) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ['userName'],
                message: t('validation.userNameRequired'),
            });
        }
    });

type EmailBoxFormData = z.infer<ReturnType<typeof createEmailBoxSchema>>;

const AddEmailBoxDialogForm = ({open, onClose, refetch}: AddEmailBoxDialogFormProps) => {
    const {t} = useScopedContextTranslator();
    const {setErrorNotification, setSuccessNotification} = getAppControlActions();

    const [showPassword, setShowPassword] = useState(false);
    const handleClickShowPassword = () => setShowPassword(!showPassword);

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

    const {watch, setValue} = form;
    const useDefaultImapPorts = watch('useDefaultImapPorts');
    const useSSl = watch('useSSl');
    const authenticateByEmail = watch('authenticateByEmail');

    useEffect(() => {
        if (useDefaultImapPorts) {
            setValue('emailDomainConnectionPort', useSSl ? 993 : 143);
        }
    }, [useDefaultImapPorts, useSSl, setValue]);

    const {mutate: createEmailBox, isPending} = useCreateEmailBox({
        onSuccess: () => {
            setSuccessNotification(t('addSuccess'));
            form.reset();
            refetch();
            onClose();
        },
        onError: () => {
            setErrorNotification(t('addError'));
        },
    });

    const onSubmit = (data: EmailBoxFormData) => {
        createEmailBox({
            body: {
                boxName: data.boxName || null,
                userName: data.userName || null,
                email: data.email,
                password: data.password,
                emailDomainConnectionHost: data.emailDomainConnectionHost,
                emailDomainConnectionPort: data.emailDomainConnectionPort,
                authenticateByEmail: data.authenticateByEmail,
                useSSl: data.useSSl,
            }
        });
    };

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
                <SubmitButton disabled={isPending}>
                    {isPending ? <ContentLoading size={24}/> : t('addSubmitButton')}
                </SubmitButton>
            }
        >
            <TextInputControl
                autoFocus
                disabled={isPending}
                label={t('boxName')}
                form={form}
                id='boxName'
            />
            {!authenticateByEmail && (
                <TextInputControl
                    disabled={isPending}
                    label={t('userName')}
                    form={form}
                    id='userName'
                />
            )}
            <TextInputControl
                disabled={isPending}
                label={t('email')}
                type='email'
                form={form}
                id='email'
            />
            <PasswordInputControl
                disabled={isPending}
                showPassword={showPassword}
                handleClickShowPassword={handleClickShowPassword}
                label={t('password')}
                form={form}
                id='password'
            />
            <TextInputControl
                disabled={isPending}
                label={t('emailDomainConnectionHost')}
                form={form}
                id='emailDomainConnectionHost'
            />
            <TextInputControl
                disabled={isPending || useDefaultImapPorts}
                label={t('emailDomainConnectionPort')}
                type='number'
                form={form}
                id='emailDomainConnectionPort'
            />
            <Box sx={{display: 'flex', alignItems: 'center', mt: 2}}>
                <Checkbox
                    disabled={isPending}
                    {...form.register('authenticateByEmail')}
                    checked={authenticateByEmail}
                />
                <Typography>{t('authenticateByEmail')}</Typography>
            </Box>
            <Box sx={{display: 'flex', alignItems: 'center', mt: 1}}>
                <Checkbox
                    disabled={isPending}
                    {...form.register('useSSl')}
                    checked={useSSl}
                />
                <Typography>{t('useSSl')}</Typography>
            </Box>
            <Box sx={{display: 'flex', alignItems: 'center', mt: 1}}>
                <Checkbox
                    disabled={isPending}
                    {...form.register('useDefaultImapPorts')}
                    checked={useDefaultImapPorts}
                />
                <Typography>{t('useDefaultImapPorts')}</Typography>
            </Box>
        </EmailTamerDialog>
    );
};

export default AddEmailBoxDialogForm;