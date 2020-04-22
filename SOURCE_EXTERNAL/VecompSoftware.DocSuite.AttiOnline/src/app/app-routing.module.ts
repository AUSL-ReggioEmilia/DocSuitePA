import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ExecutiveResolutionsComponent } from './components/executive-resolutions.component';
import { PublishedResolutionsComponent } from './components/published-resolutions.component';
import { AlboPretorioComponent } from './components/albo-pretorio.component';
import { ErrorPageComponent } from './components/error-page.component';
import { routesDefinition } from './global';

const routes: Routes = [
    {
        path: '',
        component: ErrorPageComponent,
    },
    {
        path: routesDefinition.consultazioneAtti,
        children: [
            {
                path: '',
                redirectTo: routesDefinition.errorPage,
                pathMatch: 'full'
            },
            {
                path: routesDefinition.attiEsecutivi,
                component: ExecutiveResolutionsComponent
            },
            {
                path: routesDefinition.attiPubblicati,
                component: PublishedResolutionsComponent
            },
            {
                path: routesDefinition.errorPage,
                component: ErrorPageComponent
            },
        ]
    },
    {
        path: routesDefinition.alboPretorio,
        component: AlboPretorioComponent
    },
    {
        path: routesDefinition.errorPage,
        component: ErrorPageComponent
    },
    {
        path: '**',
        redirectTo: routesDefinition.errorPage
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]

})

export class AppRoutingModule { }

