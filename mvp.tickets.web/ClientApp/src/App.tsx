import { FC, useEffect } from 'react';
import { Navigate, useRoutes } from 'react-router-dom';
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';

import HomeView from './Components/Views/HomeView';
import Layout from './Components/Shared/Layout';
import LoginView from './Components/Views/LoginView';
import NotFoundView from './Components/Views/NotFoundView';
import { useRootStore } from './Store/RootStore';
import { IUserModel } from './Models/User';
import { UIRoutesHelper } from './Helpers/UIRoutesHelper';
import TicketsView from './Components/Views/TicketsView';
import CreateTicketView from './Components/Views/CreateTicketView';
import EmployeesView from './Components/Views/Employee/EmployeesView';
import ProtectedRoute from './Components/Shared/ProtectedRoute';
import { Permissions } from './Enums/Permissions';
import AdminUsersView from './Components/Views/Admin/User/AdminUsersView';
import AdminCategoriesView from './Components/Views/Admin/Category/AdminCategoriesView';
import AdminCategoriesCreateView from './Components/Views/Admin/Category/AdminCategoriesCreateView';
import AdminCategoriesUpdateView from './Components/Views/Admin/Category/AdminCategoriesUpdateView';
import AdminPrioritiesView from './Components/Views/Admin/Priority/AdminPrioritiesView';
import AdminQueuesView from './Components/Views/Admin/Queue/AdminQueuesView';
import AdminStatusRulesView from './Components/Views/Admin/AdminStatusRulesView';
import AdminStatusesView from './Components/Views/Admin/Status/AdminStatusesView';
import AdminResolutionsView from './Components/Views/Admin/Resolution/AdminResolutionsView';
import AdminResponseTemplateTypesView from './Components/Views/Admin/ResponseTemplateType/AdminResponseTemplateTypesView';
import AdminResponseTemplatesView from './Components/Views/Admin/ResponseTemplate/AdminResponseTemplatesView';
import AdminUsersCreateView from './Components/Views/Admin/User/AdminUsersCreateView';
import AdminUsersUpdateView from './Components/Views/Admin/User/AdminUsersUpdateView';
import AdminPrioritiesCreateView from './Components/Views/Admin/Priority/AdminPrioritiesCreateView';
import AdminPrioritiesUpdateView from './Components/Views/Admin/Priority/AdminPrioritiesUpdateView';
import AdminQueuesCreateView from './Components/Views/Admin/Queue/AdminQueuesCreateView';
import AdminQueuesUpdateView from './Components/Views/Admin/Queue/AdminQueuesUpdateView';
import AdminStatusesCreateView from './Components/Views/Admin/Status/AdminStatusesCreateView';
import AdminStatusesUpdateView from './Components/Views/Admin/Status/AdminStatusesUpdateView';
import AdminResolutionsCreateView from './Components/Views/Admin/Resolution/AdminResolutionsCreateView';
import AdminResolutionsUpdateView from './Components/Views/Admin/Resolution/AdminResolutionsUpdateView';
import AdminResponseTemplateTypesCreateView from './Components/Views/Admin/ResponseTemplateType/AdminResponseTemplateTypesCreateView';
import AdminResponseTemplateTypesUpdateView from './Components/Views/Admin/ResponseTemplateType/AdminResponseTemplateTypesUpdateView';
import AdminResponseTemplatesCreateView from './Components/Views/Admin/ResponseTemplate/AdminResponseTemplatesCreateView';
import AdminResponseTemplatesUpdateView from './Components/Views/Admin/ResponseTemplate/AdminResponseTemplatesUpdateView';
import EmployeesTicketDetailView from './Components/Views/Employee/EmployeesTicketDetailView';
import TicketDetailView from './Components/Views/TicketDetailView';
import TicketsCommentCreateView from './Components/Views/TicketsCommentCreateView';
import EmployeesTicketCommentCreateView from './Components/Views/Employee/EmployeesTicketCommentCreateView';

interface IAppProps {
  user: IUserModel | null
}

