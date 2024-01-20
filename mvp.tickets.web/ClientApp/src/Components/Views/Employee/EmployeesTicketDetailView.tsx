import { Autocomplete, Box, Button, Card, Divider, Grid, List, ListItem, ListItemText, Typography, Link as MuiLink, IconButton, TextField } from '@mui/material';
import { FC, useState, useEffect } from 'react';
import { Link, useParams } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { UIRoutesHelper } from '../../../Helpers/UIRoutesHelper';
import { useRootStore } from '../../../Store/RootStore';
import { formatDistanceToNow } from 'date-fns';
import { ru } from 'date-fns/locale';
import EditIcon from '@mui/icons-material/Edit';
import SaveIcon from '@mui/icons-material/Save';
import CloseIcon from '@mui/icons-material/Close';
import { ITicketUpdateCommandRequest, UpdatedTicketField } from '../../../Models/Ticket';

interface IEmployeesTicketDetailViewProps {
}

const EmployeesTicketDetailView: FC<IEmployeesTicketDetailViewProps> = (props) => {
    const store = useRootStore();
    const { id } = useParams();
    const [request, setRequest] = useState<ITicketUpdateCommandRequest>({
        id: Number(id),
        updatedField: UpdatedTicketField.None,
        value: 0
    });
    useEffect(() => {
        store.ticketStore.getEntry(Number(id), false);
        return () => {
            store.ticketStore.setEntry(null);
            store.categoryStore.setCategories([]);
            store.priorityStore.setEntries([]);
            store.queueStore.setEntries([]);
            store.statusStore.setEntries([]);
            store.resolutionStore.setEntries([]);
            store.userStore.setAssignees([]);
        };
    }, []);

    const handleEdit = (field: UpdatedTicketField) => {
        setRequest({ ...request, updatedField: field, value: 0 });
        switch (field) {
            case UpdatedTicketField.Assignee: {
                if (!store.userStore.assignees.length) {
                    store.userStore.getAssignees();
                }
                break;
            }
            case UpdatedTicketField.Priority: {
                if (!store.priorityStore.entries.length) {
                    store.priorityStore.getEntries(true);
                }
                break;
            }
            case UpdatedTicketField.Category: {
                if (!store.categoryStore.categories.length) {
                    store.categoryStore.getCategories(true);
                }
                break;
            }
            case UpdatedTicketField.Queue: {
                if (!store.queueStore.entries.length) {
                    store.queueStore.getEntries(true);
                }
                break;
            }
            case UpdatedTicketField.Status: {
                if (!store.statusStore.entries.length) {
                    store.statusStore.getEntries(true);
                }
                break;
            }
            case UpdatedTicketField.Resolution: {
                if (!store.resolutionStore.entries.length) {
                    store.resolutionStore.getEntries(true);
                }
                break;
            }
        }
    }

    const handleSave = () => {
        if (request.updatedField !== UpdatedTicketField.None && request.value > 0) {
            store.ticketStore.update(request);
        }
        handleCancel();
    }

    const handleCancel = () => {
        setRequest({ ...request, updatedField: UpdatedTicketField.None, value: 0 });
    }

    const entry = store.ticketStore.entry;
    const assignee = entry?.assigneeId ?? 0 > 0
        ? `${entry?.assigneeFirstName} ${entry?.assigneeLastName} (${entry?.assigneeEmail})`
        : '-';
    const priority = entry?.ticketPriority ?? '-';
    return <>
        <Typography variant="h4" component="div">
            Заявка № {entry?.id}
        </Typography>
        <Typography variant="h6" component="div" sx={{ mt: 3 }}>
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
                    {
                        request.updatedField !== UpdatedTicketField.Assignee
                            ? <>
                                Назначено на: {assignee}
                                <IconButton component={Button} onClick={() => handleEdit(UpdatedTicketField.Assignee)}>
                                    <EditIcon />
                                </IconButton>
                            </>
                            : <>
                                <Autocomplete
                                    disablePortal
                                    options={store.userStore.assignees}
                                    defaultValue={{ id: entry?.assigneeId ?? 0, name: assignee }}
                                    getOptionLabel={option => option.name}
                                    onChange={(event, value) => setRequest({ ...request, value: value?.id ?? 0 })}
                                    isOptionEqualToValue={(option, value) => option.id === value.id}
                                    renderInput={(params) => <TextField {...params} label="Назначено" />}
                                />
                                <IconButton component={Button} onClick={handleSave} disabled={request.value === 0}>
                                    <SaveIcon />
                                </IconButton>
                                <IconButton component={Button} onClick={handleCancel}>
                                    <CloseIcon />
                                </IconButton>
                            </>
                    }
                </ListItemText>
            </ListItem>
            <Divider />
            <ListItem>
                <ListItemText>
                    Имя и фамилия автора: {entry?.reporterFirstName + ' ' + entry?.reporterLastName}
                </ListItemText>
            </ListItem>
            <Divider />
            <ListItem>
                <ListItemText>
                    Почта автора: {entry?.reporterEmail}
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
                    {
                        request.updatedField !== UpdatedTicketField.Priority
                            ? <>
                                Приоритет: {priority}
                                <IconButton component={Button} onClick={() => handleEdit(UpdatedTicketField.Priority)}>
                                    <EditIcon />
                                </IconButton>
                            </>
                            : <>
                                <Autocomplete
                                    disablePortal
                                    options={store.priorityStore.entries}
                                    defaultValue={{ id: entry?.ticketPriorityId ?? 0, name: priority, level: 0, isActive: false, dateCreated: new Date(), dateModified: new Date() }}
                                    getOptionLabel={option => option.name}
                                    onChange={(event, value) => setRequest({ ...request, value: value?.id ?? 0 })}
                                    isOptionEqualToValue={(option, value) => option.id === value.id}
                                    renderInput={(params) => <TextField {...params} label="Приоритет" />}
                                />
                                <IconButton component={Button} onClick={handleSave} disabled={request.value === 0}>
                                    <SaveIcon />
                                </IconButton>
                                <IconButton component={Button} onClick={handleCancel}>
                                    <CloseIcon />
                                </IconButton>
                            </>
                    }
                </ListItemText>
            </ListItem>
            <Divider />
            <ListItem>
                <ListItemText>
                    {
                        request.updatedField !== UpdatedTicketField.Status
                            ? <>
                                Статус: {entry?.ticketStatus}
                                <IconButton component={Button} onClick={() => handleEdit(UpdatedTicketField.Status)}>
                                    <EditIcon />
                                </IconButton>
                            </>
                            : <>
                                <Autocomplete
                                    disablePortal
                                    options={store.statusStore.entries}
                                    defaultValue={{ id: entry?.ticketStatusId ?? 0, name: entry?.ticketStatus ?? '', isCompletion: false, isDefault: false, isActive: false, dateCreated: new Date(), dateModified: new Date() }}
                                    getOptionLabel={option => option.name}
                                    onChange={(event, value) => setRequest({ ...request, value: value?.id ?? 0 })}
                                    isOptionEqualToValue={(option, value) => option.id === value.id}
                                    renderInput={(params) => <TextField {...params} label="Статус" />}
                                />
                                <IconButton component={Button} onClick={handleSave} disabled={request.value === 0}>
                                    <SaveIcon />
                                </IconButton>
                                <IconButton component={Button} onClick={handleCancel}>
                                    <CloseIcon />
                                </IconButton>
                            </>
                    }
                </ListItemText>
            </ListItem>
            <Divider />
            <ListItem>
                <ListItemText>
                    {
                        request.updatedField !== UpdatedTicketField.Queue
                            ? <>
                                Очередь: {entry?.ticketQueue}
                                <IconButton component={Button} onClick={() => handleEdit(UpdatedTicketField.Queue)}>
                                    <EditIcon />
                                </IconButton>
                            </>
                            : <>
                                <Autocomplete
                                    disablePortal
                                    options={store.queueStore.entries}
                                    defaultValue={{ id: entry?.ticketQueueId ?? 0, name: entry?.ticketQueue ?? '', isDefault: false, isActive: false, dateCreated: new Date(), dateModified: new Date() }}
                                    getOptionLabel={option => option.name}
                                    onChange={(event, value) => setRequest({ ...request, value: value?.id ?? 0 })}
                                    isOptionEqualToValue={(option, value) => option.id === value.id}
                                    renderInput={(params) => <TextField {...params} label="Очередь" />}
                                />
                                <IconButton component={Button} onClick={handleSave} disabled={request.value === 0}>
                                    <SaveIcon />
                                </IconButton>
                                <IconButton component={Button} onClick={handleCancel}>
                                    <CloseIcon />
                                </IconButton>
                            </>
                    }
                </ListItemText>
            </ListItem>
            <Divider />
            <ListItem>
                <ListItemText>
                    {
                        request.updatedField !== UpdatedTicketField.Category
                            ? <>
                                Категория: {entry?.ticketCategory}
                                <IconButton component={Button} onClick={() => handleEdit(UpdatedTicketField.Category)}>
                                    <EditIcon />
                                </IconButton>
                            </>
                            : <>
                                <Autocomplete
                                    disablePortal
                                    options={store.categoryStore.categories}
                                    defaultValue={{ id: entry?.ticketCategoryId ?? 0, name: entry?.ticketCategory ?? '', isDefault: false, isActive: false, isRoot: false, parentCategoryId: 0, parentCategory: '', dateCreated: new Date(), dateModified: new Date() }}
                                    getOptionLabel={option => option.name}
                                    onChange={(event, value) => setRequest({ ...request, value: value?.id ?? 0 })}
                                    isOptionEqualToValue={(option, value) => option.id === value.id}
                                    renderInput={(params) => <TextField {...params} label="Категория" />}
                                />
                                <IconButton component={Button} onClick={handleSave} disabled={request.value === 0}>
                                    <SaveIcon />
                                </IconButton>
                                <IconButton component={Button} onClick={handleCancel}>
                                    <CloseIcon />
                                </IconButton>
                            </>
                    }
                </ListItemText>
            </ListItem>
            <Divider />
            {
                entry?.isClosed === true &&
                <>
                    <ListItem>
                        <ListItemText>
                            {
                                request.updatedField !== UpdatedTicketField.Resolution
                                    ? <>
                                        Причина закрытия: {entry?.ticketResolution}
                                        <IconButton component={Button} onClick={() => handleEdit(UpdatedTicketField.Resolution)}>
                                            <EditIcon />
                                        </IconButton>
                                    </>
                                    : <>
                                        <Autocomplete
                                            disablePortal
                                            options={store.resolutionStore.entries}
                                            defaultValue={{ id: entry?.ticketResolutionId ?? 0, name: entry?.ticketResolution ?? '', isActive: false, dateCreated: new Date(), dateModified: new Date() }}
                                            getOptionLabel={option => option.name}
                                            onChange={(event, value) => setRequest({ ...request, value: value?.id ?? 0 })}
                                            isOptionEqualToValue={(option, value) => option.id === value.id}
                                            renderInput={(params) => <TextField {...params} label="Причина закрытия" />}
                                        />
                                        <IconButton component={Button} onClick={handleSave} disabled={request.value === 0}>
                                            <SaveIcon />
                                        </IconButton>
                                        <IconButton component={Button} onClick={handleCancel}>
                                            <CloseIcon />
                                        </IconButton>
                                    </>
                            }
                        </ListItemText>
                    </ListItem>
                    <Divider />
                </>
            }
        </List>
        <Typography variant="h6" component="div" sx={{ mt: 3 }}>
            Комментарии
        </Typography>
        <Button sx={{ mb: 2 }} variant="contained" component={Link} to={UIRoutesHelper.employeeTicketCommentCreate.getRoute(entry?.id ?? 0)}>Добавить комментарий</Button>
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
                                                <MuiLink href={x.path} target="_blank">{x.originalFileName}</MuiLink>
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

export default observer(EmployeesTicketDetailView);
