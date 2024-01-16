import { Button, Typography } from '@mui/material';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { FC, useEffect } from 'react';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import TableComponent, { ColumnType } from '../../../Shared/TableComponent';
import { useRootStore } from '../../../../Store/RootStore';
import { IInviteModel } from '../../../../Models/Invite';

interface IInvitesViewProps {
}

const InvitesView: FC<IInvitesViewProps> = (props) => {
    const store = useRootStore();

    useEffect(() => {
        store.inviteStore.getReport();
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Приглашения
        </Typography>
        <Button variant="contained" component={Link} to={UIRoutesHelper.invitesCreate.getRoute()}>Создать</Button>
        <TableComponent table={{
            options: {
                deleteHandle: (row: IInviteModel): void => store.inviteStore.delete(row.id),
                isServerSide: false,
                total: store.inviteStore.report.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'email', label: 'Почта', type: ColumnType.String, sortable: true, searchable: true },
                { field: 'company', label: 'Предприятие', type: ColumnType.String, sortable: true, searchable: true },
                { field: 'dateSent', label: 'Отправлено', type: ColumnType.Date, sortable: true, searchable: false },
            ],
            rows: store.inviteStore.report
        }} />
    </>;
};

export default observer(InvitesView);
