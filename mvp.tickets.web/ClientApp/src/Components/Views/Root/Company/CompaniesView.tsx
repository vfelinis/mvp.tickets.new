import { Switch, Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { FC, useEffect } from 'react';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../../Shared/TableComponent';
import { useRootStore } from '../../../../Store/RootStore';
import { ICompanyModel } from '../../../../Models/Company';

interface ICompaniesViewProps {
}

const CompaniesView: FC<ICompaniesViewProps> = (props) => {
    const store = useRootStore();

    useEffect(() => {
        store.companyStore.getReport();
    }, []);

    return <>
        <Typography variant="h6" component="div">
            Предприятия
        </Typography>
        <TableComponent table={{
            options: {
                isServerSide: false,
                total: store.companyStore.report.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'name', label: 'Название', type: ColumnType.String, sortable: true, searchable: true },
                { field: 'host', label: 'Хост', type: ColumnType.String, sortable: true, searchable: true },
                { field: 'dateCreated', label: 'Создано', type: ColumnType.Date, sortable: true, searchable: false },
                {
                    field: 'isActive', label: 'Активное', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions,
                    valueFormater: (item: ICompanyModel, value: boolean): any => {
                        return <Switch checked={item.isActive} onChange={(e) => store.companyStore.setActive({ id: item.id, isActive: !item.isActive })} />;
                    }
                },
            ],
            rows: store.companyStore.report
        }} />
    </>;
};

export default observer(CompaniesView);
