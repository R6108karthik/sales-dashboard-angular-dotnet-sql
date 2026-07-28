# Sales Dashboard Angular .NET SQL

A full-stack sales dashboard built with Angular on the frontend, ASP.NET Core on the backend, and SQL Server for persistence. The project includes real-time updates via SignalR and message-based integration with RabbitMQ.

## Project Structure

- frontend/sales-dashboard-ui: Angular application
- backend: ASP.NET Core Web API, domain/application/infrastructure layers
- database: SQL Server related assets and scripts
- docker-compose.yml: containerized services for the stack

## Prerequisites

- Node.js and npm
- .NET SDK
- Docker Desktop (optional, for running the stack locally)
- SQL Server (or Docker-based SQL container)

## Getting Started

### Frontend

```bash
cd frontend/sales-dashboard-ui
npm install
npm start
```

### Backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run --project SalesDashboard.Api/SalesDashboard.Api.csproj
```

### Docker Compose

```bash
docker compose up --build
```

## Features

- Sales dashboard UI
- Customer, product, and order management
- Real-time dashboard updates with SignalR
- Event-driven messaging with RabbitMQ
- SQL Server data storage

## Notes

Adjust connection strings and environment settings in the backend configuration files as needed for your local environment.
