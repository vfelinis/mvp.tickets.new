import { Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState, useEffect, useLayoutEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IPriorityUpdateCommandRequest } from '../../../../Models/Priority';

interface IAdminPrioritiesUpdateViewProps {
}

const AdminPrioritiesUpdateView: FC<IAdminPrioritiesUpdateViewProps> = (props) => {
    const store = useRootStore();
    const [entry, setEntry] = useState<IPriorityUpdateCommandRequest>({
        id: 0,
        name: '',
        level: 0,
        isActive: false,
    });
    const { id } = useParams();

    useEffect(() => {
        store.priorityStore.getDataForUpdateForm(Number(id));
        return () => {
            store.priorityStore.setEntry(null);
        };
    }, []);

    useLayoutEffect(() => {
        if (store.priorityStore.entry !== null) {
            setEntry({
                id: store.priorityStore.entry.id,
                name: store.priorityStore.entry.name,
                level: store.priorityStore.entry.level,
                isActive: store.priorityStore.entry.isActive,
            });
        }
    }, [store.priorityStore.entry]);

    const handleSubmit = () => {
        store.priorityStore.update(entry);
    }

    return <>
        <Typography variant="h6" component="div">
            Редактировать приоритет
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
            <TextValidator
                label="Уровень"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setEntry({ ...entry, level: +e.currentTarget.value })}
                name="level"
                type="number"
                value={entry.level}
                validators={['required', 'matchRegexp:^[0-9]+$']}
                errorMessages={['Обязательное поле', 'Требуется целое число']}
            />
            <FormControlLabel
                control={<Switch checked={entry.isActive} onChange={(e) => setEntry({ ...entry, isActive: e.currentTarget.checked })} />}
                label="Активная запись" />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Сохранить</Button>
                <Button component={Link} to={UIRoutesHelper.adminPriorities.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminPrioritiesUpdateView);
