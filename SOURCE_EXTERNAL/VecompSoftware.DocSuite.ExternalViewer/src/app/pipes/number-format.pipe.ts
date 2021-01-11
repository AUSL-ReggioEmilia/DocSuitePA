import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'numberFormat'})
export class NumberFormatPipe implements PipeTransform{
    transform(value: number): string {
        let result: string = '';
        if (!!value) {
            result = String("000000" + value).slice(-7);
        }
        return result;
    }
}