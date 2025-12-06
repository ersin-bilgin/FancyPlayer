import { Component, OnInit } from "@angular/core";
import { ConfirmationService, MessageService } from "primeng/api";
import { Product, ProductService } from "../service/product.service";
import { Table, TableModule } from "primeng/table";
import { ToastModule } from "primeng/toast";
import { ConfirmDialogModule } from "primeng/confirmdialog";
import { DialogModule } from "primeng/dialog";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { InputNumberModule } from "primeng/inputnumber";
import { SelectModule } from "primeng/select";
import { TagModule } from "primeng/tag";
import { ToolbarModule } from "primeng/toolbar";
import { FileUploadModule } from "primeng/fileupload";
import { RatingModule } from "primeng/rating";
import { RadioButtonModule } from "primeng/radiobutton";
import { IconFieldModule } from "primeng/iconfield";
import { InputIconModule } from "primeng/inputicon";
import { TextareaModule } from "primeng/textarea";
import { FormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

@Component({
  selector: "app-crud-demo",
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ToastModule,
    ConfirmDialogModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    SelectModule,
    TagModule,
    ToolbarModule,
    FileUploadModule,
    RatingModule,
    RadioButtonModule,
    IconFieldModule,
    InputIconModule,
    TextareaModule,
  ],
  templateUrl: "./crud.component.html",
  providers: [ProductService, ConfirmationService, MessageService],
})
export class CrudDemo implements OnInit {
  productDialog: boolean = false;

  deleteProductDialog: boolean = false;

  deleteProductsDialog: boolean = false;

  products: Product[] = [];

  product: Product = {};

  selectedProducts: Product[] = [];

  submitted: boolean = false;

  cols: any[] = [];

  statuses: any[] = [];

  rowsPerPageOptions = [5, 10, 20];

  loading: boolean = false;

  constructor(
    private productService: ProductService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit() {
    this.productService.getProducts().then(data => (this.products = data));

    this.statuses = [
      { label: "INSTOCK", value: "instock" },
      { label: "LOWSTOCK", value: "lowstock" },
      { label: "OUTOFSTOCK", value: "outofstock" },
    ];

    this.cols = [
      { field: "id", header: "ID" },
      { field: "name", header: "Name" },
      { field: "image", header: "Image" },
      { field: "price", header: "Price" },
      { field: "category", header: "Category" },
      { field: "rating", header: "Reviews" },
      { field: "inventoryStatus", header: "Status" },
    ];
  }

  openNew() {
    this.product = {};
    this.submitted = false;
    this.productDialog = true;
  }

  deleteSelectedProducts() {
    this.deleteProductsDialog = true;
  }

  editProduct(product: Product) {
    this.product = { ...product };
    this.productDialog = true;
  }

  deleteProduct(product: Product) {
    this.deleteProductDialog = true;
    this.product = { ...product };
  }

  confirmDeleteSelected() {
    this.deleteProductsDialog = false;
    this.products = this.products.filter(val => !this.selectedProducts.includes(val));
    this.messageService.add({ severity: "success", summary: "Successful", detail: "Products Deleted", life: 3000 });
    this.selectedProducts = [];
  }

  confirmDelete() {
    this.deleteProductDialog = false;
    this.products = this.products.filter(val => val.id !== this.product.id);
    this.messageService.add({ severity: "success", summary: "Successful", detail: "Product Deleted", life: 3000 });
    this.product = {};
  }

  hideDialog() {
    this.productDialog = false;
    this.submitted = false;
  }

  saveProduct() {
    this.submitted = true;

    if (this.product.name?.trim()) {
      if (this.product.id) {
        this.products[this.findIndexById(this.product.id)] = this.product;
        this.messageService.add({ severity: "success", summary: "Successful", detail: "Product Updated", life: 3000 });
      } else {
        this.product.id = this.createId();
        this.product.image = "product-placeholder.svg";
        this.products.push(this.product);
        this.messageService.add({ severity: "success", summary: "Successful", detail: "Product Created", life: 3000 });
      }

      this.products = [...this.products];
      this.productDialog = false;
      this.product = {};
    }
  }

  findIndexById(id: string): number {
    let index = -1;
    for (let i = 0; i < this.products.length; i++) {
      if (this.products[i].id === id) {
        index = i;
        break;
      }
    }

    return index;
  }

  createId(): string {
    let id = "";
    const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    for (let i = 0; i < 5; i++) {
      id += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return id;
  }

  onGlobalFilter(table: Table, event: Event) {
    table.filterGlobal((event.target as HTMLInputElement).value, "contains");
  }

  getSeverity(product: Product) {
    switch (product.inventoryStatus) {
      case "INSTOCK":
        return "success";
      case "LOWSTOCK":
        return "warn";
      case "OUTOFSTOCK":
        return "danger";
      default:
        return "info";
    }
  }

  exportExcel() {
    const csvContent = this.convertToCSV(this.products);
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    const link = document.createElement("a");
    link.href = URL.createObjectURL(blob);
    link.download = "products_export_" + new Date().getTime() + ".csv";
    link.click();
  }

  convertToCSV(data: any[]): string {
    const header = Object.keys(data[0]).join(",");
    const rows = data.map(row =>
      Object.values(row)
        .map(value => (typeof value === "string" ? `"${value}"` : value))
        .join(",")
    );
    return [header, ...rows].join("\n");
  }

  onImport(event: any) {
    const files = event.files;
    if (files && files.length > 0) {
      for (let file of files) {
        console.log("Imported file name:", file.name);
        console.log("File size:", file.size, "bytes");
        console.log("File type:", file.type);
      }
      this.messageService.add({
        severity: "success",
        summary: "Success",
        detail: "File(s) imported successfully",
        life: 3000,
      });
    }
  }

  onRowSelect(event: any) {
    console.log("Row selected:", event.data);
  }

  onRowUnselect(event: any) {
    console.log("Row unselected:", event.data);
  }
}
