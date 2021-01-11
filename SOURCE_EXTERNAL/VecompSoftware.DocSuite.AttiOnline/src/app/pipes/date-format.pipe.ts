import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'dateFormat' })
export class DateFormatPipe implements PipeTransform {
    transform(value: string): string {
        let result: string = '';
        if (!!value) {
            let item: Date = new Date(value);
            let monthNumber: number = item.getMonth() + 1;
            let day: string = String("0" + item.getDate()).slice(-2);
            let month: string = String("0" + monthNumber).slice(-2);
            result = [day, month, item.getFullYear()].join('/').toString();

        }
        return result;
    }
}