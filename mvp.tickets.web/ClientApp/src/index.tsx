import axios from 'axios';
import { createRoot } from 'react-dom/client';
import { unstable_HistoryRouter as HistoryRouter } from "react-router-dom";
import { createBrowserHistory } from "history";
import { App } from './App'
import { ApiRoutesHelper } from './Helpers/ApiRoutesHelper';
import { IBaseQueryResponse } from './Models/Base';
import { IUserModel } from './Models/User';
import { ICompanyModel } from './Models/Company';

export const browserHistory = createBrowserHistory({ window });

const init = (company: ICompanyModel | null, user: IUserModel | null) => {
  const element = (
    company !== null
      ? <HistoryRouter history={browserHistory}>
        <App company={company} user={user} />
      </HistoryRouter>
      : <>Неизвестное предприятие.</>
  );

  const container = document.getElementById('root');
  const root = createRoot(container!);
  root.render(element);
};

axios.get<IBaseQueryResponse<ICompanyModel>>(ApiRoutesHelper.company.current)
  .then(response => {
    if (response.data.isSuccess) {
      axios.get<IBaseQueryResponse<IUserModel>>(ApiRoutesHelper.user.current)
        .then(userResponse => {
          if (userResponse.data.isSuccess) {
            init(response.data.data, userResponse.data.data);
          } else {
            init(response.data.data, null);
          }
        })
        .catch(userError => {
          init(response.data.data, null);
        });
    } else {
      init(null, null);
    }
  })
  .catch(error => {
    init(null, null);
  });
