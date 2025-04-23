import {Card, CardContent, CardProps, Typography} from '@mui/material';
import {FormEventHandler, ReactNode} from 'react';

type FormLayoutProps = Omit <CardProps, 'onSubmit'> & {
    children: ReactNode;
    title?: string;
    onSubmit: FormEventHandler<HTMLFormElement>;
}

const FormLayout = ({children, title, onSubmit, ...restProps}: FormLayoutProps) => {
    return(
        <Card
            sx={{
                maxWidth: 400,
                mx: 'auto',
                my: 10,
                p: 3,
                pb: 0,
                borderRadius: 2,
                boxShadow: 3,
            }}
            {...restProps}
        >
            <Typography variant='h4' align='center' sx={{mt: 1}}>
                {title}
            </Typography>
            <CardContent component='form' onSubmit={onSubmit}>
                {children}
            </CardContent>
        </Card>
    );
};

export default FormLayout;