import { LOCALE_ID, NgModule, APP_INITIALIZER }      from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpModule } from '@angular/http';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { GridModule } from '@progress/kendo-angular-grid';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { FormsModule } from '@angular/forms';
import { LayoutModule } from '@progress/kendo-angular-layout';
import { ToastrModule } from 'ngx-toastr';
import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
import { load, IntlModule } from '@progress/kendo-angular-intl';
import { InputsModule } from '@progress/kendo-angular-inputs';

import { AppComponent } from './app.component';
import { mappers, services, components, helpers } from './global';
import { DateFormatPipe } from './pipes/date-format.pipe';
import { AppRoutingModule } from './app-routing.module';
import { AppConfigService } from './services/app-config.service';

load(
    require("node_modules/cldr-data/supplemental/likelySubtags.json"),
    require("node_modules/cldr-data/supplemental/currencyData.json"),
    require("node_modules/cldr-data/supplemental/weekData.json"),
    require("node_modules/cldr-data/main/it/numbers.json"),
    require("node_modules/cldr-data/main/it/currencies.json"),
    require("node_modules/cldr-data/main/it/dateFields.json"),
    require("node_modules/cldr-data/main/it/ca-gregorian.json"),
    require("node_modules/cldr-data/main/it/timeZoneNames.json")
);

export function initializeConfig(appConfig: AppConfigService) {
    return () => appConfig.load();
}

@NgModule({
    imports: [BrowserModule, IntlModule, BrowserAnimationsModule, HttpModule, InputsModule, GridModule, ButtonsModule, LayoutModule, AppRoutingModule, FormsModule, ToastrModule.forRoot(), DateInputsModule],
    declarations: [components, DateFormatPipe],
    providers: [mappers, services, helpers, { provide: LocationStrategy, useClass: HashLocationStrategy },
        { provide: LOCALE_ID, useValue: 'it-IT' },
        { provide: APP_INITIALIZER, useFactory: initializeConfig, deps: [AppConfigService], multi: true }],
    bootstrap: [AppComponent]
})
export class AppModule { }
