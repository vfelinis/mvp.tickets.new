import { Autocomplete, Box, Button, Card, Divider, Grid, List, ListItem, ListItemText, Typography, Link as MuiLink } from '@mui/material';
import { FC, useState, useEffect, Children } from 'react';
import { Link, useParams, useSearchParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { ValidatorForm, TextValidator } from 'react-material-ui-form-validator';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';
import { ITicketCreateCommandRequest } from '../../Models/Ticket';
import { useRootStore } from '../../Store/RootStore';
import FileUpload from 'react-material-file-upload';
import { ICategoryModel } from '../../Models/Category';
import { formatDistanceToNow } from 'date-fns';
import { ru } from 'date-fns/locale';
import React from 'react';

interface ITicketDetailViewProps {
}

const TicketDetailView: FC<ITicketDetailViewProps> = (props) => {
    const store = useRootStore();
    const { id } = useParams();
    const [searchParams] = useSearchParams();
    useEffect(() => {
        store.ticketStore.getEntry(Number(id), true, searchParams.get('token'));
    }, []);

    const entry = store.ticketStore.entry;

    return <>
        <Typography variant="h4" component="div">
            Заявка № {entry?.id}
        </Typography>
        <Typography variant="h6" component="div" sx={{mt:3}}>
            Описание заявки
        </Typography>
        <List>
            <ListItem>
                <ListItemText>
                    Название: {entry?.name}
                </ListItemText>
            </ListItem>
            <Divider />
            <ListItem>
                <ListItemText>
                    Статус: {entry?.isClosed === true ? 'Закрыта' : 'Открыта'}
                </ListItemText>
            </ListItem>
            <Divider />
            <ListItem>
                <ListItemText>
                    Создана: {entry !== null && formatDistanceToNow(new Date(entry.dateCreated), { addSuffix: true, locale: ru })}
                </ListItemText>
            </ListItem>
            <Divider />
            <ListItem>
                <ListItemText>
                    Обновлена: {entry !== null && formatDistanceToNow(new Date(entry.dateModified), { addSuffix: true, locale: ru })}
                </ListItemText>
            </ListItem>
            <Divider />
            <ListItem>
                <ListItemText>
                    Категория: {entry?.ticketCategory}
                </ListItemText>
            </ListItem>
            <Divider />
        </List>
        <Typography variant="h6" component="div" sx={{mt:3}}>
            Комментарии
        </Typography>
        <Button  sx={{mb:2}} variant="contained" component={Link} to={
            searchParams.get('token')
            ? UIRoutesHelper.ticketsCommentCreateAlt.getRoute(entry?.id ?? 0, searchParams.get('token'))
            : UIRoutesHelper.ticketsCommentCreate.getRoute(entry?.id ?? 0)
            }>Добавить комментарий</Button>
        <Grid container spacing={{ xs: 2, md: 3 }} columns={{ xs: 4, sm: 8, md: 12 }}>
            {entry?.ticketComments.map((s, index) => (
                <Grid item xs={2} sm={4} md={4} key={index}>
                    <Card variant="outlined" sx={{ p: 1 }}>
                        <List>
                            <div><small>{`${s.creatorFirstName} ${s.creatorLastName}`} {formatDistanceToNow(new Date(s.dateCreated), { addSuffix: true, locale: ru })}</small></div>
                            {s.text}
                            {
                                s.ticketCommentAttachmentModels.map((x, j) => (
                                    <Box key={j}>
                                        <Divider />
                                        <ListItem>
                                            <ListItemText>
                                                <MuiLink href={`${x.path}?token=${searchParams.get('token')}`} target="_blank">{x.originalFileName}</MuiLink>
                                            </ListItemText>
                                        </ListItem>
                                    </Box>
                                ))
                            }
                        </List>
                    </Card>
                </Grid>
            ))}
        </Grid>
    </>;
};

export default observer(TicketDetailView);
