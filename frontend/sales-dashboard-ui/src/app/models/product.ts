export interface Product {
  id: number;
  productName: string;
  category: string;
  price: number;
  stockQuantity: number;
}

export interface CreateProduct {
  productName: string;
  category: string;
  price: number;
  stockQuantity: number;
}