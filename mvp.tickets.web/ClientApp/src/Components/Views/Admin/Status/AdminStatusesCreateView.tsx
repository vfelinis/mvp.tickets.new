import { Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState } from 'react';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { IStatusCreateCommandRequest } from '../../../../Models/Status';
import { useRootStore } from '../../../../Store/RootStore';

interface IAdminStatusesCreateViewProps {
}

const AdminStatusesCreateView: FC<IAdminStatusesCreateViewProps> = (props) => {
    const store = useRootStore();
    const [entry, setEntry] = useState<IStatusCreateCommandRequest>({ name: '', isDefault: false, isCompletion: false, isActive: true });

    const handleSubmit = () => {
        store.statusStore.create(entry);
    }

    return <>
        <Typography variant="h6" component="div">
            Создать статус
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
                <Button type="submit">Создать</Button>
                <Button component={Link} to={UIRoutesHelper.adminStatuses.getRoute()}>Назад</Button>
            </Box>
        </Box>
    </>;
};

export default observer(AdminStatusesCreateView);
