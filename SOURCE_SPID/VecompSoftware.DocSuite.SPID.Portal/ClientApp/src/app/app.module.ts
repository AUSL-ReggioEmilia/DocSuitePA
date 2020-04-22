import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { NgProgressModule } from '@ngx-progressbar/core';
import { NgProgressRouterModule } from '@ngx-progressbar/router';

import { ServicesModule } from './services/services.module';
import { HelpersModule } from './helpers/helpers.module';
import { GlobalErrorHandler } from './global-error-handler';
import { AppComponent } from './components/app/app.component';
import { HomeComponent } from './components/home/home.component';
import { AppHeaderComponent } from './components/app/app-header/app-header.component';
import { AppSidebarComponent } from './components/app/app-sidebar/app-sidebar.component';
import { NavDropdownDirective, NavDropdownToggleDirective } from './components/shared/directives/nav-dropdown.directive';
import { SidebarToggleDirective, MobileSidebarToggleDirective } from './components/shared/directives/sidebar.directive';
import { AppHeaderUserComponent } from './components/app/app-header/app-header-user.component';
import { AppSidebarUserComponent } from './components/app/app-sidebar/app-sidebar-user.component';
import { AppSidebarHeaderComponent } from './components/app/app-sidebar/app-sidebar-header.component';
import { AppSidebarNavComponent } from './components/app/app-sidebar/app-sidebar-nav.component';
import { HomeApplicationComponent } from './components/home/home-application.component';
import { SpidAppComponent } from './components/spidapp/spidapp.component';
import { AuthGuard } from './services/auth/auth.guard';
import { FullLayoutComponent } from './components/shared/layouts/full-layout.component';
import { SimpleLayoutComponent } from './components/shared/layouts/simple-layout.component';
import { P404Component } from './components/shared/layouts/404.component';
import { P500Component } from './components/shared/layouts/500.component';
import { LoginComponent } from './components/shared/components/login/login.component';
import { ApplicationsFilterPipe } from './components/shared/pipes/applications-filter.pipe';
import { Safe } from './components/shared/pipes/safe.pipe';
import { LoadingComponent } from './components/shared/components/loading/loading.component';
import { AppSidebarLoadingComponent } from './components/app/app-sidebar/app-sidebar-loading.component';
import { AppHeaderCollapseButtonComponent } from './components/app/app-header/app-header-collapse-button.component';
import { SpidappContentComponent } from './components/spidapp/spidapp-content.component';
import { LoginSpidComponent } from './components/shared/components/login/spid/login-spid.component';
import { LoginFederaComponent } from './components/shared/components/login/federa/login-federa.component';
import { SpidButtonComponent } from './components/shared/components/login/spid/spid-button.component';
import { LoginErrorMessageComponent } from './components/shared/components/login/shared/login-error-message.component';
import { FederaButtonComponent } from './components/shared/components/login/federa/federa-button.component';

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        AppHeaderComponent,
        AppSidebarComponent,
        NavDropdownDirective,
        NavDropdownToggleDirective,
        SidebarToggleDirective,
        MobileSidebarToggleDirective,
        AppHeaderUserComponent,
        AppSidebarUserComponent,
        AppSidebarHeaderComponent,
        AppSidebarNavComponent,
        HomeApplicationComponent,
        SpidAppComponent,
        FullLayoutComponent,
        SimpleLayoutComponent,
        P404Component,
        P500Component,
        LoginComponent,
        ApplicationsFilterPipe,
        Safe,
        LoadingComponent,
        AppSidebarLoadingComponent,
        AppHeaderCollapseButtonComponent,
        SpidappContentComponent,
        LoginSpidComponent,
        LoginFederaComponent,
        SpidButtonComponent,
        LoginErrorMessageComponent,
        FederaButtonComponent
    ],
    providers: [
        {
            provide: ErrorHandler,
            useClass: GlobalErrorHandler
        }        
    ],
    imports: [
        BrowserModule,
        CommonModule,
        HttpClientModule,
        FormsModule,
        ServicesModule,
        HelpersModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            {
                path: '', component: FullLayoutComponent, children: [
                    { path: '', component: HomeComponent, pathMatch: 'full' },
                    { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },                    
                    { path: 'applications', component: SpidAppComponent, canActivate: [AuthGuard] },
                    {
                        path: 'applications/:id', component: SpidAppComponent, canActivate: [AuthGuard]
                    },
                ]
            },
            {
                path: 'pages', component: SimpleLayoutComponent, children: [
                    { path: '404', component: P404Component },
                    { path: '500', component: P500Component },
                    { path: 'login', component: LoginComponent }
                ]
            },
            { path: '**', redirectTo: 'home' }
        ]),
        NgProgressModule.forRoot(),
        NgProgressRouterModule
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
