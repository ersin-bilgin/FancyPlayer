import { Component } from "@angular/core";
import { InputTextModule } from "primeng/inputtext";
import { FluidModule } from "primeng/fluid";
import { ButtonModule } from "primeng/button";
import { SelectModule } from "primeng/select";
import { FormsModule } from "@angular/forms";
import { TextareaModule } from "primeng/textarea";

@Component({
  selector: "app-formlayout-demo",
  standalone: true,
  imports: [InputTextModule, FluidModule, ButtonModule, SelectModule, FormsModule, TextareaModule],
  templateUrl: "./formlayout.component.html",
})
export class FormLayoutDemo {
  dropdownItems = [
    { name: "New York", code: "NY" },
    { name: "Rome", code: "RM" },
    { name: "London", code: "LDN" },
    { name: "Istanbul", code: "IST" },
    { name: "Paris", code: "PRS" },
  ];

  dropdownItem = null;
}
