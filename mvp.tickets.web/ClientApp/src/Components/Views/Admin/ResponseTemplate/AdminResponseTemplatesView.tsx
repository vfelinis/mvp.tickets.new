import { FC, useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { Link } from 'react-router-dom';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IResponseTemplateModel } from '../../../../Models/ResponseTemplate';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../../Shared/TableComponent';

interface IAdminResponseTemplatesViewProps {
}

const AdminResponseTemplatesView: FC<IAdminResponseTemplatesViewProps> = (props) => {
    const store = useRootStore();

    useEffect(() => {
        store.responseTemplateStore.getEntries();
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Шаблоны
        </Typography>
        <Button variant="contained" component={Link} to={UIRoutesHelper.adminResponseTemplatesCreate.getRoute()}>Создать</Button>
        <TableComponent table={{
            options: {
                editRoute: (row: IResponseTemplateModel): string => UIRoutesHelper.adminResponseTemplatesUpdate.getRoute(row.id),
                isServerSide: false,
                total: store.responseTemplateStore.entries.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'name', label: 'Название', type: ColumnType.String, sortable: true, searchable: true },
                { field: 'ticketResponseTemplateType', label: 'Тип', type: ColumnType.String, sortable: true, searchable: true },
                {
                    field: 'isActive', label: 'Активная запись', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
            ],
            rows: store.responseTemplateStore.entries
        }} />
    </>;
};

export default observer(AdminResponseTemplatesView);
