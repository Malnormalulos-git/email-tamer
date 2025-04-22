import {useForm} from 'react-hook-form';
import {z} from 'zod';
import {zodResolver} from '@hookform/resolvers/zod';
import {useEffect, useState} from 'react';
import {Stack} from '@mui/material';

import EmailTamerDialog from '@components/forms/EmailTamerDialog.tsx';
import TextInputControl from '@components/forms/controls/TextInputControl.tsx';
import PasswordInputControl from '@components/forms/controls/PasswordInputControl.tsx';
import SubmitButton from '@components/forms/controls/SubmitButton.tsx';
import ContentLoading from '@components/ContentLoading.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {useEditEmailBox, useGetEmailBoxDetails} from '@api/emailTamerApiComponents.ts';
import {getAppControlActions} from '@store/AppControlStore.ts';
import DoubleLabeledSwitch from '@components/forms/controls/DoubleLabeledSwitch.tsx';
import LabeledCheckbox from '@components/forms/controls/LabeledCheckbox.tsx';
import Fieldset from '@components/forms/Fieldset.tsx';

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
    }).refine((data) => data.authenticateByEmail || !!data.userName, {
        message: t('validation.userNameRequired'),
        path: ['userName'],
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
            slotProps={{
                paper: {
                    component: 'form',
                    onSubmit: form.handleSubmit(onSubmit),
                },
            }}
            dialogActions={
                <SubmitButton disabled={isPending || !haveFormValuesChanged}>
                    {isPending ? <ContentLoading size={24}/> : t('editSubmitButton')}
                </SubmitButton>
            }
        >
            {isFetching ? (
                <ContentLoading/>
            ) : (
                <Fieldset disabled={isPending}>
                    <TextInputControl
                        autoFocus
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
                        showPassword={showPassword}
                        handleClickShowPassword={handleClickShowPassword}
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
                            disabled={isPending}
                        />
                        <LabeledCheckbox
                            label={t('useSSl')}
                            id={'useSSl'}
                            form={form}
                            disabled={isPending}
                        />
                        <LabeledCheckbox
                            label={t('useDefaultImapPorts')}
                            id={'useDefaultImapPorts'}
                            form={form}
                            disabled={isPending}
                        />
                    </Stack>
                </Fieldset>
            )}
        </EmailTamerDialog>
    );
};

export default EditEmailBoxDialogForm;