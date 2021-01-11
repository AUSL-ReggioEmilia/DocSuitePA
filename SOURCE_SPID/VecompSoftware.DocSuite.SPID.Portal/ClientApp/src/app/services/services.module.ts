import { NgModule, APP_INITIALIZER } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouteReuseStrategy } from '@angular/router';

import { AuthService } from './auth/auth.service';
import { AuthGuard } from './auth/auth.guard';
import { SpidApplicationService } from './spidapp/spid-application.service';
import { LocalAuthInterceptorService } from './shared/interceptors/local-auth-interceptor.service';
import { BaseLocalHttpService } from './shared/base-local-http.service';
import { CacheInterceptorService } from './shared/interceptors/cache-interceptor.service';
import { SpidRouteReuseStrategy } from './spidapp/spid-route.reusestrategy';
import { UserService } from './shared/user.service';
import { BaseWebapiHttpService } from './shared/base-webapi-http.service';
import { LoggingService } from './shared/logging.service';
import { DataTypeInterceptorService } from './shared/interceptors/data-type-interceptor.service';
import { ConfigurationService } from './shared/configuration.service';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [],
  providers: [
      AuthService,
      AuthGuard,
      SpidApplicationService,
      {
          provide: HTTP_INTERCEPTORS,
          useClass: LocalAuthInterceptorService,
          multi: true
      },
      {
          provide: HTTP_INTERCEPTORS,
          useClass: CacheInterceptorService,
          multi: true
      },
      {
          provide: HTTP_INTERCEPTORS,
          useClass: DataTypeInterceptorService,
          multi: true
      },
      {
          provide: RouteReuseStrategy,
          useClass: SpidRouteReuseStrategy
      },
      BaseLocalHttpService,
      UserService,
      BaseWebapiHttpService,
      LoggingService,      
      {
          provide: APP_INITIALIZER,
          useFactory: loadConfigurationData,
          deps: [ConfigurationService],
          multi: true
      },
      ConfigurationService
  ]
})
export class ServicesModule { }

export function loadConfigurationData(configurationService: ConfigurationService) {
    return () => configurationService.loadConfigurationData();
}
