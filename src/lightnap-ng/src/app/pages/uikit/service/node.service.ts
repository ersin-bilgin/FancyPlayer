import { Injectable } from "@angular/core";
import { TreeNode } from "primeng/api";

@Injectable({
  providedIn: "root",
})
export class NodeService {
  getFiles(): Promise<TreeNode[]> {
    return Promise.resolve([
      {
        key: "0",
        label: "Documents",
        data: "Documents Folder",
        icon: "pi pi-fw pi-inbox",
        children: [
          {
            key: "0-0",
            label: "Work",
            data: "Work Folder",
            icon: "pi pi-fw pi-cog",
            children: [
              { key: "0-0-0", label: "Expenses.doc", icon: "pi pi-fw pi-file", data: "Expenses Document" },
              { key: "0-0-1", label: "Resume.doc", icon: "pi pi-fw pi-file", data: "Resume Document" },
            ],
          },
          {
            key: "0-1",
            label: "Home",
            data: "Home Folder",
            icon: "pi pi-fw pi-home",
            children: [{ key: "0-1-0", label: "Invoices.txt", icon: "pi pi-fw pi-file", data: "Invoices for this month" }],
          },
        ],
      },
      {
        key: "1",
        label: "Events",
        data: "Events Folder",
        icon: "pi pi-fw pi-calendar",
        children: [
          { key: "1-0", label: "Meeting", icon: "pi pi-fw pi-calendar-plus", data: "Meeting" },
          { key: "1-1", label: "Product Launch", icon: "pi pi-fw pi-calendar-plus", data: "Product Launch" },
          { key: "1-2", label: "Report Review", icon: "pi pi-fw pi-calendar-plus", data: "Report Review" },
        ],
      },
      {
        key: "2",
        label: "Movies",
        data: "Movies Folder",
        icon: "pi pi-fw pi-star-fill",
        children: [
          {
            key: "2-0",
            icon: "pi pi-fw pi-star-fill",
            label: "Al Pacino",
            data: "Pacino Movies",
            children: [
              { key: "2-0-0", label: "Scarface", icon: "pi pi-fw pi-video", data: "Scarface Movie" },
              { key: "2-0-1", label: "Serpico", icon: "pi pi-fw pi-video", data: "Serpico Movie" },
            ],
          },
          {
            key: "2-1",
            label: "Robert De Niro",
            icon: "pi pi-fw pi-star-fill",
            data: "De Niro Movies",
            children: [
              { key: "2-1-0", label: "Goodfellas", icon: "pi pi-fw pi-video", data: "Goodfellas Movie" },
              { key: "2-1-1", label: "Untouchables", icon: "pi pi-fw pi-video", data: "Untouchables Movie" },
            ],
          },
        ],
      },
    ]);
  }

  getTreeTableNodes(): Promise<TreeNode[]> {
    return Promise.resolve([
      {
        key: "0",
        data: {
          name: "Applications",
          size: "100kb",
          type: "Folder",
        },
        children: [
          {
            key: "0-0",
            data: {
              name: "Angular",
              size: "25kb",
              type: "Folder",
            },
            children: [
              {
                key: "0-0-0",
                data: {
                  name: "angular.app",
                  size: "10kb",
                  type: "Application",
                },
              },
              {
                key: "0-0-1",
                data: {
                  name: "cli.app",
                  size: "10kb",
                  type: "Application",
                },
              },
              {
                key: "0-0-2",
                data: {
                  name: "mobile.app",
                  size: "5kb",
                  type: "Application",
                },
              },
            ],
          },
          {
            key: "0-1",
            data: {
              name: "React",
              size: "50kb",
              type: "Folder",
            },
            children: [
              {
                key: "0-1-0",
                data: {
                  name: "react.app",
                  size: "25kb",
                  type: "Application",
                },
              },
              {
                key: "0-1-1",
                data: {
                  name: "native.app",
                  size: "25kb",
                  type: "Application",
                },
              },
            ],
          },
          {
            key: "0-2",
            data: {
              name: "Vue",
              size: "25kb",
              type: "Folder",
            },
            children: [
              {
                key: "0-2-0",
                data: {
                  name: "vue.app",
                  size: "25kb",
                  type: "Application",
                },
              },
            ],
          },
        ],
      },
      {
        key: "1",
        data: {
          name: "Cloud",
          size: "20kb",
          type: "Folder",
        },
        children: [
          {
            key: "1-0",
            data: {
              name: "Documents",
              size: "10kb",
              type: "Folder",
            },
            children: [
              {
                key: "1-0-0",
                data: {
                  name: "work.doc",
                  size: "10kb",
                  type: "Document",
                },
              },
            ],
          },
          {
            key: "1-1",
            data: {
              name: "Pictures",
              size: "10kb",
              type: "Folder",
            },
            children: [
              {
                key: "1-1-0",
                data: {
                  name: "barcelona.jpg",
                  size: "10kb",
                  type: "Picture",
                },
              },
            ],
          },
        ],
      },
    ]);
  }
}
