import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Order, CreateOrder } from '../models/order';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
    private readonly apiUrl = `${environment.apiUrl}/Orders`;

  constructor(private http: HttpClient) {}

  getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(this.apiUrl);
  }

  createOrder(order: CreateOrder): Observable<Order> {
    return this.http.post<Order>(this.apiUrl, order);
  }

  deleteOrder(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
