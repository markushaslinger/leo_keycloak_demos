import { Routes } from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {CounterComponent} from "./counter/counter.component";
import {ApiDemoComponent} from "./api-demo/api-demo.component";
import { AuthGuard } from "../core/util/auth-guard";
import { Role } from "../core/util/leo-token";

export const routes: Routes = [
  {path: '', redirectTo: '/home', pathMatch: 'full'},
  {path: 'home', component: HomeComponent},
  {
    path: 'counter',
    component: CounterComponent,
    canActivate: [AuthGuard],
    data: {
      roles: [Role.Student]
    }
  },
  {path: 'api-demo', component: ApiDemoComponent}
];
