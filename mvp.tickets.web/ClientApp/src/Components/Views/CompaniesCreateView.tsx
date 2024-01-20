import { Box, Button, Typography } from '@mui/material';
import { FC, useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { useRootStore } from '../../Store/RootStore';
import { ICompanyCreateCommandRequest } from '../../Models/Company';
import FileUpload from 'react-material-file-upload';
import { MuiColorInput } from 'mui-color-input'

interface ICompaniesCreateViewProps {
}

const CompaniesCreateView: FC<ICompaniesCreateViewProps> = (props) => {
    const store = useRootStore();
    const [searchParams] = useSearchParams();
    const email = searchParams.get('email');
    const code = searchParams.get('code');
    const [files, setFiles] = useState<File[]>([]);
    const [request, setRequest] = useState<ICompanyCreateCommandRequest>({
        name: '',
        host: '',
        email: email ?? '',
        password: '',
        code: code ?? '',
        color: '#1976d2'
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
        const formData = new FormData();
        if (files.length) {
            formData.append('logo', files[0], files[0].name);
        }
        formData.append("name", request.name);
        formData.append("host", request.host);
        formData.append("email", request.email);
        formData.append("password", request.password);
        formData.append("code", request.code);
        formData.append("color", request.color);
        store.companyStore.create(formData);
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
            <MuiColorInput format={'hex'} value={request.color} onChange={(value) => setRequest({ ...request, color: value })} />
            <FileUpload
                value={files}
                onChange={setFiles}
                title="Перетащите логотип сюда или нажмите, чтобы выбрать"
                buttonText="Загрузка" sx={{ mt: 2 }}
                maxFiles={1}
                maxSize={4000000}
                multiple={false}
                accept="image/*"
            />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit" disabled={!store.inviteStore.isValid}>Создать</Button>
            </Box>
        </Box>
    </>;
};

export default observer(CompaniesCreateView);
