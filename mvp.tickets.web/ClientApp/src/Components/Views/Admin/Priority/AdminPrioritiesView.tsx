import { FC, useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { Link } from 'react-router-dom';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IPriorityModel } from '../../../../Models/Priority';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../../Shared/TableComponent';

interface IAdminPrioritiesViewProps {
}

const AdminPrioritiesView: FC<IAdminPrioritiesViewProps> = (props) => {
    const store = useRootStore();

    useEffect(() => {
        store.priorityStore.getEntries();
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Приоритеты
        </Typography>
        <Button variant="contained" component={Link} to={UIRoutesHelper.adminPrioritiesCreate.getRoute()}>Создать</Button>
        <TableComponent table={{
            options: {
                editRoute: (row: IPriorityModel): string => UIRoutesHelper.adminPrioritiesUpdate.getRoute(row.id),
                isServerSide: false,
                total: store.priorityStore.entries.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'name', label: 'Название', type: ColumnType.String, sortable: true, searchable: true },
                { field: 'level', label: 'Уровень', type: ColumnType.Number, sortable: true, searchable: true },
                {
                    field: 'isActive', label: 'Активная запись', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
            ],
            rows: store.priorityStore.entries
        }} />
    </>;
};

export default observer(AdminPrioritiesView);
