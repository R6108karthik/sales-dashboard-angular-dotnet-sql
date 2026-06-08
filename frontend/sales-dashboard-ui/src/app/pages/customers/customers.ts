import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CreateCustomer, Customer } from '../../models/customer';
import { CustomerService } from '../../services/customer';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [CustomerService],
  templateUrl: './customers.html',
  styleUrls: ['./customers.css']
})
export class Customers implements OnInit {
  customers: Customer[] = [];
  loading = false;
  errorMessage = '';

  form: CreateCustomer = {
    customerName: '',
    email: '',
    phoneNumber: '',
    city: ''
  };

  constructor(private customerService: CustomerService) {}

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.loading = true;

    this.customerService.getCustomers().subscribe({
      next: customers => {
        this.customers = customers;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load customers.';
        this.loading = false;
      }
    });
  }

  saveCustomer(): void {
    this.errorMessage = '';

    this.customerService.createCustomer(this.form).subscribe({
      next: () => {
        this.resetForm();
        this.loadCustomers();
      },
      error: () => {
        this.errorMessage = 'Unable to create customer.';
      }
    });
  }

  deleteCustomer(id: number): void {
    const confirmed = confirm('Delete this customer?');

    if (!confirmed) {
      return;
    }

    this.customerService.deleteCustomer(id).subscribe({
      next: () => this.loadCustomers(),
      error: () => {
        this.errorMessage = 'Unable to delete customer.';
      }
    });
  }

  resetForm(): void {
    this.form = {
      customerName: '',
      email: '',
      phoneNumber: '',
      city: ''
    };
  }
}