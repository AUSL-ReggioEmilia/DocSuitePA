import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'dateTimeFormat'})
export class DateTimeFormatPipe implements PipeTransform{
    transform(value: string): string {
        let result: string = '';
        if (!!value) {
            let item: Date = new Date(value);
            let monthNumber: number = item.getMonth() + 1;
            let day: string = String("0" + item.getDate()).slice(-2);
            let month: string = String("0" + monthNumber).slice(-2);

            let hour: string = String("0" + item.getHours().toString()).slice(-2);
            let minute: string = String("0" + item.getMinutes().toString()).slice(-2);
            let second: string = String("0" + item.getSeconds().toString()).slice(-2);

            let time: string = [hour, minute, second].join(':').toString();
            let formatDate: string = [day, month, item.getFullYear()].join('/').toString()
            result = formatDate.concat(' ', time);

        }
        return result;
    }
}