import { observable, action, makeObservable } from 'mobx';
import { RootStore } from './RootStore';

export class ErrorStore {
    private rootStore: RootStore;
    errors: string[];

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
        this.errors = [];
        makeObservable(this, {
            errors: observable,
            setError: action,
            clearErrors: action,
        });
    }

    setError(error: string): void {
        this.errors = [...this.errors, error];
    }

    clearErrors(index?: number): void {
        this.errors = index === undefined ? [] : this.errors.filter((s, i) => i !== index);
    }
}