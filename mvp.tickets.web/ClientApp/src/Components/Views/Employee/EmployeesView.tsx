import { Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { FC, useEffect } from 'react';
import { UIRoutesHelper } from '../../../Helpers/UIRoutesHelper';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../Shared/TableComponent';
import { useRootStore } from '../../../Store/RootStore';
import { SortDirection } from '../../../Enums/SortDirection';
import { ITicketModel } from '../../../Models/Ticket';
import { ICategoryModel } from '../../../Models/Category';

interface IEmployeesViewProps {
}

const EmployeesView: FC<IEmployeesViewProps> = (props) => {
  const store = useRootStore();

    useEffect(() => {
        store.ticketStore.getReport({
            searchBy: null,
            sortBy: 'dateCreated',
            sortDirection: SortDirection.DESC,
            offset: 0
        });
        store.categoryStore.getCategories(true);
        store.priorityStore.getEntries(true);
        store.queueStore.getEntries(true);
        store.statusStore.getEntries(true);
    }, []);

    const actionHandle = (searchBy: object, offset: number, sortBy: string, direction: SortDirection): void => {
      store.ticketStore.getReport({
          searchBy: searchBy,
          sortBy: sortBy,
          sortDirection: direction,
          offset: offset
      });
  };

  const categories = store.categoryStore.categories.slice().sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0));
    const rootCategories = categories.filter(s => s.isRoot || !categories.some(x => x.id === s.parentCategoryId)).map(s => {
        return {
            parent: { ...s, isRoot: true },
            children: categories.filter(x => x.parentCategoryId == s.id)
        };
    });

    let options: ICategoryModel[] = [];
    rootCategories.forEach(s => {
        options = options.concat([s.parent, ...s.children]);
    });

  return <>
    <Typography variant="h6" component="div">
      Заявки
    </Typography>
    <TableComponent table={{
            options: {
                editRoute: (row: ITicketModel): string => UIRoutesHelper.employeeTicketDetail.getRoute(row.id),
                isServerSide: true,
                actionHandle: actionHandle,
                total: store.ticketStore.report.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'name', label: 'Название', type: ColumnType.String, sortable: false, searchable: false },
                { field: 'dateCreated', label: 'Создан', type: ColumnType.Date, sortable: true, searchable: false },
                { field: 'dateModified', label: 'Обновлен', type: ColumnType.Date, sortable: true, searchable: false },
                { field: 'source', label: 'Источник', type: ColumnType.String, sortable: false, searchable: true,
                    searchOptions: [
                        { id: '0', name: 'Web' },
                        { id: '1', name: 'Email' },
                        { id: '2', name: 'Telegram' }
                    ]
                },
                { field: 'reporterEmail', label: 'Создал', type: ColumnType.String, sortable: true, searchable: true },
                { field: 'assigneeEmail', label: 'Назначено', type: ColumnType.String, sortable: true, searchable: true },
                {
                    field: 'isClosed', label: 'Закрыт', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
                {
                    field: 'ticketPriority', proxyField: 'ticketPriorityId', label: 'Приоритет', type: ColumnType.Number, sortable: false, searchable: true,
                    searchOptions: store.priorityStore.entries.slice().sort((a,b) => a.level - b.level).map(s => { return { id: `${s.id}`, name: s.name } }),
                    skipOptionsSorting: true
                },
                {
                    field: 'ticketQueue', proxyField: 'ticketQueueId', label: 'Очередь', type: ColumnType.Number, sortable: false, searchable: true,
                    searchOptions: store.queueStore.entries.map(s => { return { id: `${s.id}`, name: s.name } })
                },
                {
                    field: 'ticketStatus', proxyField: 'ticketStatusId', label: 'Статус', type: ColumnType.Number, sortable: false, searchable: true,
                    searchOptions: store.statusStore.entries.map(s => { return { id: `${s.id}`, name: s.name } })
                },
                {
                    field: 'ticketCategory', proxyField: 'ticketCategoryId', label: 'Категория', type: ColumnType.Number, sortable: false, searchable: true,
                    searchOptions: options.map(s => { return { id: `${s.id}`, name: s.name, isMain: s.isRoot } }),
                    isOptionsGrouped: true
                },
            ],
            rows: store.ticketStore.report
        }} />
  </>;
};

export default observer(EmployeesView);
