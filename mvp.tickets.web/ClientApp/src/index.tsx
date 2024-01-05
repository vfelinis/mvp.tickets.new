import axios from 'axios';
import { createRoot } from 'react-dom/client';
import { unstable_HistoryRouter as HistoryRouter } from "react-router-dom";
import { createBrowserHistory } from "history";
import { App } from './App'
import { ApiRoutesHelper } from './Helpers/ApiRoutesHelper';
import { IBaseQueryResponse } from './Models/Base';
import { IUserModel } from './Models/User';

export const browserHistory = createBrowserHistory({ window });

const init = (user: IUserModel | null) => {
  const element = (
    <HistoryRouter history={browserHistory}>
      <App user={user} />
    </HistoryRouter>
  );

  const container = document.getElementById('root');
  const root = createRoot(container!);
  root.render(element);
};
let user: IUserModel;
axios.post<IBaseQueryResponse<IUserModel>>(ApiRoutesHelper.user.current)
  .then(response => {
    if (response.data.isSuccess) {
      init(response.data.data);
    } else {
      init(null);
    }
  })
  .catch(error => {
    init(null);
  });
