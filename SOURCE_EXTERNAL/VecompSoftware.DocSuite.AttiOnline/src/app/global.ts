import { ResolutionService } from './services/resolution.service';
import { LoadingSpinnerService } from './services/loading-spinner.service';
import { ResolutionMapper } from './mappers/resolution.mapper';
import { AppComponent } from './app.component';
import { ExecutiveResolutionsComponent } from './components/executive-resolutions.component';
import { PublishedResolutionsComponent } from './components/published-resolutions.component';
import { AlboPretorioComponent } from './components/albo-pretorio.component';
import { ResolutionGridComponent } from './components/templates/resolution-grid.component';
import { ResolutionTypeComponent } from './components/templates/resolution-type.component';
import { ResolutionFilterComponent } from './components/templates/resolution-filter.component';
import { LoadingComponent } from './components/loading.component';
import { ErrorPageComponent } from './components/error-page.component';
import { BaseHelper } from './helpers/base.helper';
import { AppConfigService } from './services/app-config.service';

export const services = [ ResolutionService, LoadingSpinnerService, AppConfigService ];
export const mappers = [ ResolutionMapper];
export const helpers = [ BaseHelper];
export const components = [ AppComponent, ExecutiveResolutionsComponent, AlboPretorioComponent, ErrorPageComponent,
    ResolutionGridComponent, ResolutionTypeComponent, LoadingComponent, ResolutionFilterComponent, PublishedResolutionsComponent ];
export const routesDefinition = {
    consultazioneAtti: 'Consultazione',
    attiEsecutivi: 'Esecutivi',
    attiPubblicati: 'Pubblicati',
    alboPretorio: 'Albo',
    errorPage: 'error-page'
};