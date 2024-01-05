import { FC, useState, useEffect, useMemo } from 'react';
import { Link } from 'react-router-dom';
import Box from '@mui/material/Box';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import TableSortLabel from '@mui/material/TableSortLabel';
import Paper from '@mui/material/Paper';
import EditIcon from '@mui/icons-material/Edit';
import { visuallyHidden } from '@mui/utils';
import { formatDistanceToNow } from 'date-fns';
import { ru } from 'date-fns/locale';
import IconButton from '@mui/material/IconButton';
import { SortDirection } from '../../Enums/SortDirection';
import TablePagination from '@mui/material/TablePagination';
import debounce from 'lodash/debounce';
import TextField from '@mui/material/TextField';
import Autocomplete from '@mui/material/Autocomplete';
import Typography from '@mui/material/Typography';

interface ITableComponentProps {
    table: ITable
}

export enum ColumnType {
    String = 1,
    Number = 2,
    Date = 3,
    Boolean = 4
}

interface ITableColumnValueFormater {
    (value: any): string;
}

interface ITableColumnSearchOption {
    id: string
    name: string
    isMain?: boolean
}

export const tableColumnBooleanSearchOptions: ITableColumnSearchOption[] = [
    { id: 'True', name: "да" },
    { id: 'False', name: "нет" },
];

interface ITableColumn {
    field: string
    proxyField?: string | null
    label: string
    type: ColumnType
    sortable?: boolean
    searchable?: boolean
    searchOptions?: ITableColumnSearchOption[] | undefined
    isOptionsGrouped?: boolean
    skipOptionsSorting?: boolean
    valueFormater?: ITableColumnValueFormater | undefined
}

interface IActionHandle {
    (searchBy: object, offset: number, sortBy: string, direction: SortDirection): void;
}

interface ITableOptions {
    isServerSide: boolean
    total: number
    editRoute: Function | undefined
    actionHandle?: IActionHandle | undefined
}

interface ITable {
    options: ITableOptions
    columns: ITableColumn[]
    rows: any[]
}

