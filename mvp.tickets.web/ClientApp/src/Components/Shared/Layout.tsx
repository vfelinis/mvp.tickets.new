import { FC, useState } from 'react';
import { Outlet, Link, useNavigate } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { styled, useTheme, Theme, CSSObject } from '@mui/material/styles';
import Box from '@mui/material/Box';
import MuiDrawer from '@mui/material/Drawer';
import MuiAppBar, { AppBarProps as MuiAppBarProps } from '@mui/material/AppBar';
import Toolbar from '@mui/material/Toolbar';
import List from '@mui/material/List';
import CssBaseline from '@mui/material/CssBaseline';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemIcon from '@mui/material/ListItemIcon';
import ListItemText from '@mui/material/ListItemText';
import HomeIcon from '@mui/icons-material/Home';
import LogoutIcon from '@mui/icons-material/Logout';
import AdminPanelSettingsIcon from '@mui/icons-material/AdminPanelSettings';
import BadgeIcon from '@mui/icons-material/Badge';
import ViewListIcon from '@mui/icons-material/ViewList';
import PlaylistAddIcon from '@mui/icons-material/PlaylistAdd';
import Error from './Error';
import { useRootStore } from '../../Store/RootStore';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';
import { hasPermission, Permissions } from '../../Enums/Permissions';
import { CircularProgress, Menu, MenuItem } from '@mui/material';

const drawerWidth = 240;

const openedMixin = (theme: Theme): CSSObject => ({
    width: drawerWidth,
    transition: theme.transitions.create('width', {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.enteringScreen,
    }),
    overflowX: 'hidden',
});

const closedMixin = (theme: Theme): CSSObject => ({
    transition: theme.transitions.create('width', {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen,
    }),
    overflowX: 'hidden',
    width: `calc(${theme.spacing(7)} + 1px)`,
    [theme.breakpoints.up('sm')]: {
        width: `calc(${theme.spacing(8)} + 1px)`,
    },
});

const DrawerHeader = styled('div')(({ theme }) => ({
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'flex-end',
    padding: theme.spacing(0, 1),
    // necessary for content to be below app bar
    ...theme.mixins.toolbar,
}));

interface AppBarProps extends MuiAppBarProps {
    open?: boolean;
}

const AppBar = styled(MuiAppBar, {
    shouldForwardProp: (prop) => prop !== 'open',
})<AppBarProps>(({ theme, open }) => ({
    zIndex: theme.zIndex.drawer + 1,
    transition: theme.transitions.create(['width', 'margin'], {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen,
    }),
    ...(open && {
        marginLeft: drawerWidth,
        width: `calc(100% - ${drawerWidth}px)`,
        transition: theme.transitions.create(['width', 'margin'], {
            easing: theme.transitions.easing.sharp,
            duration: theme.transitions.duration.enteringScreen,
        }),
    }),
}));

const Drawer = styled(MuiDrawer, { shouldForwardProp: (prop) => prop !== 'open' })(
    ({ theme, open }) => ({
        width: drawerWidth,
        flexShrink: 0,
        whiteSpace: 'nowrap',
        boxSizing: 'border-box',
        ...(open && {
            ...openedMixin(theme),
            '& .MuiDrawer-paper': openedMixin(theme),
        }),
        ...(!open && {
            ...closedMixin(theme),
            '& .MuiDrawer-paper': closedMixin(theme),
        }),
    }),
);

interface ILayoutProps {
}

