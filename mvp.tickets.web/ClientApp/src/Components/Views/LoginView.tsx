import { Box, Button, Typography } from '@mui/material';
import { FC, useState } from 'react';
import { Navigate } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';

import Error from '../Shared/Error';
import { useRootStore } from '../../Store/RootStore';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';
import { IUserLoginCommandRequest } from '../../Models/User';

interface ILoginViewProps {
}

const LoginView: FC<ILoginViewProps> = (props) => {
  const store = useRootStore();
  const [request, setRequest] = useState<IUserLoginCommandRequest>({
    email: '',
    password: '',
  });

  const handleSubmit = () => {
    store.userStore.login(request);
  }

  return <>
    {store.userStore.currentUser !== null ? <Navigate to={UIRoutesHelper.home.getRoute()} replace={true} /> : null}
    <Error />
    <Typography variant="h6" component="div">
      Вход
    </Typography>
    <Box component={ValidatorForm}
      onSubmit={handleSubmit}
      onError={(errors: any) => console.log(errors)}
      noValidate
      autoComplete="off"
      sx={{
        '& .MuiTextField-root': { mt: 2, width: '100%' },
        maxWidth: 500
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
        label="Пароль"
        onChange={(e: React.FormEvent<HTMLInputElement>) => setRequest({ ...request, password: e.currentTarget.value })}
        type="password"
        name="password"
        value={request.password}
        validators={['required', 'maxStringLength:50']}
        errorMessages={['Обязательное поле', 'Максимальная длина 50']}
      />
      <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
        <Button type="submit">Войти</Button>
      </Box>
    </Box>

  </>;
};

export default observer(LoginView);
