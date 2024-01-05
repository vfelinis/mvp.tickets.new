import { Autocomplete, Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { IResponseTemplateCreateCommandRequest } from '../../../../Models/ResponseTemplate';
import { useRootStore } from '../../../../Store/RootStore';

interface IAdminResponseTemplatesCreateViewProps {
}

const AdminResponseTemplatesCreateView: FC<IAdminResponseTemplatesCreateViewProps> = (props) => {
    const store = useRootStore();
    const [entry, setEntry] = useState<IResponseTemplateCreateCommandRequest>({ name: '', text: '', isActive: true, ticketResponseTemplateTypeId: 0 });

    const handleSubmit = () => {
        store.responseTemplateStore.create(entry);
    }

    useEffect(() => {
        store.responseTemplateTypeStore.getEntries(true);
        return () => {
            store.responseTemplateTypeStore.setEntries([]);
        };
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Создать шаблон
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
                multiline
                label="Текст"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setEntry({ ...entry, text: e.currentTarget.value })}
                name="text"
                value={entry.text}
                validators={['required', 'maxStringLength:2000']}
                errorMessages={['Обязательное поле', 'Максимальная длина 2000']}
            />
            <FormControlLabel
                control={<Switch checked={entry.isActive} onChange={(e) => setEntry({ ...entry, isActive: e.currentTarget.checked })} />}
                label="Активная запись" />
            <Autocomplete
                disablePortal
                options={store.responseTemplateTypeStore.entries.slice().sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0))}
                getOptionLabel={option => option.name}
                onChange={(event, value) => setEntry({ ...entry, ticketResponseTemplateTypeId: value?.id ?? 0 })}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                renderInput={(params) => <TextValidator
                    {...params}
                    value={params.inputProps.value}
                    label="Тип"
                    name="ticketResponseTemplateTypeId"
                    validators={['required']}
                    errorMessages={['Обязательное поле']}
                />}
            />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Создать</Button>
                <Button component={Link} to={UIRoutesHelper.adminResponseTemplates.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminResponseTemplatesCreateView);
