import { Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState } from 'react';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { IPriorityCreateCommandRequest } from '../../../../Models/Priority';
import { useRootStore } from '../../../../Store/RootStore';

interface IAdminPrioritiesCreateViewProps {
}

const AdminPrioritiesCreateView: FC<IAdminPrioritiesCreateViewProps> = (props) => {
    const store = useRootStore();
    const [entry, setEntry] = useState<IPriorityCreateCommandRequest>({ name: '', level: 0, isActive: true });

    const handleSubmit = () => {
        store.priorityStore.create(entry);
    }

    return <>
        <Typography variant="h6" component="div">
            Создать приоритет
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
                <Button type="submit">Создать</Button>
                <Button component={Link} to={UIRoutesHelper.adminPriorities.getRoute()}>Назад</Button>
            </Box>
        </Box>
    </>;
};

export default observer(AdminPrioritiesCreateView);
