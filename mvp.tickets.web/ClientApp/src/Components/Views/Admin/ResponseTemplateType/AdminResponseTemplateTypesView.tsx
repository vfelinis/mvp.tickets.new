import { FC, useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { Link } from 'react-router-dom';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { IResponseTemplateTypeModel } from '../../../../Models/ResponseTemplateType';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../../Shared/TableComponent';

interface IAdminResponseTemplateTypesViewProps {
}

const AdminResponseTemplateTypesView: FC<IAdminResponseTemplateTypesViewProps> = (props) => {
    const store = useRootStore();

    useEffect(() => {
        store.responseTemplateTypeStore.getEntries();
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Типы шаблонов
        </Typography>
        <Button variant="contained" component={Link} to={UIRoutesHelper.adminResponseTemplateTypesCreate.getRoute()}>Создать</Button>
        <TableComponent table={{
            options: {
                editRoute: (row: IResponseTemplateTypeModel): string => UIRoutesHelper.adminResponseTemplateTypesUpdate.getRoute(row.id),
                isServerSide: false,
                total: store.responseTemplateTypeStore.entries.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'name', label: 'Название', type: ColumnType.String, sortable: true, searchable: true },
                {
                    field: 'isActive', label: 'Активная запись', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
            ],
            rows: store.responseTemplateTypeStore.entries
        }} />
    </>;
};

export default observer(AdminResponseTemplateTypesView);
