import { FC, useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { Link } from 'react-router-dom';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IStatusModel } from '../../../../Models/Status';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../../Shared/TableComponent';

interface IAdminStatusesViewProps {
}

const AdminStatusesView: FC<IAdminStatusesViewProps> = (props) => {
    const store = useRootStore();

    useEffect(() => {
        store.statusStore.getEntries();
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Статусы
        </Typography>
        <Button variant="contained" component={Link} to={UIRoutesHelper.adminStatusesCreate.getRoute()}>Создать</Button>
        <TableComponent table={{
            options: {
                editRoute: (row: IStatusModel): string => UIRoutesHelper.adminStatusesUpdate.getRoute(row.id),
                isServerSide: false,
                total: store.statusStore.entries.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'name', label: 'Название', type: ColumnType.String, sortable: true, searchable: true },
                {
                    field: 'isDefault', label: 'Первичная запись', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },{
                    field: 'isCompletion', label: 'Финальная запись', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
                {
                    field: 'isActive', label: 'Активная запись', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
            ],
            rows: store.statusStore.entries
        }} />
    </>;
};

export default observer(AdminStatusesView);
