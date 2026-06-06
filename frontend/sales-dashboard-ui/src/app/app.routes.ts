import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Customers } from './pages/customers/customers';
import { Products } from './pages/products/products';
import { Orders } from './pages/orders/orders';

export const routes: Routes = [
  { path: '', component: Dashboard },
  { path: 'customers', component: Customers },
  { path: 'products', component: Products },
  { path: 'orders', component: Orders, pathMatch: 'full' },
  { path: '**', redirectTo: '' }
];
