import { Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState, useEffect, useLayoutEffect } from 'react';
import { Link, } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import React from 'react';
import FileUpload from 'react-material-file-upload';
import { MuiColorInput } from 'mui-color-input'
import { ICompanyUpdateCommandRequest } from '../../../../Models/Company';

interface IAdminCompanyUpdateViewProps {
}

const AdminCompanyUpdateView: FC<IAdminCompanyUpdateViewProps> = (props) => {
    const store = useRootStore();
    const [files, setFiles] = useState<File[]>([]);
    const [request, setRequest] = useState<ICompanyUpdateCommandRequest>({
        id: 0,
        name: '',
        host: '',
        color: '#1976d2',
        removeLogo: false,
        logo: null
    });

    useEffect(() => {
        store.companyStore.getDataForUpdateForm(store.userStore.currentUser!.companyId);
        return () => {
            store.companyStore.setCompany(null);
        };
    }, []);

    useLayoutEffect(() => {
        if (store.companyStore.company !== null) {
            setRequest({
                id: store.companyStore.company.id,
                name: store.companyStore.company.name,
                host: store.companyStore.company.host,
                color: store.companyStore.company.color,
                removeLogo: false,
                logo: store.companyStore.company.logo
            });
        }
    }, [store.companyStore.company]);

    const handleSubmit = () => {
        const formData = new FormData();
        if (files.length) {
            formData.append('newLogo', files[0], files[0].name);
        }
        formData.append("id", request.id.toString());
        formData.append("name", request.name);
        formData.append("host", request.host);
        formData.append("removeLogo", request.removeLogo.toString());
        formData.append("color", request.color);
        store.companyStore.update(request.id, formData);
    }

    return <>
        <Typography variant="h6" component="div">
            Редактировать предприятие
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
            <MuiColorInput format={'hex'} value={request.color} onChange={(value) => setRequest({ ...request, color: value })} />
            {
                request.logo &&
                <>
                    <Box
                        component="img"
                        sx={{
                            height: 100,
                            marginTop: 2
                        }}
                        alt="Logo"
                        src={request.logo}
                    />
                    <Box>
                        <FormControlLabel
                            control={<Switch checked={request.removeLogo} onChange={(e) => setRequest({ ...request, removeLogo: e.currentTarget.checked })} />}
                            label="Удалить логотип" />
                    </Box>
                </>
            }

            <FileUpload
                value={files}
                onChange={setFiles}
                title="Перетащите новый логотип сюда или нажмите, чтобы выбрать"
                buttonText="Загрузка" sx={{ mt: 2 }}
                maxFiles={1}
                maxSize={4000000}
                multiple={false}
                accept="image/*"
            />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Сохранить</Button>
                <Button component={Link} to={UIRoutesHelper.home.getRoute()}>Назад</Button>
            </Box>
        </Box>
    </>;
};

export default observer(AdminCompanyUpdateView);
