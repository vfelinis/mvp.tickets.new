import { FC } from 'react';
import { useRootStore } from '../../Store/RootStore';
import { observer } from 'mobx-react-lite';
import { Alert } from '@mui/material';

interface IErrorProps {
}

const Error: FC<IErrorProps> = (props) => {
    const store = useRootStore();
    return <>
        {store.infoStore.errors.map((s, i) => <Alert key={i} onClose={() => store.infoStore.clearErrors(i)} severity="error">{s}</Alert>)}
    </>;
};

export default observer(Error);
