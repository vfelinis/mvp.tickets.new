import { Autocomplete, Box, Button, Typography } from '@mui/material';
import { FC, useState, useEffect, Children } from 'react';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';
import { ITicketCreateCommandRequest } from '../../Models/Ticket';
import { useRootStore } from '../../Store/RootStore';
import FileUpload from 'react-material-file-upload';
import { ICategoryModel } from '../../Models/Category';

interface ICreateTicketViewProps {
}

const CreateTicketView: FC<ICreateTicketViewProps> = (props) => {
    const [files, setFiles] = useState<File[]>([]);
    const store = useRootStore();
    const [entry, setEntry] = useState<ITicketCreateCommandRequest>({ name: '', ticketCategoryId: 0, text: '' });

    useEffect(() => {
        store.categoryStore.getCategories(true);
        return () => {
            store.categoryStore.setCategories([]);
        };
    }, []);

    const handleSubmit = () => {
        const formData = new FormData();
        files.map((file, index) => {
            console.log(file.size);
            return formData.append('files', file, file.name);
        });
        formData.append("name", entry.name);
        formData.append("ticketCategoryId", entry.ticketCategoryId.toString());
        if (entry.text) {
            formData.append("text", entry.text);
        }
        store.ticketStore.create(formData);
    }

    const categories = store.categoryStore.categories.slice().sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0));
    const rootCategories = categories.filter(s => s.isRoot || !categories.some(x => x.id === s.parentCategoryId)).map(s => {
        return {
            parent: { ...s, isRoot: true },
            children: categories.filter(x => x.parentCategoryId == s.id)
        };
    });

    let options: ICategoryModel[] = [];
    rootCategories.forEach(s => {
        options = options.concat([s.parent, ...s.children]);
    });


    return <>
        <Typography variant="h6" component="div">
            Создать заявку
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
                validators={['maxStringLength:2000']}
                errorMessages={['Максимальная длина 2000']}
            />
            <Autocomplete
                disablePortal
                options={options}
                getOptionLabel={option => option.name}
                renderOption={(props, option) => (
                    <Box component="li" {...props}>
                        <Typography sx={option.isRoot ? { fontWeight: 700 } : { pl: 1 }}>{option.name}</Typography>
                    </Box>
                )}
                onChange={(event, value) => setEntry({ ...entry, ticketCategoryId: value?.id ?? 0 })}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                renderInput={(params) => <TextValidator
                    {...params}
                    value={params.inputProps.value}
                    label="Категория"
                    name="ticketCategoryId"
                    validators={['required']}
                    errorMessages={['Обязательное поле']}
                />}
            />
            <FileUpload
                value={files}
                onChange={setFiles}
                title="Перетащите несколько файлов сюда или нажмите, чтобы выбрать файлы"
                buttonText="Загрузка" sx={{ mt: 2 }}
                maxFiles={5}
                maxSize={4000000}
            />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Создать</Button>
                <Button component={Link} to={UIRoutesHelper.tickets.getRoute()}>Назад</Button>
            </Box>
        </Box>
    </>;
};

export default observer(CreateTicketView);
