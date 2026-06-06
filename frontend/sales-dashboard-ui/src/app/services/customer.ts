import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateCustomer, Customer as CustomerModel } from '../models/customer';

@Injectable({
  providedIn: 'root',
})
export class Customer {
  private readonly apiUrl = 'http://localhost:5151/api/Customers';

  constructor(private readonly http: HttpClient) {}

   getCustomers(): Observable<Customer[]> {
    return this.http.get<Customer[]>(this.apiUrl);
  }

  createCustomer(customer: CreateCustomer): Observable<Customer> {
    return this.http.post<Customer>(this.apiUrl, customer);
  }

  deleteCustomer(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
