import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { Customer } from '../../models/customer';
import { CreateOrder, Order } from '../../models/order';
import { Product } from '../../models/product';
import { CustomerService } from '../../services/customer';
import { OrderService } from '../../services/order';
import { ProductService } from '../../services/product';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [CustomerService, ProductService, OrderService],
  templateUrl: './orders.html',
  styleUrls: ['./orders.css']
})
export class Orders implements OnInit {
  customers: Customer[] = [];
  products: Product[] = [];
  orders: Order[] = [];
  loading = false;
  errorMessage = '';
  successMessage = '';

  form: CreateOrder = {
    customerId: 0,
    items: [{ productId: 0, quantity: 1 }]
  };

  constructor(
    private customerService: CustomerService,
    private productService: ProductService,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    this.loadPageData();
  }

  loadPageData(): void {
    this.loading = true;

    forkJoin({
      customers: this.customerService.getCustomers(),
      products: this.productService.getProducts(),
      orders: this.orderService.getOrders()
    }).subscribe({
      next: data => {
        this.customers = data.customers;
        this.products = data.products;
        this.orders = data.orders;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load order data.';
        this.loading = false;
      }
    });
  }

  addItem(): void {
    this.form.items.push({ productId: 0, quantity: 1 });
  }

  removeItem(index: number): void {
    if (this.form.items.length > 1) {
      this.form.items.splice(index, 1);
    }
  }

  createOrder(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (
      this.form.customerId === 0 ||
      this.form.items.some(item => item.productId === 0 || item.quantity <= 0)
    ) {
      this.errorMessage = 'Select a customer, product, and valid quantity.';
      return;
    }

    this.orderService.createOrder(this.form).subscribe({
      next: () => {
        this.successMessage = 'Order created successfully.';
        this.form = {
          customerId: 0,
          items: [{ productId: 0, quantity: 1 }]
        };
        this.loadPageData();
      },
      error: (error: any) => {
        this.errorMessage = error?.error || 'Unable to create order.';
      }
    });
  }

  deleteOrder(id: number): void {
    if (!confirm('Delete this order?')) {
      return;
    }

    this.orderService.deleteOrder(id).subscribe({
      next: () => this.loadPageData(),
      error: () => {
        this.errorMessage = 'Unable to delete order.';
      }
    });
  }
}