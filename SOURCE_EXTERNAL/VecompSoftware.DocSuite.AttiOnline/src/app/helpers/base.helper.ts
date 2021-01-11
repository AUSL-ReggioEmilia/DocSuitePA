import { Injectable } from '@angular/core';

@Injectable()
export class BaseHelper {

    dateConverter(date: Date): string {
        let result: string = '';
        if (!!date) {
            let month: string = String("0" + (date.getMonth() + 1)).slice(-2);
            let day: string = String("0" + date.getDate()).slice(-2);
            let year: string = date.getFullYear().toString();
            result = year.concat(month, day, "000000");
        }
        return result;
    }
}