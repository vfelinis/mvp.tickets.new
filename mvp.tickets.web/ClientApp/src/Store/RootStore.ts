import { createContext, useContext } from 'react';
import { CategoryStore } from './CategoryStore';
import { InfoStore } from './InfoStore';
import { PriorityStore } from './PriorityStore';
import { QueueStore } from './QueueStore';
import { ResolutionStore } from './ResolutionStore';
import { ResponseTemplateStore } from './ResponseTemplateStore';
import { ResponseTemplateTypeStore } from './ResponseTemplateTypeStore';
import { StatusStore } from './StatusStore';
import { TicketStore } from './TicketStore';
import { UserStore } from './UserStore';
import { InviteStore } from './InviteStore';
import { CompanyStore } from './CompanyStore';
import { ICompanyModel } from '../Models/Company';

export class RootStore {
  userStore: UserStore;
  infoStore: InfoStore;
  categoryStore: CategoryStore;
  priorityStore: PriorityStore;
  queueStore: QueueStore;
  resolutionStore: ResolutionStore;
  responseTemplateTypeStore: ResponseTemplateTypeStore;
  responseTemplateStore: ResponseTemplateStore;
  statusStore: StatusStore;
  ticketStore: TicketStore;
  inviteStore: InviteStore;
  companyStore: CompanyStore;

  constructor() {
    this.userStore = new UserStore(this);
    this.infoStore = new InfoStore(this);
    this.categoryStore = new CategoryStore(this);
    this.priorityStore = new PriorityStore(this);
    this.queueStore = new QueueStore(this);
    this.resolutionStore = new ResolutionStore(this);
    this.responseTemplateTypeStore = new ResponseTemplateTypeStore(this);
    this.responseTemplateStore = new ResponseTemplateStore(this);
    this.statusStore = new StatusStore(this);
    this.ticketStore = new TicketStore(this);
    this.inviteStore = new InviteStore(this);
    this.companyStore = new CompanyStore(this);
  }
}

const RootStoreContext = createContext(new RootStore());

export function useRootStore() {
  return useContext(RootStoreContext)
}
