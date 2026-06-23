import { Routes } from '@angular/router';
import { DashboardComponent } from './pages/dashboard/dashboard';
import { Customers } from './pages/customers/customers';
import { Products } from './pages/products/products';
import { Orders } from './pages/orders/orders';

export const routes: Routes = [
  { path: '', component: DashboardComponent },
  { path: 'customers', component: Customers },
  { path: 'products', component: Products },
  { path: 'orders', component: Orders, pathMatch: 'full' },
  { path: '**', redirectTo: '' }
];
