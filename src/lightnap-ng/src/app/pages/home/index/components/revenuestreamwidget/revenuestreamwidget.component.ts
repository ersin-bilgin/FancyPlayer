import { Component, OnInit, OnDestroy } from "@angular/core";
import { ChartModule } from "primeng/chart";

@Component({
  standalone: true,
  selector: "app-revenue-stream-widget",
  imports: [ChartModule],
  templateUrl: "./revenuestreamwidget.component.html",
})
export class RevenueStreamWidget implements OnInit, OnDestroy {
  chartData: any;
  chartOptions: any;

  ngOnInit() {
    this.initChart();
  }

  initChart() {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue("--text-color") || "#495057";
    const borderColor = documentStyle.getPropertyValue("--surface-border") || "#dee2e6";
    const textMutedColor = documentStyle.getPropertyValue("--text-color-secondary") || "#6c757d";

    this.chartData = {
      labels: ["Q1", "Q2", "Q3", "Q4"],
      datasets: [
        {
          type: "bar",
          label: "Subscriptions",
          backgroundColor: "#3B82F6",
          data: [4000, 10000, 15000, 4000],
          barThickness: 32,
        },
        {
          type: "bar",
          label: "Advertising",
          backgroundColor: "#60A5FA",
          data: [2100, 8400, 2400, 7500],
          barThickness: 32,
        },
        {
          type: "bar",
          label: "Affiliate",
          backgroundColor: "#93C5FD",
          data: [4100, 5200, 3400, 7400],
          borderRadius: {
            topLeft: 8,
            topRight: 8,
            bottomLeft: 0,
            bottomRight: 0,
          },
          borderSkipped: false,
          barThickness: 32,
        },
      ],
    };

    this.chartOptions = {
      maintainAspectRatio: false,
      aspectRatio: 0.8,
      plugins: {
        legend: {
          labels: {
            color: textColor,
          },
        },
      },
      scales: {
        x: {
          stacked: true,
          ticks: {
            color: textMutedColor,
          },
          grid: {
            color: "transparent",
            borderColor: "transparent",
          },
        },
        y: {
          stacked: true,
          ticks: {
            color: textMutedColor,
          },
          grid: {
            color: borderColor,
            borderColor: "transparent",
            drawTicks: false,
          },
        },
      },
    };
  }

  ngOnDestroy() {
    // Cleanup if needed
  }
}