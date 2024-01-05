import { FC, useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { Link } from 'react-router-dom';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IQueueModel } from '../../../../Models/Queue';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../../Shared/TableComponent';

interface IAdminQueuesViewProps {
}

const AdminQueuesView: FC<IAdminQueuesViewProps> = (props) => {
    const store = useRootStore();

    useEffect(() => {
        store.queueStore.getEntries();
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Очереди
        </Typography>
        <Button variant="contained" component={Link} to={UIRoutesHelper.adminQueuesCreate.getRoute()}>Создать</Button>
        <TableComponent table={{
            options: {
                editRoute: (row: IQueueModel): string => UIRoutesHelper.adminQueuesUpdate.getRoute(row.id),
                isServerSide: false,
                total: store.queueStore.entries.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'name', label: 'Название', type: ColumnType.String, sortable: true, searchable: true },
                {
                    field: 'isDefault', label: 'Первичная запись', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
                {
                    field: 'isActive', label: 'Активная запись', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
            ],
            rows: store.queueStore.entries
        }} />
    </>;
};

export default observer(AdminQueuesView);
