import { Button, Typography } from '@mui/material';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { FC, useEffect } from 'react';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../../Shared/TableComponent';
import { useRootStore } from '../../../../Store/RootStore';
import { SortDirection } from '../../../../Enums/SortDirection';
import { IUserModel } from '../../../../Models/User';
import { Permissions, hasPermission } from '../../../../Enums/Permissions';

interface IAdminUsersViewProps {
}

const AdminUsersView: FC<IAdminUsersViewProps> = (props) => {
    const store = useRootStore();

    useEffect(() => {
        store.userStore.getReport({
            searchBy: null,
            sortBy: 'id',
            sortDirection: SortDirection.ASC,
            offset: 0
        });
    }, []);

    const actionHandle = (searchBy: object, offset: number, sortBy: string, direction: SortDirection): void => {
        store.userStore.getReport({
            searchBy: searchBy,
            sortBy: sortBy,
            sortDirection: direction,
            offset: offset
        });
    };

    return <>
        <Typography variant="h6" component="div">
            Пользователи
        </Typography>
        <Button variant="contained" component={Link} to={UIRoutesHelper.adminUsersCreate.getRoute()}>Создать</Button>
        <TableComponent table={{
            options: {
                editRoute: (row: IUserModel): string => UIRoutesHelper.adminUsersUpdate.getRoute(row.id),
                isServerSide: true,
                actionHandle: actionHandle,
                total: store.userStore.report.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'email', label: 'Почта', type: ColumnType.String, sortable: true, searchable: true },
                { field: 'firstName', label: 'Имя', type: ColumnType.String, sortable: true, searchable: true },
                { field: 'lastName', label: 'Фамилия', type: ColumnType.String, sortable: true, searchable: true },
                {
                    field: 'permissions', label: 'Доступ', type: ColumnType.Number, sortable: false, searchable: true,
                    valueFormater: (value: Permissions): string => {
                        let label = '';
                        if (hasPermission(value, Permissions.Admin)) {
                            label += 'Администратор; ';
                        }
                        if (hasPermission(value, Permissions.Employee)) {
                            label += 'Сотрудник; ';
                        }
                        if (hasPermission(value, Permissions.User)) {
                            label += 'Пользователь;';
                        }
                        return label;
                    },
                    searchOptions: [
                        { id: `${Permissions.Admin}`, name: "Администратор" },
                        { id: `${Permissions.Employee}`, name: "Сотрудник" },
                        { id: `${Permissions.User}`, name: "Пользователь" },
                    ]
                },
                {
                    field: 'isLocked', label: 'Заблокирован', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
            ],
            rows: store.userStore.report
        }} />
    </>;
};

export default observer(AdminUsersView);