const Layout: FC<ILayoutProps> = (props) => {
    const store = useRootStore();
    const theme = useTheme();
    const [open, setOpen] = useState(false);
    const navigate = useNavigate();

    const handleDrawerOpen = () => {
        setOpen(true);
    };

    const handleDrawerClose = () => {
        setOpen(false);
    };

    const [anchorAdminEl, setAnchorAdminEl] = useState<null | HTMLElement>(null);
    const openAdmin = Boolean(anchorAdminEl);
    const handleAdminClick = (event: React.MouseEvent<HTMLLIElement>) => {
        setAnchorAdminEl(event.currentTarget);
    };
    const handleAdminClose = () => {
        setAnchorAdminEl(null);
    };

    const isLoading = store.categoryStore.isLoading
        || store.userStore.isLoading
        || store.priorityStore.isLoading
        || store.queueStore.isLoading
        || store.resolutionStore.isLoading
        || store.responseTemplateStore.isLoading
        || store.responseTemplateTypeStore.isLoading
        || store.statusStore.isLoading
        || store.ticketStore.isLoading;

    return <>
        {isLoading && <Box sx={{
            position: 'absolute',
            display: 'flex',
            width: '100%',
            height: '100vh',
            alignItems: 'center',
            justifyContent: 'center',
            backgroundColor: 'black',
            zIndex: '10000',
            opacity: 0.6,
        }}><CircularProgress /></Box>}
        <Box sx={{ display: 'flex' }}>
            <CssBaseline />
            <AppBar position="fixed" open={open}>
                <Toolbar>
                    <IconButton
                        color="inherit"
                        aria-label="open drawer"
                        onClick={handleDrawerOpen}
                        edge="start"
                        sx={{
                            marginRight: 5,
                            ...(open && { display: 'none' }),
                        }}
                    >
                        <MenuIcon />
                    </IconButton>
                    <Typography variant="h6" noWrap component="div">
                        MVP Tickets
                    </Typography>
                </Toolbar>
            </AppBar>
            <Drawer variant="permanent" open={open}>
                <DrawerHeader>
                    <IconButton onClick={handleDrawerClose}>
                        {theme.direction === 'rtl' ? <ChevronRightIcon /> : <ChevronLeftIcon />}
                    </IconButton>
                </DrawerHeader>
                <Divider />
                <List>
                    <ListItem key="Главная" disablePadding sx={{ display: 'block' }} onClick={() => navigate(UIRoutesHelper.home.getRoute(), { replace: true })}>
                        <ListItemButton
                            sx={{
                                minHeight: 48,
                                justifyContent: open ? 'initial' : 'center',
                                px: 2.5,
                            }}
                        >
                            <ListItemIcon
                                sx={{
                                    minWidth: 0,
                                    mr: open ? 3 : 'auto',
                                    justifyContent: 'center',
                                }}
                            >
                                <HomeIcon />
                            </ListItemIcon>
                            <ListItemText primary="Главная" sx={{ opacity: open ? 1 : 0 }} />
                        </ListItemButton>
                    </ListItem>
                    {
                        store.userStore.currentUser != null && hasPermission(store.userStore.currentUser.permissions, Permissions.User) &&
                        <>
                            <ListItem key="Мои заявки" disablePadding sx={{ display: 'block' }} onClick={() => navigate(UIRoutesHelper.tickets.getRoute(), { replace: true })}>
                                <ListItemButton
                                    sx={{
                                        minHeight: 48,
                                        justifyContent: open ? 'initial' : 'center',
                                        px: 2.5,
                                    }}
                                >
                                    <ListItemIcon
                                        sx={{
                                            minWidth: 0,
                                            mr: open ? 3 : 'auto',
                                            justifyContent: 'center',
                                        }}
                                    >
                                        <ViewListIcon />
                                    </ListItemIcon>
                                    <ListItemText primary="Мои заявки" sx={{ opacity: open ? 1 : 0 }} />
                                </ListItemButton>
                            </ListItem>
                            <ListItem key="Создавать заявку" disablePadding sx={{ display: 'block' }} onClick={() => navigate(UIRoutesHelper.ticketsCreate.getRoute(), { replace: true })}>
                                <ListItemButton
                                    sx={{
                                        minHeight: 48,
                                        justifyContent: open ? 'initial' : 'center',
                                        px: 2.5,
                                    }}
                                >
                                    <ListItemIcon
                                        sx={{
                                            minWidth: 0,
                                            mr: open ? 3 : 'auto',
                                            justifyContent: 'center',
                                        }}
                                    >
                                        <PlaylistAddIcon />
                                    </ListItemIcon>
                                    <ListItemText primary="Создать заявку" sx={{ opacity: open ? 1 : 0 }} />
                                </ListItemButton>
                            </ListItem>
                        </>
                    }
                </List>
                {
                    store.userStore.currentUser != null && hasPermission(store.userStore.currentUser.permissions, Permissions.Admin) &&
                    <>
                        <Divider />
                        <List>
                            <ListItem
                                key="Администратор"
                                disablePadding
                                sx={{ display: 'block' }}
                                id="admin-button"
                                aria-controls={open ? 'admin-menu' : undefined}
                                aria-haspopup="true"
                                aria-expanded={openAdmin ? 'true' : undefined}
                                onClick={handleAdminClick}
                            >
                                <ListItemButton
                                    sx={{
                                        minHeight: 48,
                                        justifyContent: open ? 'initial' : 'center',
                                        px: 2.5,
                                    }}
                                >
                                    <ListItemIcon
                                        sx={{
                                            minWidth: 0,
                                            mr: open ? 3 : 'auto',
                                            justifyContent: 'center',
                                        }}
                                    >
                                        <AdminPanelSettingsIcon />
                                    </ListItemIcon>
                                    <ListItemText primary="Администратор" sx={{ opacity: open ? 1 : 0 }} />
                                </ListItemButton>
                            </ListItem>
                            <Menu
                                id="admin-menu"
                                anchorEl={anchorAdminEl}
                                open={openAdmin}
                                onClose={handleAdminClose}
                                MenuListProps={{
                                    'aria-labelledby': 'admin-button',
                                }}
                            >
                                <MenuItem onClick={handleAdminClose} component={Link} to={UIRoutesHelper.adminUsers.getRoute()}>
                                    Пользователи
                                </MenuItem>
                                <MenuItem onClick={handleAdminClose} component={Link} to={UIRoutesHelper.adminCategories.getRoute()}>
                                    Категории
                                </MenuItem>
                                <MenuItem onClick={handleAdminClose} component={Link} to={UIRoutesHelper.adminPriorities.getRoute()}>
                                    Приоритеты
                                </MenuItem>
                                <MenuItem onClick={handleAdminClose} component={Link} to={UIRoutesHelper.adminQueues.getRoute()}>
                                    Очереди
                                </MenuItem>
                                {/* <MenuItem onClick={handleAdminClose} component={Link} to={UIRoutesHelper.adminStatusRules.getRoute()}>
                                    Правила статусов
                                </MenuItem> */}
                                <MenuItem onClick={handleAdminClose} component={Link} to={UIRoutesHelper.adminStatuses.getRoute()}>
                                    Статусы
                                </MenuItem>
                                <MenuItem onClick={handleAdminClose} component={Link} to={UIRoutesHelper.adminResolutions.getRoute()}>
                                    Резолюции
                                </MenuItem>
                                <MenuItem onClick={handleAdminClose} component={Link} to={UIRoutesHelper.adminResponseTemplateTypes.getRoute()}>
                                    Типы шаблонов
                                </MenuItem>
                                <MenuItem onClick={handleAdminClose} component={Link} to={UIRoutesHelper.adminResponseTemplates.getRoute()}>
                                    Шаблоны
                                </MenuItem>
                            </Menu>
                        </List>
                    </>
                }
                {
                    store.userStore.currentUser != null && hasPermission(store.userStore.currentUser.permissions, Permissions.Employee) &&
                    <>
                        <Divider />
                        <List>
                            <ListItem key="Сотрудник" disablePadding sx={{ display: 'block' }} onClick={() => navigate(UIRoutesHelper.employee.getRoute(), { replace: true })}>
                                <ListItemButton
                                    sx={{
                                        minHeight: 48,
                                        justifyContent: open ? 'initial' : 'center',
                                        px: 2.5,
                                    }}
                                >
                                    <ListItemIcon
                                        sx={{
                                            minWidth: 0,
                                            mr: open ? 3 : 'auto',
                                            justifyContent: 'center',
                                        }}
                                    >
                                        <BadgeIcon />
                                    </ListItemIcon>
                                    <ListItemText primary="Сотрудник" sx={{ opacity: open ? 1 : 0 }} />
                                </ListItemButton>
                            </ListItem>
                        </List>
                    </>
                }
                <Divider />
                {store.userStore.currentUser != null &&
                    <List>
                        <ListItem key="Выйти" disablePadding sx={{ display: 'block' }} onClick={() => store.userStore.logout()}>
                            <ListItemButton
                                sx={{
                                    minHeight: 48,
                                    justifyContent: open ? 'initial' : 'center',
                                    px: 2.5,
                                }}
                            >
                                <ListItemIcon
                                    sx={{
                                        minWidth: 0,
                                        mr: open ? 3 : 'auto',
                                        justifyContent: 'center',
                                    }}
                                >
                                    <LogoutIcon />
                                </ListItemIcon>
                                <ListItemText primary="Выйти" sx={{ opacity: open ? 1 : 0 }} />
                            </ListItemButton>
                        </ListItem>
                    </List>
                }
                
            </Drawer>
            <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
                <DrawerHeader />
                <Error />
                <Outlet />
            </Box>
        </Box>
    </>;
};

export default observer(Layout);
