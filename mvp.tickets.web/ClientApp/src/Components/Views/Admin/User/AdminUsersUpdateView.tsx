import { Autocomplete, Box, Button, FormControlLabel, Switch, TextField, Typography } from '@mui/material';
import { FC, useState, useEffect, useLayoutEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IUserUpdateCommandRequest } from '../../../../Models/User';
import { hasPermission, Permissions } from '../../../../Enums/Permissions';

interface IAdminUsersUpdateViewProps {
}

interface IOption {
    id: Permissions,
    name: string
}

const AdminUsersUpdateView: FC<IAdminUsersUpdateViewProps> = (props) => {
    const store = useRootStore();
    const [user, setUser] = useState<IUserUpdateCommandRequest>({
        id: 0,
        email: '',
        firstName: '',
        lastName: '',
        isLocked: false,
        password: '',
        permissions: Permissions.None,
    });
    const [permissions, setPermissions] = useState<IOption[]>([]);
    const [isChangePass, setIsChangePass] = useState(false);
    const { id } = useParams();

    const permissionOptions: IOption[] = [
        { id: Permissions.User, name: 'Пользователь' },
        { id: Permissions.Employee, name: 'Сотрудник' },
        { id: Permissions.Admin, name: 'Администратор' },
    ];

    useEffect(() => {
        store.userStore.getDataForUpdateForm(Number(id));
        return () => {
            store.userStore.setEditableUser(null);
        };
    }, []);

    useLayoutEffect(() => {
        if (store.userStore.editableUser !== null) {
            setUser({
                id: store.userStore.editableUser.id,
                email: store.userStore.editableUser.email,
                firstName: store.userStore.editableUser.firstName,
                lastName: store.userStore.editableUser.lastName,
                isLocked: store.userStore.editableUser.isLocked,
                password: '',
                permissions: store.userStore.editableUser.permissions
            });
            let existingPermissions: IOption[] = [];
            permissionOptions.forEach((item) => {
                if (hasPermission(store.userStore.editableUser.permissions, item.id)) {
                    existingPermissions.push(item);
                }
            });
            setPermissions(existingPermissions);
        }
    }, [store.userStore.editableUser]);

    const handleSubmit = () => {
        permissions.forEach((item) => {
            user.permissions |= item.id;
        });
        store.userStore.update(user);
    }

    return <>
        <Typography variant="h6" component="div">
            Редактировать пользователя
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
                value={permissions}
                options={permissionOptions}
                getOptionLabel={option => option.name}
                onChange={(event, value) => setPermissions(value)}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                renderInput={(params) => <TextField {...params} label="Доступ" />}
            />
            <FormControlLabel
                control={<Switch checked={isChangePass} onChange={(e) => setIsChangePass(e.currentTarget.checked)} />}
                label="Изменить пароль" />
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
                <Button type="submit">Сохранить</Button>
                <Button component={Link} to={UIRoutesHelper.adminUsers.getRoute()}>Назад</Button>
            </Box>


        </Box>
    </>;
};

export default observer(AdminUsersUpdateView);
