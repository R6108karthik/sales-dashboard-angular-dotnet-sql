import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateProduct, Product } from '../../models/product';
import { ProductService } from '../../services/product';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.html',
  styleUrls: ['./products.css']
})
export class Products implements OnInit {
  products: Product[] = [];
  loading = false;
  errorMessage = '';

  form: CreateProduct = {
    productName: '',
    category: '',
    price: 0,
    stockQuantity: 0
  };

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;

    this.productService.getProducts().subscribe({
      next: products => {
        this.products = products;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load products.';
        this.loading = false;
      }
    });
  }

  saveProduct(): void {
    this.errorMessage = '';

    this.productService.createProduct(this.form).subscribe({
      next: () => {
        this.resetForm();
        this.loadProducts();
      },
      error: () => {
        this.errorMessage = 'Unable to create product.';
      }
    });
  }

  deleteProduct(id: number): void {
    const confirmed = confirm('Delete this product?');

    if (!confirmed) {
      return;
    }

    this.productService.deleteProduct(id).subscribe({
      next: () => this.loadProducts(),
      error: () => {
        this.errorMessage = 'Unable to delete product.';
      }
    });
  }

  resetForm(): void {
    this.form = {
      productName: '',
      category: '',
      price: 0,
      stockQuantity: 0
    };
  }
}