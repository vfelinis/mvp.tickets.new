import { Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState } from 'react';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { IResolutionCreateCommandRequest } from '../../../../Models/Resolution';
import { useRootStore } from '../../../../Store/RootStore';

interface IAdminResolutionsCreateViewProps {
}

const AdminResolutionsCreateView: FC<IAdminResolutionsCreateViewProps> = (props) => {
    const store = useRootStore();
    const [entry, setEntry] = useState<IResolutionCreateCommandRequest>({ name: '', isActive: true });

    const handleSubmit = () => {
        store.resolutionStore.create(entry);
    }

    return <>
        <Typography variant="h6" component="div">
            Создать резолюцию
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
                <Button type="submit">Создать</Button>
                <Button component={Link} to={UIRoutesHelper.adminResolutions.getRoute()}>Назад</Button>
            </Box>
        </Box>
    </>;
};

export default observer(AdminResolutionsCreateView);
