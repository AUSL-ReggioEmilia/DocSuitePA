import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
  selector: '[appNavDropdown]'
})
export class NavDropdownDirective {
    private stateClassRef = {
        open: "fa fa-chevron-down nested-icon toggle-up",
        close: "fa fa-chevron-down nested-icon toggle-down"
    }
    private state: string = "close";

    constructor(private el: ElementRef) { }

    toggle() {
        this.el.nativeElement.classList.toggle('open');

        this.state = this.state === "open" ? "close" : "open";
        (<HTMLElement>document.querySelector('.nested-icon')).className = (<any>this.stateClassRef)[this.state];
    }
}

@Directive({
    selector: '[appNavDropdownToggle]'
})
export class NavDropdownToggleDirective {

    constructor(private dropdown: NavDropdownDirective) { }

    @HostListener('click', ['$event'])
    toggle($event: any) {
        $event.preventDefault();
        this.dropdown.toggle();
    }
}
