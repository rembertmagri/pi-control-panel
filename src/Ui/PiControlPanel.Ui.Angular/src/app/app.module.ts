import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { AccordionModule } from 'ngx-bootstrap/accordion';
import { SortableModule } from 'ngx-bootstrap/sortable';
import { PopoverModule } from 'ngx-bootstrap/popover';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { MatProgressSpinnerModule, MatSpinner } from '@angular/material/progress-spinner';
import { OverlayModule } from '@angular/cdk/overlay';

import { NgBytesPipeModule } from 'angular-pipes';

import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';

import { ApolloModule, APOLLO_OPTIONS } from 'apollo-angular';
import { HttpLinkModule, HttpLink } from 'apollo-angular-link-http';
import { InMemoryCache } from 'apollo-cache-inmemory';
import { split } from 'apollo-link';
import { WebSocketLink } from 'apollo-link-ws';
import { getMainDefinition } from 'apollo-utilities';

import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from '@interceptors/auth.interceptor';

import { environment } from '@environments/environment';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DashboardResolve } from './dashboard/dashboard.resolve';
import { RealTimeModalComponent } from './dashboard/modal/real-time-modal.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, resolve: { raspberryPi: DashboardResolve } },
  { path: '**', redirectTo: '/home' },
];

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    RealTimeModalComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(routes),
    AccordionModule.forRoot(),
    SortableModule.forRoot(),
    PopoverModule.forRoot(),
    BsDropdownModule.forRoot(),
    ModalModule.forRoot(),
    NgxChartsModule,
    MatProgressSpinnerModule,
    OverlayModule,
    //StoreModule.forRoot(appReducers),
    StoreDevtoolsModule.instrument({
      maxAge: 25,
      logOnly: environment.production
    }),
    ApolloModule,
    HttpLinkModule,
    NgBytesPipeModule
  ],
  entryComponents: [
    RealTimeModalComponent,
    MatSpinner
  ],
  providers: [
    DashboardResolve,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: APOLLO_OPTIONS,
      deps: [HttpLink],
      useFactory: (httpLink: HttpLink) => {
        return {
          cache: new InMemoryCache(),
          link: split(
            ({ query }) => {
              const definition = getMainDefinition(query);
              return definition.kind === 'OperationDefinition' && definition.operation === 'subscription';
            },
            new WebSocketLink({
              uri: `ws://${environment.graphqlEndpoint}/graphql`,
              options: {
                reconnect: true,
                connectionParams: () => {
                  const token = localStorage.getItem('jwt');
                  return token ? { 'Authorization': `Bearer ${token}` } : {};
                }
              }
            }),
            httpLink.create({ uri: `http://${environment.graphqlEndpoint}/graphql` }),
          )
        }
      }
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
