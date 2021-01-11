import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { JwtAuthHelper } from './auth/jwt-auth.helper';
import { IdpImageHelper } from './common/idp-image.helper';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [],
  providers: [JwtAuthHelper, IdpImageHelper]
})
export class HelpersModule { }
