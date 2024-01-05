import { Typography } from '@mui/material';
import { observer } from 'mobx-react-lite';
import { FC, useEffect } from 'react';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../Shared/TableComponent';
import { useRootStore } from '../../Store/RootStore';
import { SortDirection } from '../../Enums/SortDirection';
import { ITicketModel } from '../../Models/Ticket';
import { ICategoryModel } from '../../Models/Category';

interface ITicketsViewProps {
}

const TicketsView: FC<ITicketsViewProps> = (props) => {
  const store = useRootStore();

    useEffect(() => {
        store.ticketStore.getReport({
            searchBy: null,
            sortBy: 'dateModified',
            sortDirection: SortDirection.DESC,
            offset: 0
        });
        store.categoryStore.getCategories(true);
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
      Мои заявки
    </Typography>
    <TableComponent table={{
            options: {
                editRoute: (row: ITicketModel): string => UIRoutesHelper.ticketsDetail.getRoute(row.id),
                isServerSide: true,
                actionHandle: actionHandle,
                total: store.ticketStore.report.length,
            },
            columns: [
                { field: 'id', label: 'Id', type: ColumnType.Number, sortable: true, searchable: true },
                { field: 'name', label: 'Название', type: ColumnType.String, sortable: false, searchable: false },
                {
                    field: 'isClosed', label: 'Закрыт', type: ColumnType.Boolean, sortable: false, searchable: true,
                    searchOptions: tableColumnBooleanSearchOptions
                },
                { field: 'dateModified', label: 'Обновлен', type: ColumnType.Date, sortable: true, searchable: false },
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

export default observer(TicketsView);
