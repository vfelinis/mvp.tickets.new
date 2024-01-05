import { Autocomplete, Box, Button, FormControlLabel, Switch, Typography } from '@mui/material';
import { FC, useState, useEffect, Children } from 'react';
import { Link, useParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../Helpers/UIRoutesHelper';
import { ITicketCommentCreateCommandRequest } from '../../../Models/Ticket';
import { useRootStore } from '../../../Store/RootStore';
import FileUpload from 'react-material-file-upload';

interface IEmployeesTicketCommentCreateViewProps {
}

const EmployeesTicketCommentCreateView: FC<IEmployeesTicketCommentCreateViewProps> = (props) => {
    const [files, setFiles] = useState<File[]>([]);
    const store = useRootStore();
    const [entry, setEntry] = useState<ITicketCommentCreateCommandRequest>({ text: '', isInternal: false });
    const { id } = useParams();

    const handleSubmit = () => {
        const formData = new FormData();
        files.map((file, index) => {
            console.log(file.size);
            return formData.append('files', file, file.name);
        });
        formData.append("isInternal", entry.isInternal.toString());
        if (entry.text) {
            formData.append("text", entry.text);
        }
        store.ticketStore.createComment(Number(id), false, formData);
    }


    return <>
        <Typography variant="h6" component="div">
            Создать комментарий
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
                multiline
                label="Текст"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setEntry({ ...entry, text: e.currentTarget.value })}
                name="text"
                value={entry.text}
                validators={['maxStringLength:2000']}
                errorMessages={['Максимальная длина 2000']}
            />
            <FormControlLabel
                control={<Switch checked={entry.isInternal} onChange={(e) => setEntry({ ...entry, isInternal: e.currentTarget.checked })} />}
                label="Внутренний" />
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
                <Button component={Link} to={UIRoutesHelper.employeeTicketDetail.getRoute(Number(id))}>Назад</Button>
            </Box>
        </Box>
    </>;
};

export default observer(EmployeesTicketCommentCreateView);
