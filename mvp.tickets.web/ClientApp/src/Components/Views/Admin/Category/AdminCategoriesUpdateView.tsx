import { Autocomplete, Box, Button, FormControlLabel, Switch, TextField, Typography } from '@mui/material';
import { FC, useState, useEffect, useLayoutEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { ICategoryModel, ICategoryUpdateCommandRequest } from '../../../../Models/Category';
import { useRootStore } from '../../../../Store/RootStore';
import React from 'react';

interface IAdminCategoriesUpdateViewProps {
}

const AdminCategoriesUpdateView: FC<IAdminCategoriesUpdateViewProps> = (props) => {
    const store = useRootStore();
    const [category, setCategory] = useState<ICategoryUpdateCommandRequest>({ id: 0, name: '', isActive: true, isDefault: false, parentCategoryId: null });
    const [selectedParent, setSelectedParent] = useState<ICategoryModel | null>(null);
    const { id } = useParams();

    const handleSubmit = () => {
        store.categoryStore.update(category);
    }

    const onParentSelect = (value: ICategoryModel | null) : void => {
        setSelectedParent(value);
        setCategory({...category, parentCategoryId: value?.id ?? null});
    };

    useEffect(() => {
        store.categoryStore.getDataForUpdateForm(Number(id));
        return () => {
            store.categoryStore.setCategory(null);
            store.categoryStore.setCategories([]);
        };
    }, []);

    useLayoutEffect(() => {
        setCategory({
            id: store.categoryStore.category?.id ?? 0,
            name: store.categoryStore.category?.name ?? '',
            isActive: store.categoryStore.category?.isActive ?? true,
            isDefault: store.categoryStore.category?.isDefault ?? false,
            parentCategoryId: store.categoryStore.category?.parentCategoryId ?? null
        });
        const parent = store.categoryStore.categories.find(s => s.id === store.categoryStore.category?.parentCategoryId);
        if (parent) {
            setSelectedParent(parent);
        }
    }, [store.categoryStore.categories]);

    return <>
        <Typography variant="h6" component="div">
            Редактировать категорию
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
                value={selectedParent}
                options={store.categoryStore.categories.filter(s => s.id !== category.id).sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0))}
                getOptionLabel={option => option.name}
                onChange={(event, value) => onParentSelect(value)}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                renderInput={(params) => <TextField {...params} label="Родительская категория" />}
            />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Сохранить</Button>
                <Button component={Link} to={UIRoutesHelper.adminCategories.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminCategoriesUpdateView);
