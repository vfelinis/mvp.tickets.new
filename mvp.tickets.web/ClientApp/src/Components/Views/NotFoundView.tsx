import { Typography } from '@mui/material';
import { FC } from 'react';

interface INotFoundViewProps {
}

const NotFoundView: FC<INotFoundViewProps> = (props) => {
  return <>
    <Typography variant="h6" component="div">
      Страница не найдена.
    </Typography>
  </>;
};

export default NotFoundView;
