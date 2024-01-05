import { Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState, useEffect, useLayoutEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IStatusUpdateCommandRequest } from '../../../../Models/Status';

interface IAdminStatusesUpdateViewProps {
}

const AdminStatusesUpdateView: FC<IAdminStatusesUpdateViewProps> = (props) => {
    const store = useRootStore();
    const [entry, setEntry] = useState<IStatusUpdateCommandRequest>({
        id: 0,
        name: '',
        isDefault: false,
        isCompletion: false,
        isActive: false,
    });
    const { id } = useParams();

    useEffect(() => {
        store.statusStore.getDataForUpdateForm(Number(id));
        return () => {
            store.statusStore.setEntry(null);
        };
    }, []);

    useLayoutEffect(() => {
        if (store.statusStore.entry !== null) {
            setEntry({
                id: store.statusStore.entry.id,
                name: store.statusStore.entry.name,
                isDefault: store.statusStore.entry.isDefault,
                isCompletion: store.statusStore.entry.isCompletion,
                isActive: store.statusStore.entry.isActive,
            });
        }
    }, [store.statusStore.entry]);

    const handleSubmit = () => {
        store.statusStore.update(entry);
    }

    return <>
        <Typography variant="h6" component="div">
            Редактировать статус
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
                control={<Switch checked={entry.isDefault} onChange={(e) => setEntry({ ...entry, isDefault: e.currentTarget.checked })} />}
                label="Первичная запись" />
            <FormControlLabel
                control={<Switch checked={entry.isCompletion} onChange={(e) => setEntry({ ...entry, isCompletion: e.currentTarget.checked })} />}
                label="Финальная запись" />
            <FormControlLabel
                control={<Switch checked={entry.isActive} onChange={(e) => setEntry({ ...entry, isActive: e.currentTarget.checked })} />}
                label="Активная запись" />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Сохранить</Button>
                <Button component={Link} to={UIRoutesHelper.adminStatuses.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminStatusesUpdateView);
