import {useForm} from 'react-hook-form';
import {z} from 'zod';
import {zodResolver} from '@hookform/resolvers/zod';
import {useNavigate} from 'react-router-dom';
import {useLogin} from '@api/emailTamerApiComponents';
import {CircularProgress} from '@mui/material';
import {HOME_ROUTE, REGISTER_ROUTE} from '@router/routes';

import {getUserActions} from '@store/AuthStore.ts';

import TextInputControl from '@components/forms/controls/TextInputControl.tsx';

import {getAppControlActions} from '@store/AppControlStore.ts';

import FormLayout from '@components/forms/FormLayout.tsx';
import SubmitButton from '@components/forms/controls/SubmitButton.tsx';
import NavigateButton from '@components/forms/controls/NavigateButton.tsx';

import PasswordInputControl from '@components/forms/controls/PasswordInputControl.tsx';

import useScopedContextTranslator from '../i18n/hooks/useScopedTranslator.ts';

const createLoginSchema = (t: (key: string) => string) =>
    z.object({
        email: z.string()
            .min(1, { message: t('validation.emailRequired') })
            .email(t('validation.invalidEmail')),
        password: z.string().min(12, t('validation.passwordMinLength')),
    });

type LoginFormData = z.infer<ReturnType<typeof createLoginSchema>>;

const LoginPage = () => {
    const {t} = useScopedContextTranslator();
    const navigate = useNavigate();
    const {setAccessToken} = getUserActions();
    const {setErrorNotification, setSuccessNotification} = getAppControlActions();

    const {mutate: login, isPending} = useLogin({
        onSuccess: (data) => {
            if (data.token) {
                setAccessToken(data.token);
                navigate(HOME_ROUTE);
                setSuccessNotification(t('success'));
            }
        },
        onError: (error) => {
            setErrorNotification((error as any).payload || t('error'));
        },
    });

    const form = useForm<LoginFormData>({
        resolver: zodResolver(createLoginSchema(t)),
        defaultValues: {email: '', password: ''},
    });

    const onSubmit = (data: LoginFormData) => {
        login({body: data});
    };

    return (
        <FormLayout
            title={t('title')}
            onSubmit={form.handleSubmit(onSubmit)}
        >
            <TextInputControl label={t('email')} form={form} id='email' />
            <PasswordInputControl label={t('password')} form={form} id='password' />
            <SubmitButton disabled={isPending}>
                {isPending ? <CircularProgress size={24}/> : t('loginButton')}
            </SubmitButton>
            <NavigateButton route={REGISTER_ROUTE}>
                {t('toRegister')}
            </NavigateButton>
        </FormLayout>
    );
};

export default LoginPage;