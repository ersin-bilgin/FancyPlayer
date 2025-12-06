import { Component } from "@angular/core";
import { NotificationsWidget } from "./components/notificationswidget/notificationswidget.component";
import { StatsWidget } from "./components/statswidget/statswidget.component";
import { RecentSalesWidget } from "./components/recentsaleswidget/recentsaleswidget.component";
import { BestSellingWidget } from "./components/bestsellingwidget/bestsellingwidget.component";
import { RevenueStreamWidget } from "./components/revenuestreamwidget/revenuestreamwidget.component";

@Component({
  selector: "app-home-index",
  standalone: true,
  templateUrl: "./index.component.html",
  imports: [StatsWidget, RecentSalesWidget, BestSellingWidget, RevenueStreamWidget, NotificationsWidget],
})
export class IndexComponent {}