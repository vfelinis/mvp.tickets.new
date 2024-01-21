import { Typography } from '@mui/material';
import { FC } from 'react';
import { useRootStore } from '../../Store/RootStore';

interface IHomeViewProps {
}

const HomeView: FC<IHomeViewProps> = (props) => {
  const store = useRootStore();
  return <>
    <Typography variant="h6" component="div">
      Добро пожаловать - {store.userStore.currentUser?.firstName} {store.userStore.currentUser?.lastName}
    </Typography>
  </>;
};

export default HomeView;
