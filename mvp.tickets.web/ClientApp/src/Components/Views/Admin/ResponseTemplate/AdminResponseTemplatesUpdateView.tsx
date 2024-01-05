import { Autocomplete, Box, Button, FormControlLabel, Switch, Typography, TextField } from '@mui/material';
import { FC, useState, useEffect, useLayoutEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { IResponseTemplateUpdateCommandRequest } from '../../../../Models/ResponseTemplate';
import { useRootStore } from '../../../../Store/RootStore';
import { IResponseTemplateTypeModel } from '../../../../Models/ResponseTemplateType';

interface IAdminResponseTemplatesUpdateViewProps {
}

const AdminResponseTemplatesUpdateView: FC<IAdminResponseTemplatesUpdateViewProps> = (props) => {
    const store = useRootStore();
    const [entry, setEntry] = useState<IResponseTemplateUpdateCommandRequest>({ id: 0, name: '', text: '', isActive: true, ticketResponseTemplateTypeId: 0 });
    const [selectedType, setSelectedType] = useState<IResponseTemplateTypeModel | null>(null);
    const { id } = useParams();

    const handleSubmit = () => {
        store.responseTemplateStore.update(entry);
    }

    const onTypeSelect = (value: IResponseTemplateTypeModel | null) : void => {
        setSelectedType(value);
        setEntry({...entry, ticketResponseTemplateTypeId: value?.id ?? 0});
    };

    useEffect(() => {
        store.responseTemplateStore.getDataForUpdateForm(Number(id));
        store.responseTemplateTypeStore.getEntries(true);
        return () => {
            store.responseTemplateStore.setEntry(null);
            store.responseTemplateTypeStore.setEntries([]);
        };
    }, []);

    useLayoutEffect(() => {
        if (store.responseTemplateStore.entry !== null) {
            setEntry({
                id: store.responseTemplateStore.entry.id,
                name: store.responseTemplateStore.entry.name,
                text: store.responseTemplateStore.entry.text,
                isActive: store.responseTemplateStore.entry.isActive,
                ticketResponseTemplateTypeId: store.responseTemplateStore.entry.ticketResponseTemplateTypeId
            });
            setSelectedType({
                id: store.responseTemplateStore.entry.ticketResponseTemplateTypeId,
                name: store.responseTemplateStore.entry.ticketResponseTemplateType,
                isActive: false,
                dateCreated: new Date(),
                dateModified: new Date()
            });
        }
    }, [store.responseTemplateStore.entry]);

    return <>
        <Typography variant="h6" component="div">
            Редактировать шаблон
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
                value={selectedType}
                options={store.responseTemplateTypeStore.entries.slice()
                    .concat(selectedType !== null && !store.responseTemplateTypeStore.entries.some(s => s.id === selectedType.id) ? [selectedType] : [])
                    .sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0))}
                getOptionLabel={option => option.name}
                onChange={(event, value) => onTypeSelect(value)}
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
                <Button type="submit">Сохранить</Button>
                <Button component={Link} to={UIRoutesHelper.adminResponseTemplates.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminResponseTemplatesUpdateView);
