import { Component, inject, OnInit } from "@angular/core";
import { TreeNode } from "primeng/api";
import { TreeModule } from "primeng/tree";
import { FormsModule } from "@angular/forms";
import { TreeTableModule } from "primeng/treetable";
import { CommonModule } from "@angular/common";
import { NodeService } from "../service/node.service";
import { Observable, from } from "rxjs";

@Component({
  selector: "app-tree-demo",
  standalone: true,
  imports: [CommonModule, FormsModule, TreeModule, TreeTableModule],
  templateUrl: "./tree.component.html",
  providers: [NodeService],
})
export class TreeDemo implements OnInit {
  treeValue$!: Observable<TreeNode[]>;
  treeTableValue$!: Observable<TreeNode[]>;

  selectedTreeValue: TreeNode[] = [];

  selectedTreeTableValue = {};

  cols: any[] = [];

  nodeService = inject(NodeService);

  ngOnInit() {
    this.treeValue$ = from(this.nodeService.getFiles());
    this.treeTableValue$ = from(this.nodeService.getTreeTableNodes());

    this.cols = [
      { field: "name", header: "Name" },
      { field: "size", header: "Size" },
      { field: "type", header: "Type" },
    ];

    this.selectedTreeTableValue = {
      "0-0": {
        partialChecked: false,
        checked: true,
      },
    };
  }
}
