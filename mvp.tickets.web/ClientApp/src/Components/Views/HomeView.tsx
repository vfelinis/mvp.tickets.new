import { Typography } from '@mui/material';
import { FC } from 'react';

interface IHomeViewProps {
}

const HomeView: FC<IHomeViewProps> = (props) => {
  return <>
    <Typography variant="h6" component="div">
      Добро пожаловать
    </Typography>
  </>;
};

export default HomeView;
