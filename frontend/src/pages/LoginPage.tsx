import {useForm} from 'react-hook-form';
import {z} from 'zod';
import {zodResolver} from '@hookform/resolvers/zod';
import {useNavigate} from 'react-router-dom';
import {useGetCurrentUser, useLogin} from '@api/emailTamerApiComponents';
import {HOME_ROUTE, REGISTER_ROUTE, LOGIN_ROUTE} from '@router/routes';
import {getAppControlActions} from '@store/AppControlStore.ts';
import FormLayout from '@components/forms/FormLayout.tsx';
import SubmitButton from '@components/forms/controls/SubmitButton.tsx';
import NavigateButton from '@components/forms/controls/NavigateButton.tsx';
import TextInputControl from '@components/forms/controls/TextInputControl.tsx';
import PasswordInputControl from '@components/forms/controls/PasswordInputControl.tsx';
import useScopedContextTranslator from '@hooks/useScopedTranslator.ts';
import {useEffect} from 'react';
import useAuthStore from '@store/AuthStore.ts';
import ContentLoading from '@components/ContentLoading.tsx';
import {REDIRECT_TO_PARAM} from '@router/urlParams.ts';
import Fieldset from '@components/forms/Fieldset.tsx';
import {getUrlParam} from '@utils/urlUtils.ts';

const createLoginSchema = (t: (key: string) => string) =>
    z.object({
        email: z
            .string()
            .min(1, {message: t('validation.emailRequired')})
            .email(t('validation.invalidEmail')),
        password: z.string().min(12, t('validation.passwordMinLength')),
    });

type LoginFormData = z.infer<ReturnType<typeof createLoginSchema>>;

const LoginPage = () => {
    const {t} = useScopedContextTranslator();
    const {setUser, setToken, isAuthenticated, logout} = useAuthStore((state) => ({
        setUser: state.setUser,
        setToken: state.setToken,
        isAuthenticated: state.isAuthenticated,
        logout: state.logout,
    }));
    const {setErrorNotification, setSuccessNotification} = getAppControlActions();

    const navigate = useNavigate();
    const redirectToValue = getUrlParam(REDIRECT_TO_PARAM);
    const redirect = () => {
        const target = redirectToValue && redirectToValue !== LOGIN_ROUTE ? redirectToValue : HOME_ROUTE;
        navigate(target, {replace: true});
    };

    const {
        isFetching: getCurrentUserProcessing,
        refetch: fetchCurrentUser,
        data: currentUserData,
    } = useGetCurrentUser(
        {},
        {
            onSuccess: (user) => {
                setUser(user);
                setSuccessNotification(t('success'));
            },
            onError: () => {
                logout();
                setErrorNotification(t('error'));
            },
            retry: false,
            enabled: false,
        }
    );

    useEffect(() => {
        if (currentUserData) {
            setUser(currentUserData);
            setSuccessNotification(t('success'));
        }
    }, [currentUserData, setUser]);

    useEffect(() => {
        const token = useAuthStore.getState().token;
        if (isAuthenticated) {
            redirect();
        } else if (token) {
            fetchCurrentUser();
        }
    }, [isAuthenticated, fetchCurrentUser]);

    const {mutate: login, isPending} = useLogin({
        onSuccess: (data) => {
            if (data.token) {
                setToken(data.token);
                fetchCurrentUser();
            }
        },
        onError: () => {
            setErrorNotification(t('error'));
        },
    });

    const form = useForm<LoginFormData>({
        resolver: zodResolver(createLoginSchema(t)),
        defaultValues: {email: '', password: ''},
    });

    const onSubmit = (data: LoginFormData) => {
        login({body: data});
    };

    const isLoading = isPending || getCurrentUserProcessing;

    return (
        <FormLayout title={t('title')} onSubmit={form.handleSubmit(onSubmit)}>
            <Fieldset disabled={isLoading}>
                <TextInputControl
                    autoFocus
                    type='email'
                    label={t('email')}
                    form={form}
                    id='email'
                />
                <PasswordInputControl
                    label={t('password')}
                    form={form}
                    id='password'
                />
                <SubmitButton disabled={isLoading}>
                    {isLoading ? <ContentLoading size={24}/> : t('loginButton')}
                </SubmitButton>
                <NavigateButton route={REGISTER_ROUTE}>{t('toRegister')}</NavigateButton>
            </Fieldset>
        </FormLayout>
    );
};

export default LoginPage;