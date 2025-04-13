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
import {useEditEmailBox, useGetEmailBoxDetails} from '@api/emailTamerApiComponents.ts';
import {getAppControlActions} from '@store/AppControlStore.ts';

interface EditEmailBoxDialogFormProps {
    open: boolean;
    onClose: () => void;
    refetch: () => void;
    boxId: string;
}

const editEmailBoxSchema = (t: (key: string) => string) =>
    z.object({
        boxName: z.string().optional(),
        userName: z.string().optional(),
        email: z
            .string()
            .min(1, {message: t('validation.emailRequired')})
            .email({message: t('validation.invalidEmail')}),
        password: z.string().optional(),
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

type EmailBoxFormData = z.infer<ReturnType<typeof editEmailBoxSchema>>;

const EditEmailBoxDialogForm = ({open, onClose, refetch, boxId}: EditEmailBoxDialogFormProps) => {
    const {t} = useScopedContextTranslator();
    const {setErrorNotification, setSuccessNotification} = getAppControlActions();

    const [showPassword, setShowPassword] = useState(false);
    const handleClickShowPassword = () => setShowPassword(!showPassword);

    const {data: emailBox, isLoading: isFetching} = useGetEmailBoxDetails({
        pathParams: {id: boxId},
    });

    const form = useForm<EmailBoxFormData>({
        resolver: zodResolver(editEmailBoxSchema(t)),
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

    const {watch, setValue, reset, formState: {isDirty: haveFormValuesChanged}} = form;
    const useDefaultImapPorts = watch('useDefaultImapPorts');
    const useSSl = watch('useSSl');
    const authenticateByEmail = watch('authenticateByEmail');

    useEffect(() => {
        if (emailBox) {
            reset({
                boxName: emailBox.boxName || '',
                userName: emailBox.userName || '',
                email: emailBox.email || '',
                password: '',
                emailDomainConnectionHost: emailBox.emailDomainConnectionHost || '',
                emailDomainConnectionPort: emailBox.emailDomainConnectionPort || 993,
                authenticateByEmail: emailBox.authenticateByEmail || false,
                useSSl: emailBox.useSSl || true,
                useDefaultImapPorts: true,
            });
        }
    }, [emailBox, reset]);

    useEffect(() => {
        if (useDefaultImapPorts) {
            setValue('emailDomainConnectionPort', useSSl ? 993 : 143);
        }
    }, [useDefaultImapPorts, useSSl, setValue]);

    const {mutate: editEmailBox, isPending} = useEditEmailBox({
        onSuccess: () => {
            setSuccessNotification(t('editSuccess'));
            form.reset();
            refetch();
            onClose();
        },
        onError: () => {
            setErrorNotification(t('editError'));
        },
    });

    const onSubmit = (data: EmailBoxFormData) => {
        editEmailBox({
            body: {
                id: boxId,
                boxName: data.boxName || null,
                userName: data.userName || null,
                email: data.email,
                password: data.password || null,
                emailDomainConnectionHost: data.emailDomainConnectionHost,
                emailDomainConnectionPort: data.emailDomainConnectionPort,
                authenticateByEmail: data.authenticateByEmail,
                useSSl: data.useSSl,
            },
        });
    };

    return (
        <EmailTamerDialog
            title={t('editEmailBox')}
            open={open}
            onClose={onClose}
        >
            {isFetching ? (
                <ContentLoading/>
            ) : (
                <form onSubmit={form.handleSubmit(onSubmit)}>
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
                    <SubmitButton disabled={isPending || !haveFormValuesChanged}>
                        {isPending ? <ContentLoading size={24}/> : t('editSubmitButton')}
                    </SubmitButton>
                </form>
            )}
        </EmailTamerDialog>
    );
};

export default EditEmailBoxDialogForm;