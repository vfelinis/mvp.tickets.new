import { Autocomplete, Box, Button, FormControlLabel, Switch, TextField, Typography } from '@mui/material';
import { FC, useState } from 'react';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IInviteCreateCommandRequest } from '../../../../Models/Invite';

interface IInvitesCreateViewProps {
}

const InvitesCreateView: FC<IInvitesCreateViewProps> = (props) => {
    const store = useRootStore();
    const [request, setRequest] = useState<IInviteCreateCommandRequest>({
        email: '',
        company: '',
    });

    const handleSubmit = () => {
        store.inviteStore.create(request);
    }

    return <>
        <Typography variant="h6" component="div">
            Отправить приглашение
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
                label="Почта"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setRequest({ ...request, email: e.currentTarget.value })}
                name="email"
                value={request.email}
                validators={['required', 'maxStringLength:250', 'isEmail']}
                errorMessages={['Обязательное поле', 'Максимальная длина 250', 'Некорректная почта']}
            />
            <TextValidator
                label="Предприятие"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setRequest({ ...request, company: e.currentTarget.value })}
                name="company"
                value={request.company}
                validators={['required', 'maxStringLength:100']}
                errorMessages={['Обязательное поле', 'Максимальная длина 100']}
            />
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Отправить</Button>
                <Button component={Link} to={UIRoutesHelper.invites.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(InvitesCreateView);
