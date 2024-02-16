import { Button, Typography } from '@mui/material';
import { Link } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { FC, useEffect } from 'react';
import { UIRoutesHelper } from '../../../../Helpers/UIRoutesHelper';
import TableComponent, { ColumnType, tableColumnBooleanSearchOptions } from '../../../Shared/TableComponent';
import { useRootStore } from '../../../../Store/RootStore';
import { SortDirection } from '../../../../Enums/SortDirection';

interface IMetricsViewProps {
}

const MetricsView: FC<IMetricsViewProps> = (props) => {
    const store = useRootStore();
    return <>
        <Typography variant="h6" component="div">
            Метрики
        </Typography>
        <div style={{
            position: 'relative',
            overflow: 'hidden',
            width: '100%',
            paddingTop: '56.25%'
        }}>
            <iframe style={{
                position: 'absolute',
                top: 0,
                left: 0,
                bottom: 0,
                right: 0,
                width: '100%',
                height: '100%'
            }} src="https://grafana.mvp-stack.ru/d/ssuhGl2Ik/node-exporter-nodes?orgId=1&refresh=30s" />
        </div>
    </>;
};

export default observer(MetricsView);
