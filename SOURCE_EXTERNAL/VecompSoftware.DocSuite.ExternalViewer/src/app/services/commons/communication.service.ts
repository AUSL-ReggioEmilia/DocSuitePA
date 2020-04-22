import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class CommunicationService {
    constructor() { }

    private emitSubject = new Subject<any>();

    changeEmitted$ = this.emitSubject.asObservable();

    emitChange(data: any) {
        this.emitSubject.next(data);
    }
}