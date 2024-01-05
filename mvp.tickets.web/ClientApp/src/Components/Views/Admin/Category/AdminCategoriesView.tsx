import { FC, useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { Link } from 'react-router-dom';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../../Store/RootStore';
import { ICategoryModel } from '../../../../Models/Category';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../../Shared/TableComponent';
import React from 'react';


interface IAdminCategoriesViewProps {
}

const AdminCategoriesView: FC<IAdminCategoriesViewProps> = (props) => {
    const store = useRootStore();

    useEffect(() => {
        store.categoryStore.getCategories();
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Категории
        </Typography>
        <Button variant="contained" component={Link} to={UIRoutesHelper.adminCategoriesCreate.getRoute()}>Создать</Button>
        <TableComponent table={{
            options: {
                editRoute: (row: ICategoryModel): string => UIRoutesHelper.adminCategoriesUpdate.getRoute(row.id),
                isServerSide: false,
                total: store.categoryStore.categories.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'name', label: 'Название', type: ColumnType.String, sortable: true, searchable: true },
                { field: 'parentCategory', label: 'Родитель', type: ColumnType.String, sortable: true, searchable: true },
                {
                    field: 'isRoot', label: 'Корневая', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
                {
                    field: 'isActive', label: 'Активная запись', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
                {
                    field: 'isDefault', label: 'Запись по умолчанию', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
            ],
            rows: store.categoryStore.categories
        }} />
    </>;
};

export default observer(AdminCategoriesView);