export const App: FC<IAppProps> = (props) => {
  const store = useRootStore();

  useEffect(() => {
    if (props.user !== null) {
      store.userStore.setCurrentUser(props.user);
    }
  }, []);

  const mainRoutes = {
    path: UIRoutesHelper.home.getRoute(),
    element: <ProtectedRoute permissions={Permissions.None} children={<Layout />} user={props.user} />,
    children: [
      { path: '*', element: <Navigate to={UIRoutesHelper.notFound.getRoute()} /> },
      { path: UIRoutesHelper.home.path, element: <HomeView /> },
      { path: UIRoutesHelper.tickets.path, element: <ProtectedRoute permissions={Permissions.User} children={<TicketsView />} user={props.user} /> },
      { path: UIRoutesHelper.ticketsCreate.path, element: <ProtectedRoute permissions={Permissions.User} children={<CreateTicketView />} user={props.user} /> },
      { path: UIRoutesHelper.ticketsDetail.path, element: <ProtectedRoute permissions={Permissions.User} children={<TicketDetailView />} user={props.user} /> },
      { path: UIRoutesHelper.ticketsCommentCreate.path, element: <ProtectedRoute permissions={Permissions.User} children={<TicketsCommentCreateView />} user={props.user} /> },

      { path: UIRoutesHelper.adminUsers.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminUsersView />} user={props.user} /> },
      { path: UIRoutesHelper.adminUsersCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminUsersCreateView />} user={props.user} /> },
      { path: UIRoutesHelper.adminUsersUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminUsersUpdateView />} user={props.user} /> },

      { path: UIRoutesHelper.adminCategories.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminCategoriesView />} user={props.user} /> },
      { path: UIRoutesHelper.adminCategoriesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminCategoriesCreateView />} user={props.user} /> },
      { path: UIRoutesHelper.adminCategoriesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminCategoriesUpdateView />} user={props.user} /> },
      
      { path: UIRoutesHelper.adminPriorities.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminPrioritiesView />} user={props.user} /> },
      { path: UIRoutesHelper.adminPrioritiesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminPrioritiesCreateView />} user={props.user} /> },
      { path: UIRoutesHelper.adminPrioritiesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminPrioritiesUpdateView />} user={props.user} /> },
      
      { path: UIRoutesHelper.adminQueues.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminQueuesView />} user={props.user} /> },
      { path: UIRoutesHelper.adminQueuesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminQueuesCreateView />} user={props.user} /> },
      { path: UIRoutesHelper.adminQueuesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminQueuesUpdateView />} user={props.user} /> },
      
      { path: UIRoutesHelper.adminStatuses.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminStatusesView />} user={props.user} /> },
      { path: UIRoutesHelper.adminStatusesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminStatusesCreateView />} user={props.user} /> },
      { path: UIRoutesHelper.adminStatusesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminStatusesUpdateView />} user={props.user} /> },
      
      { path: UIRoutesHelper.adminResolutions.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResolutionsView />} user={props.user} /> },
      { path: UIRoutesHelper.adminResolutionsCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResolutionsCreateView />} user={props.user} /> },
      { path: UIRoutesHelper.adminResolutionsUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResolutionsUpdateView />} user={props.user} /> },
      
      { path: UIRoutesHelper.adminResponseTemplateTypes.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplateTypesView />} user={props.user} /> },
      { path: UIRoutesHelper.adminResponseTemplateTypesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplateTypesCreateView />} user={props.user} /> },
      { path: UIRoutesHelper.adminResponseTemplateTypesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplateTypesUpdateView />} user={props.user} /> },
      
      { path: UIRoutesHelper.adminResponseTemplates.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplatesView />} user={props.user} /> },
      { path: UIRoutesHelper.adminResponseTemplatesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplatesCreateView />} user={props.user} /> },
      { path: UIRoutesHelper.adminResponseTemplatesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplatesUpdateView />} user={props.user} /> },
      
      { path: UIRoutesHelper.employee.path, element: <ProtectedRoute permissions={Permissions.Employee} children={<EmployeesView />} user={props.user} /> },
      { path: UIRoutesHelper.employeeTicketDetail.path, element: <ProtectedRoute permissions={Permissions.Employee} children={<EmployeesTicketDetailView />} user={props.user} /> },
      { path: UIRoutesHelper.employeeTicketCommentCreate.path, element: <ProtectedRoute permissions={Permissions.Employee} children={<EmployeesTicketCommentCreateView />} user={props.user} /> },
      
      { path: UIRoutesHelper.notFound.path, element: <NotFoundView /> },
    ],
  };
  const loginRoute = {
    path: UIRoutesHelper.login.path,
    element: <LoginView />
  };
  const ticketDetailAltRoute = {
    path: UIRoutesHelper.ticketsDetailAlt.path,
    element: <Layout />,
    children: [
      { path: '', element: <TicketDetailView /> }
    ]
  };
  const ticketsCommentCreateAltRoute = {
    path: UIRoutesHelper.ticketsCommentCreateAlt.path,
    element: <Layout />,
    children: [
      { path: '', element: <TicketsCommentCreateView /> }
    ]
  };
  const routing = useRoutes([mainRoutes, loginRoute, ticketDetailAltRoute, ticketsCommentCreateAltRoute]);

  return <>{routing}</>;
}
