export interface Customer {
  id: number;
  customerName: string;
  email: string;
  phoneNumber: string;
  city: string;
}

export interface CreateCustomer {
  customerName: string;
  email: string;
  phoneNumber: string;
  city: string;
}