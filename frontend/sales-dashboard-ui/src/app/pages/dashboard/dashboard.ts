import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { DashboardSummary } from '../../models/dashboard-summary';
import { DashboardService } from '../../services/dashboard';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {
  summary: DashboardSummary | null = null;
  loading = false;
  errorMessage = '';

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadSummary();
  }

  loadSummary(): void {
    this.loading = true;
    this.errorMessage = '';

    this.dashboardService.getSummary().subscribe({
      next: (summary: DashboardSummary) => {
        this.summary = summary;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load dashboard summary.';
        this.loading = false;
      }
    });
  }
}