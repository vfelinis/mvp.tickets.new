import { FC, useLayoutEffect, useRef } from 'react';
import firebase from 'firebase/compat/app';
import 'firebase/compat/auth';
import { initializeApp } from 'firebase/app';
import { getAuth, EmailAuthProvider } from 'firebase/auth';
import * as firebaseui from 'firebaseui'
import 'firebaseui/dist/firebaseui.css'
import useScript from 'react-use-scripts';
import { Navigate } from 'react-router-dom';
import { observer } from 'mobx-react-lite';

import Error from '../Shared/Error';
import { useRootStore } from '../../Store/RootStore';
import { UIRoutesHelper } from '../../Helpers/UIRoutesHelper';

const firebaseConfig = {
  apiKey: 'AIzaSyD2gQY5dzF3wmgJO4EIEV7yHLhIr9zUl3o',
  authDomain: 'mvp-stack.firebaseapp.com',
  projectId: 'mvp-stack',
  storageBucket: 'mvp-stack.appspot.com',
  messagingSenderId: '1055185177353',
  appId: '1:1055185177353:web:29563c35fb436dede361d8'
};
const app = initializeApp(firebaseConfig);

interface ILoginViewProps {
}

const LoginView: FC<ILoginViewProps> = (props) => {
  const store = useRootStore();
  const firebaseuiElement = useRef<HTMLDivElement | null>(null);

  const { ready } = useScript({
    src: 'https://www.gstatic.com/firebasejs/ui/6.0.1/firebase-ui-auth__ru.js',
  });

  const firebaseUiConfig: firebaseui.auth.Config = {
    callbacks: {
      signInSuccessWithAuthResult: (authResult) => {
        authResult.user.getIdToken()
          .then((token: string) => {
            store.userStore.login(token);
          })
          .catch((error: any) => {
            store.errorStore.setError(JSON.stringify(error));
          });
        return false;
      },
    },
    signInFlow: 'popup',
    signInOptions: [
      EmailAuthProvider.PROVIDER_ID,
    ],
  };

  useLayoutEffect(() => {
    if (ready && firebaseuiElement.current) {
      (window as any).firebase = firebase;
      const auth = getAuth(app);
      const firebaseuiUMD: typeof firebaseui = (window as any).firebaseui;
      const ui = new firebaseuiUMD.auth.AuthUI(auth);
      ui.start(firebaseuiElement.current, firebaseUiConfig);
    }
  }, [ready]);

  return <>
    {store.userStore.currentUser !== null ? <Navigate to={UIRoutesHelper.home.getRoute()} replace={true} /> : null}
    <Error />
    <div ref={firebaseuiElement} />
  </>;
};

export default observer(LoginView);
