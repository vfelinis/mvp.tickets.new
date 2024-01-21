import { FC } from 'react';
import { useRootStore } from '../../Store/RootStore';
import { observer } from 'mobx-react-lite';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';

interface IModalProps {
}

const Modal: FC<IModalProps> = (props) => {
    const store = useRootStore();

    const handleClose = () => {
        store.infoStore.setMessage(null);
    }

    return <>
        <Dialog
            open={!!store.infoStore.message}
            onClose={handleClose}
            aria-describedby="alert-dialog-description"
        >
            <DialogContent>
                <DialogContentText id="alert-dialog-description">
                    {store.infoStore.message}
                </DialogContentText>
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose} autoFocus>
                    Закрыть
                </Button>
            </DialogActions>
        </Dialog>
    </>;
};

export default observer(Modal);
