import { Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState, useEffect, useLayoutEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IResolutionUpdateCommandRequest } from '../../../../Models/Resolution';

interface IAdminResolutionsUpdateViewProps {
}

const AdminResolutionsUpdateView: FC<IAdminResolutionsUpdateViewProps> = (props) => {
    const store = useRootStore();
    const [entry, setEntry] = useState<IResolutionUpdateCommandRequest>({
        id: 0,
        name: '',
        isActive: false,
    });
    const { id } = useParams();

    useEffect(() => {
        store.resolutionStore.getDataForUpdateForm(Number(id));
        return () => {
            store.resolutionStore.setEntry(null);
        };
    }, []);

    useLayoutEffect(() => {
        if (store.resolutionStore.entry !== null) {
            setEntry({
                id: store.resolutionStore.entry.id,
                name: store.resolutionStore.entry.name,
                isActive: store.resolutionStore.entry.isActive,
            });
        }
    }, [store.resolutionStore.entry]);

    const handleSubmit = () => {
        store.resolutionStore.update(entry);
    }

    return <>
        <Typography variant="h6" component="div">
            Редактировать резолюцию
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
                <Button component={Link} to={UIRoutesHelper.adminResolutions.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminResolutionsUpdateView);
