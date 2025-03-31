import {useForm} from 'react-hook-form';
import {z} from 'zod';
import {zodResolver} from '@hookform/resolvers/zod';
import {useNavigate} from 'react-router-dom';
import {useRegister} from '@api/emailTamerApiComponents';
import {CircularProgress} from '@mui/material';
import {LOGIN_ROUTE} from '@router/routes';

import {getAppControlActions} from '@store/AppControlStore.ts';
import FormLayout from '@components/forms/FormLayout.tsx';
import TextInputControl from '@components/forms/controls/TextInputControl.tsx';
import NavigateButton from '@components/forms/controls/NavigateButton.tsx';
import SubmitButton from '@components/forms/controls/SubmitButton.tsx';

import PasswordInputControl from '@components/forms/controls/PasswordInputControl.tsx';

import {useState} from 'react';

import useScopedContextTranslator from '../i18n/hooks/useScopedTranslator.ts';
import {UserRole} from "@api/emailTamerApiSchemas.ts";

const createRegisterSchema = (t: (key: string) => string) =>
    z.object({
        email: z.string()
            .min(1, { message: t('validation.emailRequired') })
            .email({ message: t('validation.invalidEmail') }),
        password: z.string()
            .min(12, { message: t('validation.passwordMinLength') })
            .regex(/[a-z]/, { message: t('validation.passwordLowercase') })
            .regex(/[A-Z]/, { message: t('validation.passwordUppercase') })
            .regex(/[0-9]/, { message: t('validation.passwordDigit') }),
        confirmPassword: z.string()
            .min(1, { message: t('validation.confirmPasswordRequired') }),
    }).refine((data) => data.password === data.confirmPassword, {
        path: ['confirmPassword'],
        message: t('validation.passwordsDontMatch'),
    });

type RegisterFormData = z.infer<ReturnType<typeof createRegisterSchema>>;

const RegisterPage = () => {
    const {t} = useScopedContextTranslator();
    const navigate = useNavigate();
    const {setErrorNotification, setSuccessNotification} = getAppControlActions();

    const [showPassword, setShowPassword] = useState(false);
    const handleClickShowPassword = () => setShowPassword(!showPassword);

    const {mutate: register, isPending} = useRegister({
        onSuccess: () => {
            navigate(LOGIN_ROUTE);
            setSuccessNotification(t('success'));
        },
        onError: (error) => {
            setErrorNotification((error as any).payload || t('error'));
        },
    });

    const form = useForm<RegisterFormData>({
        resolver: zodResolver(createRegisterSchema(t)),
        defaultValues: {email: '', password: '', confirmPassword: ''},
    });

    const onSubmit = (data: RegisterFormData) => {
        register({
            body: {
                email: data.email,
                password: data.password,
                role: UserRole.User
            }
        });
    };

    return (
        <FormLayout
            title={t('title')}
            onSubmit={form.handleSubmit(onSubmit)}
        >
            <TextInputControl label={t('email')} form={form} id='email' />
            <PasswordInputControl
                showPassword={showPassword}
                handleClickShowPassword={handleClickShowPassword}
                label={t('password')}
                form={form}
                id='password'
            />
            <PasswordInputControl
                showPassword={showPassword}
                handleClickShowPassword={handleClickShowPassword}
                label={t('confirmPassword')}
                form={form}
                id='confirmPassword'
            />
            <SubmitButton disabled={isPending}>
                {isPending ? <CircularProgress size={24}/> : t('registerButton')}
            </SubmitButton>
            <NavigateButton route={LOGIN_ROUTE}>
                {t('toLogin')}
            </NavigateButton>
        </FormLayout>
    );
};

export default RegisterPage;