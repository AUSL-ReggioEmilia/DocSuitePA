import { NgModule, APP_INITIALIZER } from '@angular/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

import { CommonModule } from '@angular/common';
import { WorkflowService } from './workflows/workflow.service';
import { UserService } from './shared/user.service';
import { WorkflowStatusService } from './workflows/workflow-status.service';
import { BaseWebapiHttpService } from './shared/base-webapi-http.service';
import { ConfigurationService } from './shared/configuration.service';
import { AuthGuard } from './auth/auth.guard';
import { LoggingService } from './shared/logging.service';
import { CacheInterceptorService } from './shared/interceptors/cache-interceptor.service';
import { BaseLocalHttpService } from './shared/base-local-http.service';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [],
  providers: [
      BaseWebapiHttpService,
      BaseLocalHttpService,
      WorkflowService,
      WorkflowStatusService,
      ConfigurationService,
      {
          provide: APP_INITIALIZER,
          useFactory: loadConfigurationData,
          deps: [ConfigurationService],
          multi: true
      },
      UserService,
      {
          provide: APP_INITIALIZER,
          useFactory: loadUser,
          deps: [UserService],
          multi: true
      },
      {
          provide: HTTP_INTERCEPTORS,
          useClass: CacheInterceptorService,
          multi: true
      },
      AuthGuard,
      LoggingService
  ]
})
export class ServicesModule { }

export function loadConfigurationData(configurationService: ConfigurationService) {
    return () => configurationService.loadConfigurationData();
}

export function loadUser(userService: UserService) {
    return () => userService.loadUser();
}
