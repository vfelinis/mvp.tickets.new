import { FC, useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { Link } from 'react-router-dom';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IResolutionModel } from '../../../../Models/Resolution';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../../Shared/TableComponent';

interface IAdminResolutionsViewProps {
}

const AdminResolutionsView: FC<IAdminResolutionsViewProps> = (props) => {
    const store = useRootStore();

    useEffect(() => {
        store.resolutionStore.getEntries();
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Резолюции
        </Typography>
        <Button variant="contained" component={Link} to={UIRoutesHelper.adminResolutionsCreate.getRoute()}>Создать</Button>
        <TableComponent table={{
            options: {
                editRoute: (row: IResolutionModel): string => UIRoutesHelper.adminResolutionsUpdate.getRoute(row.id),
                isServerSide: false,
                total: store.resolutionStore.entries.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'name', label: 'Название', type: ColumnType.String, sortable: true, searchable: true },
                {
                    field: 'isActive', label: 'Активная запись', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
            ],
            rows: store.resolutionStore.entries
        }} />
    </>;
};

export default observer(AdminResolutionsView);
