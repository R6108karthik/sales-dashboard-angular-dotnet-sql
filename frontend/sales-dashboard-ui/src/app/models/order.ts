export interface CreateOrderItem {
  productId: number;
  quantity: number;
}

export interface CreateOrder {
  customerId: number;
  items: CreateOrderItem[];
}

export interface OrderItem {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface Order {
  id: number;
  customerId: number;
  customerName: string;
  orderDate: string;
  status: string;
  totalAmount: number;
  items: OrderItem[];
}