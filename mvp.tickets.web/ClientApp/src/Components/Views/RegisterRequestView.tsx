import { Box, Button, Typography } from '@mui/material';
import { FC, useState } from 'react';
import { Link, Navigate } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';

import Error from '../Shared/Error';
import { useRootStore } from '../../Store/RootStore';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';
import { IUserRegisterRequestCommandRequest } from '../../Models/User';

interface IRegisterRequestViewProps {
}

const RegisterRequestView: FC<IRegisterRequestViewProps> = (props) => {
  const store = useRootStore();
  const [request, setRequest] = useState<IUserRegisterRequestCommandRequest>({
    email: '',
  });

  const handleSubmit = () => {
    store.userStore.registerRequest(request);
  }

  return <>
    {store.userStore.currentUser !== null ? <Navigate to={UIRoutesHelper.home.getRoute()} replace={true} /> : null}
    <Typography variant="h6" component="div">
      Запрос на регистрацию
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
      <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
        <Button type="submit">Отправить</Button>
        <Button component={Link} to={UIRoutesHelper.login.getRoute()}>Назад</Button>
      </Box>
    </Box>

  </>;
};

export default observer(RegisterRequestView);
