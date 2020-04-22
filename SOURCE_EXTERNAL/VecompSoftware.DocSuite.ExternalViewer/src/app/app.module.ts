import { LOCALE_ID, NgModule, ErrorHandler, APP_INITIALIZER } from '@angular/core';
import { BrowserModule, Title } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ToastrModule } from 'ngx-toastr'; 
import { LayoutModule } from '@progress/kendo-angular-layout';
import { DropDownsModule } from '@progress/kendo-angular-dropdowns';
import { GridModule } from '@progress/kendo-angular-grid';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ButtonModule } from '@progress/kendo-angular-buttons';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { PanelBarModule } from '@progress/kendo-angular-layout';
import { IntlModule } from '@progress/kendo-angular-intl';

import { AppComponent } from './app.component';
import { NumberFormatPipe } from './pipes/number-format.pipe';
import { DateFormatPipe } from './pipes/date-format.pipe';
import { DateTimeFormatPipe } from './pipes/date-time-format.pipe';

import { mappers, services, components, helpers } from './globals';
import { AppConfigService } from './services/commons/app-config.service';
import { AppRoutingModule } from './app-routing.module';

import '@progress/kendo-angular-intl/locales/it/all';
import { CacheInterceptor } from './interceptors/cache.interceptor';;
import { DialogsModule } from '@progress/kendo-angular-dialog';
import { UploadModule } from '@progress/kendo-angular-upload';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { TooltipModule } from '@progress/kendo-angular-tooltip';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { TreeViewModule } from '@progress/kendo-angular-treeview';
import { AngularFontAwesomeModule } from 'angular-font-awesome';

export function initializeConfig(appConfig: AppConfigService) {
    return () => appConfig.load();
}

@NgModule({
    imports: [BrowserModule, BrowserAnimationsModule, HttpClientModule, AppRoutingModule, LayoutModule,
              DropDownsModule, GridModule, ToastrModule.forRoot(), ButtonModule, DatePickerModule, PanelBarModule,
        IntlModule, FormsModule, DialogsModule, UploadModule, InputsModule, TooltipModule, PdfViewerModule, TreeViewModule,
        AngularFontAwesomeModule],    
    declarations: [components, NumberFormatPipe, DateFormatPipe, DateTimeFormatPipe],     
    providers: [
        mappers,
        services,
        helpers,
        Title,
        { provide: LocationStrategy, useClass: HashLocationStrategy },
        { provide: LOCALE_ID, useValue: 'it-IT' },
        { provide: APP_INITIALIZER, useFactory: initializeConfig, deps: [AppConfigService], multi: true },
        { provide: HTTP_INTERCEPTORS, useClass: CacheInterceptor, multi: true}
    ],
    bootstrap: [AppComponent]
})


export class AppModule {}