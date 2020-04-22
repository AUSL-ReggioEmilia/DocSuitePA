import { Directive, HostListener } from '@angular/core';

@Directive({
    selector: '[appSidebarToggler]'
})
export class SidebarToggleDirective {
    constructor() { }

    @HostListener('click', ['$event'])
    toggleOpen($event: any) {
        $event.preventDefault();
        (<HTMLElement>document.querySelector('body')).classList.toggle('sidebar-hidden');
    }
}

@Directive({
    selector: '[appMobileSidebarToggler]'
})
export class MobileSidebarToggleDirective {
    constructor() { }

    // Check if element has class
    private hasClass(target: any, elementClassName: string) {
        return new RegExp('(\\s|^)' + elementClassName + '(\\s|$)').test(target.className);
    }

    @HostListener('click', ['$event'])
    toggleOpen($event: any) {
        $event.preventDefault();
        (<HTMLElement>document.querySelector('body')).classList.toggle('sidebar-mobile-show');
    }
}
