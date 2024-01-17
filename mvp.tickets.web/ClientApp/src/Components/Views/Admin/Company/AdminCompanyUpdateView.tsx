import { Box, Button, Typography } from '@mui/material';
import { FC, useState, useEffect, useLayoutEffect } from 'react';
import { Link, } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import React from 'react';
import { ICompanyUpdateCommandRequest } from '../../../../Models/Company';

interface IAdminCompanyUpdateViewProps {
}

const AdminCompanyUpdateView: FC<IAdminCompanyUpdateViewProps> = (props) => {
    const store = useRootStore();
    const [request, setRequest] = useState<ICompanyUpdateCommandRequest>({
        id: 0,
        name: '',
        host: '',
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
            });
        }
    }, [store.companyStore.company]);

    const handleSubmit = () => {
        store.companyStore.update(request);
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
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Сохранить</Button>
                <Button component={Link} to={UIRoutesHelper.home.getRoute()}>Назад</Button>
            </Box>
        </Box>
    </>;
};

export default observer(AdminCompanyUpdateView);
