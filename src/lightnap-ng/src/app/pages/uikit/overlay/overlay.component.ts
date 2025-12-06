import { Component, OnInit } from "@angular/core";
import { ConfirmationService, MessageService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { ConfirmDialogModule } from "primeng/confirmdialog";
import { ConfirmPopupModule } from "primeng/confirmpopup";
import { DialogModule } from "primeng/dialog";
import { DrawerModule } from "primeng/drawer";
import { InputTextModule } from "primeng/inputtext";
import { Popover, PopoverModule } from "primeng/popover";
import { TableModule } from "primeng/table";
import { ToastModule } from "primeng/toast";
import { TooltipModule } from "primeng/tooltip";
import { FormsModule } from "@angular/forms";
import { Product, ProductService } from "../service/product.service";

@Component({
  selector: "app-overlay-demo",
  standalone: true,
  imports: [
    ToastModule,
    DialogModule,
    ButtonModule,
    DrawerModule,
    PopoverModule,
    ConfirmPopupModule,
    ConfirmDialogModule,
    InputTextModule,
    FormsModule,
    TooltipModule,
    TableModule,
    ToastModule,
  ],
  templateUrl: "./overlay.component.html",
  providers: [ProductService, ConfirmationService, MessageService],
})
export class OverlayDemo implements OnInit {
  images: any[] = [];

  display: boolean = false;

  products: Product[] = [];

  visibleLeft: boolean = false;

  visibleRight: boolean = false;

  visibleTop: boolean = false;

  visibleBottom: boolean = false;

  visibleFull: boolean = false;

  displayConfirmation: boolean = false;

  selectedProduct!: Product;

  constructor(
    private productService: ProductService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.productService.getProductsSmall().then(products => (this.products = products));

    this.images = [
      {
        itemImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria1.jpg",
        thumbnailImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria1s.jpg",
        alt: "Description for Image 1",
        title: "Title 1",
      },
      {
        itemImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria2.jpg",
        thumbnailImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria2s.jpg",
        alt: "Description for Image 2",
        title: "Title 2",
      },
      {
        itemImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria3.jpg",
        thumbnailImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria3s.jpg",
        alt: "Description for Image 3",
        title: "Title 3",
      },
      {
        itemImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria4.jpg",
        thumbnailImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria4s.jpg",
        alt: "Description for Image 4",
        title: "Title 4",
      },
      {
        itemImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria5.jpg",
        thumbnailImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria5s.jpg",
        alt: "Description for Image 5",
        title: "Title 5",
      },
      {
        itemImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria6.jpg",
        thumbnailImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria6s.jpg",
        alt: "Description for Image 6",
        title: "Title 6",
      },
      {
        itemImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria7.jpg",
        thumbnailImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria7s.jpg",
        alt: "Description for Image 7",
        title: "Title 7",
      },
      {
        itemImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria8.jpg",
        thumbnailImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria8s.jpg",
        alt: "Description for Image 8",
        title: "Title 8",
      },
      {
        itemImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria9.jpg",
        thumbnailImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria9s.jpg",
        alt: "Description for Image 9",
        title: "Title 9",
      },
      {
        itemImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria10.jpg",
        thumbnailImageSrc: "https://primefaces.org/cdn/primeng/images/galleria/galleria10s.jpg",
        alt: "Description for Image 10",
        title: "Title 10",
      },
    ];
  }

  confirm(event: Event) {
    this.confirmationService.confirm({
      key: "confirm2",
      target: event.target || new EventTarget(),
      message: "Are you sure that you want to proceed?",
      icon: "pi pi-exclamation-triangle",
      accept: () => {
        this.messageService.add({ severity: "info", summary: "Confirmed", detail: "You have accepted" });
      },
      reject: () => {
        this.messageService.add({ severity: "error", summary: "Rejected", detail: "You have rejected" });
      },
    });
  }

  open() {
    this.display = true;
  }

  close() {
    this.display = false;
  }

  toggleDataTable(op: Popover, event: any) {
    op.toggle(event);
  }

  onProductSelect(op: Popover, event: any) {
    op.hide();
    this.messageService.add({ severity: "info", summary: "Product Selected", detail: event?.data.name, life: 3000 });
  }

  openConfirmation() {
    this.displayConfirmation = true;
  }

  closeConfirmation() {
    this.displayConfirmation = false;
  }
}
