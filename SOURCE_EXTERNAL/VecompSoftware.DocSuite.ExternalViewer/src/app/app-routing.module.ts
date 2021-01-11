import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home.component';
import { AuthenticationComponent } from './components/commons/authentication.component';
import { DocumentComponent } from './components/commons/document.component';
import { ErrorPageComponent } from './components/commons/error-page.component';
import { FascicleMenuResolve } from './services/menu/fascicle-menu.resolve';
import { FascicleMenuComponent } from './components/menu/fascicle-menu.component';
import { FascicleSummaryComponent } from './components/fascicles/fascicle-summary.component';
import { DossiersComponent } from './components/dossiers/dossiers.component';
import { DossierResolve } from './resolvers/dossiers/dossiers.resolve';
import { DossierSummaryComponent } from './components/dossiers/dossier-summary.component';
import { ProtocolSummaryComponent } from './components/protocols/protocol-summary.component';

const routes: Routes = [
    {
        path: '',
        component: HomeComponent,
    },
    {
        path: 'Auth/appId/:appId/authrule/:authrule/kind/:kind/param/:param/redirectUrl/:url',
        component: AuthenticationComponent
    },
    {
        path: 'Fascicolo',
        children: [
            {
                path: 'identificativo/:id',
                component: FascicleMenuComponent,
                resolve: {
                    responseModel: FascicleMenuResolve
                },
                children: [
                    {
                        path: '',
                        redirectTo: 'Sommario',
                        pathMatch: 'full'
                    },
                    {
                        path: 'Sommario',
                        component: FascicleSummaryComponent
                    },
                    {
                        path: 'Protocollo/Anno/:year/Numero/:number',
                        component: ProtocolSummaryComponent
                    },
                    {
                        path: 'Documento/identificativo/:id',
                        component: DocumentComponent
                    }
                ]
            },
            {
                path: 'Titolo/:title',
                component: FascicleMenuComponent,
                resolve: {
                    responseModel: FascicleMenuResolve
                },
                children: [
                    {
                        path: '',
                        redirectTo: 'Sommario',
                        pathMatch: 'full'
                    },
                    {
                        path: 'Sommario',
                        component: FascicleSummaryComponent
                    },
                    {
                        path: 'Protocollo/Anno/:year/Numero/:number',
                        component: ProtocolSummaryComponent
                    },
                    {
                        path: 'Documento/identificativo/:id',
                        component: DocumentComponent
                    }
                ]
            }
        ]
    },
    {
        path: 'Dossier',
        children: [
            {
                path: 'identificativo/:id',
                component: DossiersComponent,
                resolve: {
                    responseModel: DossierResolve
                },
                children: [
                    {
                        path: '',
                        redirectTo: 'Sommario',
                        pathMatch: 'full'
                    },
                    {
                        path: 'Sommario',
                        component: DossierSummaryComponent
                    },
                    {
                        path: 'Fascicolo/:id',
                        component: FascicleSummaryComponent,
                        resolve: {
                            responseModel: FascicleMenuResolve
                        }
                    },
                    {
                        path: 'Protocollo/Anno/:year/Numero/:number',
                        component: ProtocolSummaryComponent
                    },
                    {
                        path: 'Documento/:id',
                        component: DocumentComponent
                    }
                ]
            },
            {
                path: 'Anno/:year/Numero/:number',
                component: DossiersComponent,
                resolve: {
                    responseModel: DossierResolve
                },
                children: [
                    {
                        path: '',
                        redirectTo: 'Sommario',
                        pathMatch: 'full'
                    },
                    {
                        path: 'Sommario',
                        component: DossierSummaryComponent
                    },
                    {
                        path: 'Fascicolo/:id',
                        component: FascicleSummaryComponent,
                        resolve: {
                            responseModel: FascicleMenuResolve
                        }
                    },
                    {
                        path: 'Protocollo/Anno/:year/Numero/:number',
                        component: ProtocolSummaryComponent
                    },
                    {
                        path: 'Documento/:id',
                        component: DocumentComponent
                    }
                ]
            }
        ]

    },
    {
        path: 'error-page',
        component: ErrorPageComponent
    },
    {
        path: '**',
        redirectTo: 'error-page'
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
    exports: [RouterModule]

})

export class AppRoutingModule { }

