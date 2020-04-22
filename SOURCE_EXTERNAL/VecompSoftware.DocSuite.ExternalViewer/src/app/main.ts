import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { enableProdMode } from '@angular/core';

import { AppModule } from './app.module';
import { environment } from '../environments/environment';

//togliere il commento in produzione
if (environment.production) {
    enableProdMode();
}
platformBrowserDynamic().bootstrapModule(AppModule);


