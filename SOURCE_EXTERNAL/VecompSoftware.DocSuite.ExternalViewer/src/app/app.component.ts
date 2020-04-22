import { Component, OnInit, ErrorHandler } from '@angular/core'; 
import { Router, Event, NavigationStart, NavigationEnd } from '@angular/router';

import { GlobalSetting } from './settings/global.setting';
import { AppConfigService } from './services/commons/app-config.service';
import { LoadingSpinnerService } from './services/commons/loading-spinner.service';
import { ERROR_PAGE_NAME } from './settings/global.setting';
import { Title } from '@angular/platform-browser';


@Component({
    moduleId: module.id,
    selector: 'my-app',
    templateUrl: 'app.component.html'
})
export class AppComponent implements OnInit{

    title: string = '';

    constructor(private spinnerService: LoadingSpinnerService, private router: Router, private globalSetting: GlobalSetting, titleService: Title) {
        if (AppConfigService.settings.title) {
            titleService.setTitle(AppConfigService.settings.title);
        }        
        router.events.subscribe((event: Event) => {
            if (event instanceof NavigationStart && !this.router.url.includes(ERROR_PAGE_NAME)) {
                this.spinnerService.start();
                this.router
            }
        })
    }

    ngOnInit() {
        //TODO: verificare che esista la console e il localstorage
        try {

            if (AppConfigService.settings.applicationName) {
                this.title = AppConfigService.settings.applicationName;
            }
            //TODO: validare il globalsetting controllando che esista ogni proprietà
            let address: string = this.globalSetting.apiOdataAddress;

            if (!address || address.length == 0) {
               
            }
            
        }
        catch (ex) {
            //TODO:da verificare se serve
            console.log(ex);
        }
    }   
}