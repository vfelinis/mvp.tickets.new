import { observable, action, makeObservable } from 'mobx';
import { RootStore } from './RootStore';

export class InfoStore {
    private rootStore: RootStore;
    errors: string[];
    message: string | null;

    constructor(rootStore: RootStore) {
        this.rootStore = rootStore;
        this.errors = [];
        this.message = null;
        makeObservable(this, {
            errors: observable,
            message: observable,
            setMessage: action,
            setError: action,
            clearErrors: action,
        });
    }

    setMessage(message: string | null): void {
        this.message = message;
    }

    setError(error: string): void {
        this.errors = [...this.errors.filter(s => s !== error), error];
    }

    clearErrors(index?: number): void {
        this.errors = index === undefined ? [] : this.errors.filter((s, i) => i !== index);
    }
}