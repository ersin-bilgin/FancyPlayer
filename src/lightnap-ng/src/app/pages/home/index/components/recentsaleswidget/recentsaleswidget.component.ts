import { Component, OnInit } from "@angular/core";
import { RippleModule } from "primeng/ripple";
import { TableModule } from "primeng/table";
import { ButtonModule } from "primeng/button";
import { CommonModule } from "@angular/common";
import { Observable, from } from "rxjs";
import { Product, ProductService } from "../../../../uikit/service/product.service";

@Component({
  standalone: true,
  selector: "app-recent-sales-widget",
  imports: [CommonModule, TableModule, ButtonModule, RippleModule],
  templateUrl: "./recentsaleswidget.component.html",
  providers: [ProductService],
})
export class RecentSalesWidget implements OnInit {
  products$!: Observable<Product[]>;

  constructor(private productService: ProductService) {}

  ngOnInit() {
    this.products$ = from(this.productService.getProductsSmall());
  }
}