import { Autocomplete, Box, Button, FormControlLabel, Switch, TextField, Typography } from '@mui/material';
import { FC, useState } from 'react';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IUserCreateCommandRequest } from '../../../../Models/User';
import { Permissions } from '../../../../Enums/Permissions';

interface IAdminUsersCreateViewProps {
}

interface IOption {
    id: Permissions,
    name: string
}

const AdminUsersCreateView: FC<IAdminUsersCreateViewProps> = (props) => {
    const store = useRootStore();
    const [user, setUser] = useState<IUserCreateCommandRequest>({
        email: '',
        firstName: '',
        lastName: '',
        isLocked: false,
        password: '',
        permissions: Permissions.None,
    });
    const [permissions, setPermissions] = useState<IOption[]>([]);
    const [isChangePass, setIsChangePass] = useState(false);

    const handleSubmit = () => {
        permissions.forEach((item) => {
            user.permissions |= item.id;
        });
        store.userStore.create(user);
    }

    const permissionOptions: IOption[] = [
        {id:Permissions.User, name: 'Пользователь'},
        {id:Permissions.Employee, name: 'Сотрудник'},
        {id:Permissions.Admin, name: 'Администратор'},
    ];

    return <>
        <Typography variant="h6" component="div">
            Создать пользователя
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
                onChange={(e: React.FormEvent<HTMLInputElement>) => setUser({ ...user, email: e.currentTarget.value })}
                name="email"
                value={user.email}
                validators={['required', 'maxStringLength:250', 'isEmail']}
                errorMessages={['Обязательное поле', 'Максимальная длина 250', 'Некорректная почта']}
            />
            <TextValidator
                label="Имя"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setUser({ ...user, firstName: e.currentTarget.value })}
                name="firstName"
                value={user.firstName}
                validators={['required', 'maxStringLength:50']}
                errorMessages={['Обязательное поле', 'Максимальная длина 50']}
            />
            <TextValidator
                label="Фамилия"
                onChange={(e: React.FormEvent<HTMLInputElement>) => setUser({ ...user, lastName: e.currentTarget.value })}
                name="lastName"
                value={user.lastName}
                validators={['required', 'maxStringLength:50']}
                errorMessages={['Обязательное поле', 'Максимальная длина 50']}
            />
            <FormControlLabel
                control={<Switch checked={user.isLocked} onChange={(e) => setUser({ ...user, isLocked: e.currentTarget.checked })} />}
                label="Заблокировать" />
            <Autocomplete
                disablePortal
                multiple
                options={permissionOptions}
                getOptionLabel={option => option.name}
                onChange={(event, value) => setPermissions(value)}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                renderInput={(params) => <TextField {...params} label="Доступ" />}
            />
            <FormControlLabel
                control={<Switch checked={isChangePass} onChange={(e) => setIsChangePass(e.currentTarget.checked)} />}
                label="Установить пароль" />
            {
                isChangePass &&
                <TextValidator
                    label="Пароль"
                    onChange={(e: React.FormEvent<HTMLInputElement>) => setUser({ ...user, password: e.currentTarget.value })}
                    type="password"
                    name="password"
                    value={user.password}
                // validators={['required', 'maxStringLength:50']}
                // errorMessages={['Обязательное поле', 'Максимальная длина 50']}
                />
            }
            <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
                <Button type="submit">Создать</Button>
                <Button component={Link} to={UIRoutesHelper.adminUsers.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminUsersCreateView);
