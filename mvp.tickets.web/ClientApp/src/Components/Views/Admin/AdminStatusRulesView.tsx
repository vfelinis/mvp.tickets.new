import { Typography } from '@mui/material';
import { FC } from 'react';

interface IAdminStatusRulesViewProps {
}

const AdminStatusRulesView: FC<IAdminStatusRulesViewProps> = (props) => {
    return <>
        <Typography variant="h6" component="div">
            Правила статусов
        </Typography>
    </>;
};

export default AdminStatusRulesView;
