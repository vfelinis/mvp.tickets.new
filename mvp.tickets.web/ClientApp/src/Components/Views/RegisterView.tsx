import { Box, Button, Typography } from '@mui/material';
import { FC, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../Store/RootStore';
import { IUserRegisterCommandRequest } from '../../Models/User';

interface IRegisterViewProps {
}

const RegisterView: FC<IRegisterViewProps> = (props) => {
    const store = useRootStore();
    const [searchParams] = useSearchParams();
    const email = searchParams.get('email');
    const code = searchParams.get('code');
    const [request, setRequest] = useState<IUserRegisterCommandRequest>({
        firstName: '',
        lastName: '',
        password: '',
        code: code ?? '',
    });

    const handleSubmit = () => {
        store.userStore.register(request);
    }

    return <>
        <Typography variant="h6" component="div">
            Регистрация
        </Typography>
        <Box component={ValidatorForm}
            onSubmit={handleSubmit}
            onError={(errors: any) => console.log(errors)}
            noValidate
            autoComplete="off"
            sx={{
                '& .MuiTextField-root': { mt: 2, width: '100%' },
            }}
        >
            <TextValidator
                label="Почта"
                name="email"
                value={email}
                disabled={true}
            />
            <TextValidator
                label="Имя"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setRequest({ ...request, firstName: e.currentTarget.value })}
                name="firstName"
                value={request.firstName}
                validators={['required', 'maxStringLength:50']}
                errorMessages={['Обязательное поле', 'Максимальная длина 50']}
            />
            <TextValidator
                label="Фамилия"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setRequest({ ...request, lastName: e.currentTarget.value })}
                name="lastName"
                value={request.lastName}
                validators={['required', 'maxStringLength:50']}
                errorMessages={['Обязательное поле', 'Максимальная длина 50']}
            />
            <TextValidator
                label="Пароль"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setRequest({ ...request, password: e.currentTarget.value })}
                type="password"
                name="password"
                value={request.password}
                validators={['required', 'maxStringLength:50']}
                errorMessages={['Обязательное поле', 'Максимальная длина 50']}
            />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Зарегистрироваться</Button>
                <Button component={Link} to={UIRoutesHelper.login.getRoute()}>Назад</Button>
            </Box>
        </Box>
    </>;
};

export default observer(RegisterView);
