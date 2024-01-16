import { Autocomplete, Box, Button, FormControlLabel, Switch, TextField, Typography } from '@mui/material';
import { FC, useState, useEffect } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../Store/RootStore';
import { ICompanyCreateCommandRequest } from '../../Models/Company';

interface ICompaniesCreateViewProps {
}

const CompaniesCreateView: FC<ICompaniesCreateViewProps> = (props) => {
    const store = useRootStore();
    const [searchParams] = useSearchParams();
    const email = searchParams.get('email');
    const code = searchParams.get('code');
    const [request, setRequest] = useState<ICompanyCreateCommandRequest>({
        name: '',
        host: '',
        email: email ?? '',
        password: '',
        code: code ?? '',
    });

    useEffect(() => {
        if (email && code) {
            store.inviteStore.validate({
                email: email,
                code: code
            });
        }
    }, []);

    const handleSubmit = () => {
        store.companyStore.create(request);
    }

    return <>
        <Typography variant="h6" component="div">
            Создать предприятие
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
                label="Название"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setRequest({ ...request, name: e.currentTarget.value })}
                name="name"
                value={request.name}
                validators={['required', 'maxStringLength:100']}
                errorMessages={['Обязательное поле', 'Максимальная длина 100']}
            />
            <TextValidator
                label="Префикс адреса сайта"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setRequest({ ...request, host: e.currentTarget.value })}
                name="host"
                value={request.host}
                validators={['required', 'maxStringLength:50', 'matchRegexp:^[a-z0-9]+$']}
                errorMessages={['Обязательное поле', 'Максимальная длина 50', 'Допустимы только цифры и латинские строчные буквы']}
            />
            <TextValidator
                label="Почта администратора"
                name="email"
                value={request.email}
                disabled={true}
            />
            <TextValidator
                label="Пароль администратора"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setRequest({ ...request, password: e.currentTarget.value })}
                type="password"
                name="password"
                value={request.password}
                validators={['required', 'maxStringLength:50']}
                errorMessages={['Обязательное поле', 'Максимальная длина 50']}
            />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit" disabled={!store.inviteStore.isValid}>Создать</Button>
            </Box>
        </Box>
    </>;
};

export default observer(CompaniesCreateView);
