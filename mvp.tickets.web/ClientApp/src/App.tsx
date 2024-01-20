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
import InvitesView from './Components/Views/Root/Invite/InvitesView';
import CompaniesView from './Components/Views/Root/Company/CompaniesView';
import MetricsView from './Components/Views/Root/Metric/MetricsView';
import InvitesCreateView from './Components/Views/Root/Invite/InvitesCreateView';
import CompaniesCreateView from './Components/Views/CompaniesCreateView';
import AdminCompanyUpdateView from './Components/Views/Admin/Company/AdminCompanyUpdateView';
import RegisterRequestView from './Components/Views/RegisterRequestView';
import EmailConfirmationView from './Components/Views/EmailConfirmationView';
import ForgotPasswordView from './Components/Views/ForgotPasswordView';
import ResetPasswordView from './Components/Views/ResetPasswordView';
import RegisterView from './Components/Views/RegisterView';
import { ICompanyModel } from './Models/Company';

interface IAppProps {
  company: ICompanyModel,
  user: IUserModel | null,
}

export const App: FC<IAppProps> = (props) => {
  const store = useRootStore();

  useEffect(() => {
    document.title = props.company.name;
    store.companyStore.setCurrent(props.company);
    if (props.user !== null) {
      store.userStore.setCurrentUser(props.user);
    }
  }, []);

  const mainRoutes = {
    path: UIRoutesHelper.home.getRoute(),
    element: <Layout />,
    children: [
      { path: '*', element: <Navigate to={UIRoutesHelper.notFound.getRoute()} /> },
      { path: UIRoutesHelper.home.path, element: <ProtectedRoute permissions={Permissions.None} children={<HomeView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.tickets.path, element: <ProtectedRoute permissions={Permissions.User} children={<TicketsView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.ticketsCreate.path, element: <ProtectedRoute permissions={Permissions.User} children={<CreateTicketView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.ticketsDetail.path, element: <ProtectedRoute permissions={Permissions.User} children={<TicketDetailView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.ticketsCommentCreate.path, element: <ProtectedRoute permissions={Permissions.User} children={<TicketsCommentCreateView />} company={props.company} user={props.user} /> },

      { path: UIRoutesHelper.adminUsers.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminUsersView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminUsersCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminUsersCreateView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminUsersUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminUsersUpdateView />} company={props.company} user={props.user} /> },

      { path: UIRoutesHelper.adminCategories.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminCategoriesView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminCategoriesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminCategoriesCreateView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminCategoriesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminCategoriesUpdateView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.adminPriorities.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminPrioritiesView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminPrioritiesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminPrioritiesCreateView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminPrioritiesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminPrioritiesUpdateView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.adminQueues.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminQueuesView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminQueuesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminQueuesCreateView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminQueuesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminQueuesUpdateView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.adminStatuses.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminStatusesView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminStatusesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminStatusesCreateView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminStatusesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminStatusesUpdateView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.adminResolutions.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResolutionsView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminResolutionsCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResolutionsCreateView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminResolutionsUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResolutionsUpdateView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.adminResponseTemplateTypes.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplateTypesView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminResponseTemplateTypesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplateTypesCreateView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminResponseTemplateTypesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplateTypesUpdateView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.adminResponseTemplates.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplatesView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminResponseTemplatesCreate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplatesCreateView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.adminResponseTemplatesUpdate.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminResponseTemplatesUpdateView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.adminCompany.path, element: <ProtectedRoute permissions={Permissions.Admin} children={<AdminCompanyUpdateView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.employee.path, element: <ProtectedRoute permissions={Permissions.Employee} children={<EmployeesView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.employeeTicketDetail.path, element: <ProtectedRoute permissions={Permissions.Employee} children={<EmployeesTicketDetailView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.employeeTicketCommentCreate.path, element: <ProtectedRoute permissions={Permissions.Employee} children={<EmployeesTicketCommentCreateView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.invites.path, element: <ProtectedRoute onlyRoot={true} permissions={Permissions.Admin} children={<InvitesView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.invitesCreate.path, element: <ProtectedRoute onlyRoot={true} permissions={Permissions.Admin} children={<InvitesCreateView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.companies.path, element: <ProtectedRoute onlyRoot={true} permissions={Permissions.Admin} children={<CompaniesView />} company={props.company} user={props.user} /> },
      { path: UIRoutesHelper.metrics.path, element: <ProtectedRoute onlyRoot={true} permissions={Permissions.Admin} children={<MetricsView />} company={props.company} user={props.user} /> },

      { path: UIRoutesHelper.notFound.path, element: <ProtectedRoute permissions={Permissions.None} children={<NotFoundView />} company={props.company} user={props.user} /> },
      
      { path: UIRoutesHelper.login.path, element: <LoginView /> },
      { path: UIRoutesHelper.ticketsDetailAlt.path, element: <TicketDetailView /> },
      { path: UIRoutesHelper.ticketsCommentCreateAlt.path, element: <TicketsCommentCreateView /> },
      { path: UIRoutesHelper.companiesCreate.path, element: <CompaniesCreateView /> },
      { path: UIRoutesHelper.registerRequest.path, element: <RegisterRequestView /> },
      { path: UIRoutesHelper.register.path, element: <RegisterView /> },
      { path: UIRoutesHelper.emailConfirmation.path, element: <EmailConfirmationView /> },
      { path: UIRoutesHelper.forgotPassword.path, element: <ForgotPasswordView /> },
      { path: UIRoutesHelper.resetPassword.path, element: <ResetPasswordView /> },
    ],
  };
  
  const routing = useRoutes([mainRoutes]);

  return <>{routing}</>;
}
