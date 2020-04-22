import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({ name: 'safeHtml' })
export class Safe implements PipeTransform {
    constructor(private sanitizer: DomSanitizer) { }

    transform(style: string, args?: any) {
        return this.sanitizer.bypassSecurityTrustHtml(style);
    }
}