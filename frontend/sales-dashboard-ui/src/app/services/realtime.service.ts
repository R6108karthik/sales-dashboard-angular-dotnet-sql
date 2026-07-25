import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class RealtimeService {
  private hubConnection?: signalR.HubConnection;

  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/sales-dashboard')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connected'))
      .catch(error => console.log('SignalR connection error:', error));
  }

  onOrderCreated(callback: () => void): void {
    this.hubConnection?.on('orderCreated', () => {
      callback();
    });
  }
}