const TableComponent: FC<ITableComponentProps> = (props) => {
    const [direction, setDirection] = useState(SortDirection.ASC);
    const [searchBy, setSearchBy] = useState({});
    const [page, setPage] = useState(0);
    const [sortBy, setSortBy] = useState(props.table.columns[0].field);
    const limit = 30;

    const getValue = (item: any, column: ITableColumn): any => {
        return column.type === ColumnType.Date
            ? new Date(item[column.field])
            : item[column.field];
    };

    const getFormatedValue = (item: any, column: ITableColumn): any => {
        let value: any;
        switch (column.type) {
            case ColumnType.Date:
                value = formatDistanceToNow(new Date(item[column.field]), { addSuffix: true, locale: ru });
                break;
            case ColumnType.Boolean:
                value = item[column.field] === true ? 'да' : 'нет';
                break;
            default:
                value = item[column.field];
                break;
        }
        return column.valueFormater !== undefined
            ? column.valueFormater(value)
            : value;
    };

    const order = direction === SortDirection.ASC ? 'asc' : 'desc';

    const getField = (field: string) : string => {
        const column = props.table.columns.find(s => s.field === field);
        return column?.proxyField ?? column?.field ?? '';
    };

    const handleSort = (field: string): void => {
        if (props.table.options.isServerSide && props.table.options.actionHandle !== undefined) {
            props.table.options.actionHandle(searchBy, getOffset(page), getField(field), direction * -1);
        }
        setDirection(direction * -1);
        setSortBy(field);
    };

    const handleSearch = (field: string, value: any): void => {
        const newSearchBy = { ...searchBy, [field]: value };
        if (props.table.options.isServerSide && props.table.options.actionHandle !== undefined) {
            let searchByToSend = {};
            for (let [key, value] of Object.entries(newSearchBy)) {
                const newKey = getField(key);
                searchByToSend = {...searchByToSend, [newKey]: value};
            }
            props.table.options.actionHandle(searchByToSend, 0, getField(sortBy), direction);
        }
        setSearchBy(newSearchBy);
        setPage(0);
    };

    const debouncedHandleSearch = useMemo(
        () => debounce(handleSearch, 1000)
        , []);

    useEffect(() => {
        return () => {
            debouncedHandleSearch.cancel();
        }
    }, []);

    const sortedColumn = props.table.columns.find(s => s.field === sortBy);

    let sortedRows = [...props.table.rows];
    if (sortedColumn) {
        sortedRows = sortedRows.sort((a: any, b: any) => {
            const aValue = getValue(a, sortedColumn);
            const bValue = getValue(b, sortedColumn);
            return aValue > bValue ? direction : bValue > aValue ? direction * -1 : 0;
        });
    }

    const getOffset = (page: number): number => (page) * limit;

    const handleChangePage = (
        event: React.MouseEvent<HTMLButtonElement> | null,
        newPage: number,
    ) => {
        if (props.table.options.isServerSide && props.table.options.actionHandle !== undefined) {
            props.table.options.actionHandle(searchBy, getOffset(newPage), sortBy, direction);
        }
        setPage(newPage);
    };

    let total = props.table.options.total;
    if (props.table.options.isServerSide === false) {
        if (Object.entries(searchBy).some(s => s[1])) {
            sortedRows = sortedRows.filter(s => {
                let success = true;
                for (let [key, value] of Object.entries(searchBy)) {
                    if (value) {
                        if (typeof s[key] === typeof Boolean) {
                            if (s[key] !== (value === 'True' ? true : false)) {
                                success = false;
                            }
                        } else if (`${s[key]}`.indexOf(`${value}`.toLowerCase()) === -1) {
                            success = false;
                        }
                    }
                }
                return success;
            });
            total = sortedRows.length;
        }
        sortedRows = sortedRows.splice(getOffset(page), limit);
    }

    return <>
        <TablePagination
            component="div"
            count={total}
            page={page}
            onPageChange={handleChangePage}
            rowsPerPage={limit}
            rowsPerPageOptions={[limit]}
            labelDisplayedRows={(data: any): string => `${data.from}–${data.to} из ${data.count !== -1 ? data.count : `больше чем ${data.to}`}`}
        />
        <TableContainer sx={{ mt: 2 }} component={Paper}>
            <Table>
                <TableHead>
                    <TableRow>
                        {props.table.columns.map((column, i) => {
                            const label = column.searchOptions === undefined
                                ? <TextField
                                    label={column.label}
                                    variant="standard"
                                    type="search"
                                    disabled={column.searchable !== true}
                                    onChange={(e) => props.table.options.isServerSide === false
                                        ? handleSearch(column.field, e.currentTarget.value)
                                        : debouncedHandleSearch(column.field, e.currentTarget.value)
                                    }
                                    onClick={(e) => e.stopPropagation()}
                                />
                                : <Autocomplete
                                    forcePopupIcon={false}
                                    componentsProps={{
                                        paper: {
                                            sx: {
                                                minWidth: 200,
                                                wordBreak: 'break-all'
                                            }
                                        }
                                    }}
                                    options={column.isOptionsGrouped === true || column.skipOptionsSorting === true
                                        ? column.searchOptions
                                        : column.searchOptions.slice().sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0))}
                                    getOptionLabel={option => option.name}
                                    renderOption={(props, option) => (
                                        <Box component="li" {...props}>
                                            {
                                                column.isOptionsGrouped == true
                                                    ? <Typography sx={option.isMain ? { fontWeight: 700 } : { pl: 1 }}>{option.name}</Typography>
                                                    : <>{option.name}</>
                                            }

                                        </Box>
                                    )}
                                    onChange={(event, value) => handleSearch(column.field, value?.id)}
                                    isOptionEqualToValue={(option, value) => option.id === value.id}
                                    renderInput={(params) => <TextField {...params} label={column.label} variant="standard" />}
                                />;
                            return column.sortable
                                ? <TableCell
                                    key={i}
                                    sortDirection={sortBy === column.field ? order : false}
                                    sx={{ minWidth: 150 }}
                                >

                                    <TableSortLabel
                                        active={sortBy === column.field}
                                        direction={sortBy === column.field ? order : 'asc'}
                                        onClick={() => handleSort(column.field)}
                                    >
                                        {sortBy === column.field ? (
                                            <Box component="span" sx={visuallyHidden}>
                                                {order === 'desc' ? 'sorted descending' : 'sorted ascending'}
                                            </Box>
                                        ) : null}
                                        {label}
                                    </TableSortLabel>
                                </TableCell>
                                : <TableCell key={i} sx={{ minWidth: 150 }}>
                                    {label}
                                </TableCell>;
                        })}
                        {props.table.options.editRoute && <TableCell></TableCell>}
                    </TableRow>
                </TableHead>
                <TableBody>
                    {sortedRows.map((row, i) => (
                        <TableRow
                            key={i}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            {props.table.columns.map((column, i) => (
                                <TableCell key={i}>{getFormatedValue(row, column)}</TableCell>
                            ))}
                            {props.table.options.editRoute !== undefined &&
                                <TableCell>
                                    <IconButton component={Link} to={props.table.options.editRoute(row)}>
                                        <EditIcon />
                                    </IconButton>
                                </TableCell>
                            }
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    </>;
};

export default TableComponent;
