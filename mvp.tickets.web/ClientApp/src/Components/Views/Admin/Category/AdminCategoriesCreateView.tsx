import { Autocomplete, Box, Button, FormControlLabel, Switch, TextField, Typography } from '@mui/material';
import { FC, useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { ICategoryCreateCommandRequest } from '../../../../Models/Category';
import { useRootStore } from '../../../../Store/RootStore';
import React from 'react';

interface IAdminCategoriesCreateViewProps {
}

const AdminCategoriesCreateView: FC<IAdminCategoriesCreateViewProps> = (props) => {
    const store = useRootStore();
    const [category, setCategory] = useState<ICategoryCreateCommandRequest>({ name: '', isActive: true, isDefault: false, parentCategoryId: null });

    const handleSubmit = () => {
        store.categoryStore.create(category);
    }

    useEffect(() => {
        store.categoryStore.getDataForCreateForm();
        return () => {
            store.categoryStore.setCategories([]);
        };
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Создать категорию
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
                onChange={(e: React.FormEvent<HTMLInputElement>) => setCategory({ ...category, name: e.currentTarget.value })}
                name="name"
                value={category.name}
                validators={['required', 'maxStringLength:100']}
                errorMessages={['Обязательное поле', 'Максимальная длина 100']}
            />
            <FormControlLabel
                control={<Switch checked={category.isActive} onChange={(e) => setCategory({ ...category, isActive: e.currentTarget.checked })} />}
                label="Активная запись" />
            <FormControlLabel
                control={<Switch checked={category.isDefault} onChange={(e) => setCategory({ ...category, isDefault: e.currentTarget.checked })} />}
                label="Запись по умолчанию" />
            <Autocomplete
                disablePortal
                options={store.categoryStore.categories.slice().sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0))}
                getOptionLabel={option => option.name}
                onChange={(event, value) => setCategory({ ...category, parentCategoryId: value?.id ?? null })}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                renderInput={(params) => <TextField {...params} label="Родительская категория" />}
            />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Создать</Button>
                <Button component={Link} to={UIRoutesHelper.adminCategories.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminCategoriesCreateView);
