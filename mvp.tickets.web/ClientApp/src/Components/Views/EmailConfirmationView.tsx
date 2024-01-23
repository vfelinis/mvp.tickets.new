import { Box, Button, Typography } from '@mui/material';
import { FC } from 'react';
import { Link } from 'react-router-dom';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';

interface IEmailConfirmationViewProps {
}

const EmailConfirmationView: FC<IEmailConfirmationViewProps> = (props) => {
    return <>
        <Typography variant="h6" component="div">
            Письмо с инструкцией отправлено на указанную электронную почту.
        </Typography>
        <Box sx={{ mt: 2, display: 'flex', alignItems: 'center', justifyContent: 'center' }} >
            <Button component={Link} to={UIRoutesHelper.home.getRoute()}>Назад</Button>
        </Box>
    </>;
};

export default EmailConfirmationView;
