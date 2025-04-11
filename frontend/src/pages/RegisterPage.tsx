import {useForm} from 'react-hook-form';
import {z} from 'zod';
import {zodResolver} from '@hookform/resolvers/zod';
import {useNavigate} from 'react-router-dom';
import {useRegister} from '@api/emailTamerApiComponents';
import {HOME_ROUTE, LOGIN_ROUTE} from '@router/routes';

import {getAppControlActions} from '@store/AppControlStore.ts';
import FormLayout from '@components/forms/FormLayout.tsx';
import TextInputControl from '@components/forms/controls/TextInputControl.tsx';
import NavigateButton from '@components/forms/controls/NavigateButton.tsx';
import SubmitButton from '@components/forms/controls/SubmitButton.tsx';

import PasswordInputControl from '@components/forms/controls/PasswordInputControl.tsx';

import {useEffect, useState} from 'react';

import {UserRole} from '@api/emailTamerApiSchemas.ts';

import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import useAuthStore from '@store/AuthStore.ts';
import ContentLoading from '@components/ContentLoading.tsx';

const createRegisterSchema = (t: (key: string) => string) =>
    z.object({
        email: z.string()
            .min(1, {message: t('validation.emailRequired')})
            .email({message: t('validation.invalidEmail')}),
        password: z.string()
            .min(12, {message: t('validation.passwordMinLength')})
            .regex(/[a-z]/, {message: t('validation.passwordLowercase')})
            .regex(/[A-Z]/, {message: t('validation.passwordUppercase')})
            .regex(/[0-9]/, {message: t('validation.passwordDigit')}),
        confirmPassword: z.string()
            .min(1, {message: t('validation.confirmPasswordRequired')}),
    }).refine((data) => data.password === data.confirmPassword, {
        path: ['confirmPassword'],
        message: t('validation.passwordsDontMatch'),
    });

type RegisterFormData = z.infer<ReturnType<typeof createRegisterSchema>>;

const RegisterPage = () => {
    const {t} = useScopedContextTranslator();
    const navigate = useNavigate();
    const isAuthenticated = useAuthStore((state) => state.isAuthenticated);

    useEffect(() => {
        if (isAuthenticated) {
            navigate(HOME_ROUTE, {replace: true});
        }
    }, [isAuthenticated, navigate]);

    const {setErrorNotification, setSuccessNotification} = getAppControlActions();

    const [showPassword, setShowPassword] = useState(false);
    const handleClickShowPassword = () => setShowPassword(!showPassword);

    const {mutate: register, isPending} = useRegister({
        onSuccess: () => {
            navigate(LOGIN_ROUTE);
            setSuccessNotification(t('success'));
        },
        onError: () => {
            setErrorNotification(t('error'));
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
            <TextInputControl autoFocus type='email' disabled={isPending} label={t('email')} form={form} id='email'/>
            <PasswordInputControl
                disabled={isPending}
                showPassword={showPassword}
                handleClickShowPassword={handleClickShowPassword}
                label={t('password')}
                form={form}
                id='password'
            />
            <PasswordInputControl
                disabled={isPending}
                showPassword={showPassword}
                handleClickShowPassword={handleClickShowPassword}
                label={t('confirmPassword')}
                form={form}
                id='confirmPassword'
            />
            <SubmitButton disabled={isPending}>
                {isPending ? <ContentLoading size={24}/> : t('registerButton')}
            </SubmitButton>
            <NavigateButton route={LOGIN_ROUTE}>
                {t('toLogin')}
            </NavigateButton>
        </FormLayout>
    );
};

export default RegisterPage;