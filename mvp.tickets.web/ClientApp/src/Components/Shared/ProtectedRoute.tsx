import { FC } from 'react';
import { Navigate } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';
import { hasPermission, Permissions } from '../../Enums/Permissions';
import { useRootStore } from '../../Store/RootStore';
import { IUserModel } from '../../Models/User';

interface IProtectedRouteProps {
    permissions: Permissions,
    children: JSX.Element,
    user: IUserModel | null
}

const ProtectedRoute: FC<IProtectedRouteProps> = (props) => {
    const store = useRootStore();
    const user = store.userStore.wasInit === false ? (props.user ?? store.userStore.currentUser) : store.userStore.currentUser;
    if (user === null) {
        return <Navigate to={UIRoutesHelper.login.getRoute()} replace={true} />;
    }
    else if (!hasPermission(user.permissions, props.permissions)) {
        return <Navigate to={UIRoutesHelper.home.getRoute()} replace={true} />;
    }
    return props.children;
};

export default observer(ProtectedRoute);
