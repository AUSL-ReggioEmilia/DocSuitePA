import { NgModule, ErrorHandler, LOCALE_ID, APP_INITIALIZER } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { NumericTextBoxModule } from '@progress/kendo-angular-inputs';
import { IntlModule } from '@progress/kendo-angular-intl';

import '@progress/kendo-angular-intl/locales/it/all';

import { AppComponent } from './components/app/app.component';
import { AppHeaderComponent } from './components/app/app-header/app-header.component';
import { HomeComponent } from './components/home/home.component';
import { AppSidebarComponent } from './components/app/app-sidebar/app-sidebar.component';
import { NewRequestComponent } from './components/requests/new-request.component';
import { MyRequestsComponent } from './components/requests/my-requests.component';

import { GlobalErrorHandler } from './global-error-handler';
import { ServicesModule } from './services/services.module';
import { MappersModule } from './mappers/mappers.module';
import { BreadcrumbsComponent } from './components/shared/components/layout/breadcrumbs.component';
import { AppFooterComponent } from './components/app/app-footer/app-footer.component';
import { SidebarToggleDirective, MobileSidebarToggleDirective } from './components/shared/directives/sidebar.directive';
import { ErrorComponent } from './components/shared/components/layout/error.component';
import { FormShowErrorsComponent } from './components/shared/components/layout/form-show-errors.component';
import { TelephoneNumberValidatorDirective } from './components/shared/directives/telephone-number-validator.directive';
import { EmailValidatorDirective } from './components/shared/directives/email-validator.directive';
import { RequestGridComponent } from './components/shared/components/requests/request-grid.component';
import { LoadingComponent } from './components/shared/components/layout/loading.component';
import { MyLastRequestsComponent } from './components/home/shared/widgets/my-last-requests.component';
import { RequestsComponent } from './components/shared/components/requests/requests.component';
import { LastRequestsPipe } from './components/home/shared/pipes/last-requests.pipe';
import { PendingRequestsComponent } from './components/home/shared/widgets/pending-requests.component';
import { ConfirmedRequestsComponent } from './components/home/shared/widgets/confirmed-requests.component';
import { DeniedRequestsComponent } from './components/home/shared/widgets/denied-requests.component';
import { DeniedRequestsCountPipe } from './components/home/shared/pipes/denied-requests-count.pipe';
import { ConfirmedRequestsCountPipe } from './components/home/shared/pipes/confirmed-requests-count.pipe';
import { PendingRequestsCountPipe } from './components/home/shared/pipes/pending-requests-count.pipe';
import { UrlExternalViewerPipe } from './components/shared/pipes/url-external-viewer.pipe';
import { AuthGuard } from './services/auth/auth.guard';
import { NotAuthorizedComponent } from './components/shared/components/layout/not-authorized.component';

@NgModule({
    declarations: [
        AppComponent,
        AppHeaderComponent,
        HomeComponent,
        AppSidebarComponent,
        NewRequestComponent,
        MyRequestsComponent,
        BreadcrumbsComponent,
        AppFooterComponent,
        SidebarToggleDirective,
        MobileSidebarToggleDirective,
        ErrorComponent,
        FormShowErrorsComponent,
        TelephoneNumberValidatorDirective,
        EmailValidatorDirective,
        RequestGridComponent,
        LoadingComponent,
        MyLastRequestsComponent,
        RequestsComponent,
        LastRequestsPipe,
        PendingRequestsComponent,
        ConfirmedRequestsComponent,
        DeniedRequestsComponent,
        DeniedRequestsCountPipe,
        ConfirmedRequestsCountPipe,
        PendingRequestsCountPipe,
        UrlExternalViewerPipe,
        NotAuthorizedComponent
    ],
    providers: [
        {
            provide: ErrorHandler,
            useClass: GlobalErrorHandler
        },
        {
            provide: LOCALE_ID,
            useValue: 'it-IT'
        }
    ],
    imports: [
        CommonModule,
        HttpClientModule,
        FormsModule,
        DatePickerModule,
        NumericTextBoxModule,
        IntlModule,
        ServicesModule,
        MappersModule,
        BrowserAnimationsModule,
        BrowserModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
            { path: 'nuova-richiesta', component: NewRequestComponent, data: { breadcrumb: "Nuova richiesta" }, canActivate: [AuthGuard] },
            { path: 'le-mie-richieste', component: MyRequestsComponent, data: { breadcrumb: "Le mie richieste" }, canActivate: [AuthGuard] },
            { path: 'error', component: ErrorComponent },
            { path: 'not-authorized', component: NotAuthorizedComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}