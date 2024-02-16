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

let company: ICompanyModel | null = null;
let user: IUserModel | null = null;

let companyRequest = axios.get<IBaseQueryResponse<ICompanyModel>>(ApiRoutesHelper.company.current)
  .then(response => {
    if (response.data.isSuccess) {
      company = response.data.data;
    } else {
      company = null;
    }
  })
  .catch(err => {
    console.log(err);
    company = null;
  });

let userRequest = axios.get<IBaseQueryResponse<IUserModel>>(ApiRoutesHelper.user.current)
  .then(userResponse => {
    if (userResponse.data.isSuccess) {
      user = userResponse.data.data;
    } else {
      user = null;
    }
  })
  .catch(err => {
    console.log(err);
    user = null;
  });

Promise.all([companyRequest, userRequest]).then(function(values){
  init(company, user);
}).catch(function(err){
  console.log(err);
  init(company, user);
});