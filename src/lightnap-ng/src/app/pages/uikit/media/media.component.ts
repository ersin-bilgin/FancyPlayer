import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { ButtonModule } from "primeng/button";
import { CarouselModule } from "primeng/carousel";
import { GalleriaModule } from "primeng/galleria";
import { ImageModule } from "primeng/image";
import { TagModule } from "primeng/tag";
import { PhotoService } from "../service/photo.service";
import { Product, ProductService } from "../service/product.service";
import { Observable, from } from "rxjs";

@Component({
  selector: "app-media-demo",
  standalone: true,
  imports: [CommonModule, CarouselModule, ButtonModule, GalleriaModule, ImageModule, TagModule],
  templateUrl: "./media.component.html",
  providers: [ProductService, PhotoService],
})
export class MediaDemo implements OnInit {
  products$!: Observable<Product[]>;
  images$!: Observable<any[]>;

  galleriaResponsiveOptions: any[] = [
    {
      breakpoint: "1024px",
      numVisible: 5,
    },
    {
      breakpoint: "960px",
      numVisible: 4,
    },
    {
      breakpoint: "768px",
      numVisible: 3,
    },
    {
      breakpoint: "560px",
      numVisible: 1,
    },
  ];

  carouselResponsiveOptions: any[] = [
    {
      breakpoint: "1024px",
      numVisible: 3,
      numScroll: 3,
    },
    {
      breakpoint: "768px",
      numVisible: 2,
      numScroll: 2,
    },
    {
      breakpoint: "560px",
      numVisible: 1,
      numScroll: 1,
    },
  ];

  constructor(
    private productService: ProductService,
    private photoService: PhotoService
  ) {}

  ngOnInit() {
    this.products$ = from(this.productService.getProductsSmall());
    this.images$ = from(this.photoService.getImages());
  }

  getSeverity(status: string) {
    switch (status) {
      case "INSTOCK":
        return "success";
      case "LOWSTOCK":
        return "warn";
      case "OUTOFSTOCK":
        return "danger";
      default:
        return "success";
    }
  }
}
