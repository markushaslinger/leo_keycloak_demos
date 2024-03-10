import { Routes } from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {CounterComponent} from "./counter/counter.component";
import {ApiDemoComponent} from "./api-demo/api-demo.component";

export const routes: Routes = [
  {path: '', redirectTo: '/home', pathMatch: 'full'},
  {path: 'home', component: HomeComponent},
  {path: 'counter', component: CounterComponent},
  {path: 'api-demo', component: ApiDemoComponent}
];
