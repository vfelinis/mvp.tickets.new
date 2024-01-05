import { Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState, useEffect, useLayoutEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IResponseTemplateTypeUpdateCommandRequest } from '../../../../Models/ResponseTemplateType';

interface IAdminResponseTemplateTypesUpdateViewProps {
}

const AdminResponseTemplateTypesUpdateView: FC<IAdminResponseTemplateTypesUpdateViewProps> = (props) => {
    const store = useRootStore();
    const [entry, setEntry] = useState<IResponseTemplateTypeUpdateCommandRequest>({
        id: 0,
        name: '',
        isActive: false,
    });
    const { id } = useParams();

    useEffect(() => {
        store.responseTemplateTypeStore.getDataForUpdateForm(Number(id));
        return () => {
            store.responseTemplateTypeStore.setEntry(null);
        };
    }, []);

    useLayoutEffect(() => {
        if (store.responseTemplateTypeStore.entry !== null) {
            setEntry({
                id: store.responseTemplateTypeStore.entry.id,
                name: store.responseTemplateTypeStore.entry.name,
                isActive: store.responseTemplateTypeStore.entry.isActive,
            });
        }
    }, [store.responseTemplateTypeStore.entry]);

    const handleSubmit = () => {
        store.responseTemplateTypeStore.update(entry);
    }

    return <>
        <Typography variant="h6" component="div">
            Редактировать тип шаблона
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
                onChange={(e: React.FormEvent<HTMLInputElement>) => setEntry({ ...entry, name: e.currentTarget.value })}
                name="name"
                value={entry.name}
                validators={['required', 'maxStringLength:100']}
                errorMessages={['Обязательное поле', 'Максимальная длина 100']}
            />
            <FormControlLabel
                control={<Switch checked={entry.isActive} onChange={(e) => setEntry({ ...entry, isActive: e.currentTarget.checked })} />}
                label="Активная запись" />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Сохранить</Button>
                <Button component={Link} to={UIRoutesHelper.adminResponseTemplateTypes.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminResponseTemplateTypesUpdateView);
