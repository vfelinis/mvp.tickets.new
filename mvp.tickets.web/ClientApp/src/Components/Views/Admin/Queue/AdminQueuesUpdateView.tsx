import { Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState, useEffect, useLayoutEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IQueueUpdateCommandRequest } from '../../../../Models/Queue';

interface IAdminQueuesUpdateViewProps {
}

const AdminQueuesUpdateView: FC<IAdminQueuesUpdateViewProps> = (props) => {
    const store = useRootStore();
    const [entry, setEntry] = useState<IQueueUpdateCommandRequest>({
        id: 0,
        name: '',
        isDefault: false,
        isActive: false,
    });
    const { id } = useParams();

    useEffect(() => {
        store.queueStore.getDataForUpdateForm(Number(id));
        return () => {
            store.queueStore.setEntry(null);
        };
    }, []);

    useLayoutEffect(() => {
        if (store.queueStore.entry !== null) {
            setEntry({
                id: store.queueStore.entry.id,
                name: store.queueStore.entry.name,
                isDefault: store.queueStore.entry.isDefault,
                isActive: store.queueStore.entry.isActive,
            });
        }
    }, [store.queueStore.entry]);

    const handleSubmit = () => {
        store.queueStore.update(entry);
    }

    return <>
        <Typography variant="h6" component="div">
            Редактировать очередь
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
                control={<Switch checked={entry.isActive} onChange={(e) => setEntry({ ...entry, isActive: e.currentTarget.checked })} />}
                label="Активная запись" />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Сохранить</Button>
                <Button component={Link} to={UIRoutesHelper.adminQueues.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminQueuesUpdateView);
