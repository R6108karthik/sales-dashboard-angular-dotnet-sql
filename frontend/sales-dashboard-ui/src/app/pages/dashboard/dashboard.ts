import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { CustomerService } from '../../services/customer';
import { ProductService } from '../../services/product';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class Dashboard implements OnInit {
  totalCustomers = 0;
  totalProducts = 0;
  totalStock = 0;
  inventoryValue = 0;
  loading = false;
  errorMessage = '';

  constructor(
    private customerService: CustomerService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading = true;

    this.customerService.getCustomers().subscribe({
      next: customers => {
        this.totalCustomers = customers.length;
        this.loadProductStats();
      },
      error: () => {
        this.errorMessage = 'Unable to load dashboard customer data.';
        this.loading = false;
      }
    });
  }

  private loadProductStats(): void {
    this.productService.getProducts().subscribe({
      next: products => {
        this.totalProducts = products.length;
        this.totalStock = products.reduce(
          (sum, product) => sum + product.stockQuantity,
          0
        );
        this.inventoryValue = products.reduce(
          (sum, product) => sum + product.price * product.stockQuantity,
          0
        );
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load dashboard product data.';
        this.loading = false;
      }
    });
  }
}