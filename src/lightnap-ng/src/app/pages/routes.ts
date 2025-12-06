import { loggedInGuard } from "@core/guards/logged-in.guard";
import { roleGuard } from "@core/guards/role.guard";
import { AppLayoutComponent } from "@core/layout/components/layouts/app-layout/app-layout.component";
import { PublicLayoutComponent } from "@core/layout/components/layouts/public-layout/public-layout.component";
import { Routes as AdminRoutes } from "./admin/routes";
import { AppRoute } from "../core/routing/models/app-route";
import { Routes as IdentityRoutes } from "./identity/routes";
import { Routes as ProfileRoutes } from "./profile/routes";
import { Routes as PublicRoutes } from "./public/routes";
import { Routes as HomeRoutes } from "./home/routes";

export const Routes: AppRoute[] = [
  { path: "", component: PublicLayoutComponent, children: PublicRoutes },
  {
    path: "",
    component: AppLayoutComponent,
    children: [
      { path: "home", data: { breadcrumb: "Home" }, canActivate: [loggedInGuard], children: HomeRoutes },
      { path: "profile", data: { breadcrumb: "Profile" }, canActivate: [loggedInGuard], children: ProfileRoutes },
      {
        path: "uikit",
        data: { breadcrumb: "UI Kit" },
        canActivate: [loggedInGuard],
        children: [
          { path: "", redirectTo: "button", pathMatch: "full" },
          { path: "button", title: "Button", loadComponent: () => import("./uikit/button/button.component").then(m => m.ButtonDemo) },
          { path: "input", title: "Input", loadComponent: () => import("./uikit/input/input.component").then(m => m.InputDemo) },
          { path: "table", title: "Table", loadComponent: () => import("./uikit/table/table.component").then(m => m.TableDemo) },
          { path: "list", title: "List", loadComponent: () => import("./uikit/list/list.component").then(m => m.ListDemo) },
          { path: "crud", title: "CRUD", loadComponent: () => import("./uikit/crud/crud.component").then(m => m.CrudDemo) },
          { path: "media", title: "Media", loadComponent: () => import("./uikit/media/media.component").then(m => m.MediaDemo) },
          { path: "overlay", title: "Overlay", loadComponent: () => import("./uikit/overlay/overlay.component").then(m => m.OverlayDemo) },
          { path: "tree", title: "Tree", loadComponent: () => import("./uikit/tree/tree.component").then(m => m.TreeDemo) },
          { path: "menu", title: "Menu", loadComponent: () => import("./uikit/menu/menu.component").then(m => m.MenuDemo) },
          { path: "charts", title: "Charts", loadComponent: () => import("./uikit/charts/charts.component").then(m => m.ChartDemo) },
          { path: "file", title: "File", loadComponent: () => import("./uikit/file/file.component").then(m => m.FileDemo) },
          {
            path: "formlayout",
            title: "Form Layout",
            loadComponent: () => import("./uikit/formlayout/formlayout.component").then(m => m.FormLayoutDemo),
          },
          { path: "panel", title: "Panel", loadComponent: () => import("./uikit/panel/panel.component").then(m => m.PanelsDemo) },
          {
            path: "timeline",
            title: "Timeline",
            loadComponent: () => import("./uikit/timeline/timeline.component").then(m => m.TimelineDemo),
          },
          { path: "message", title: "Message", loadComponent: () => import("./uikit/message/message.component").then(m => m.MessagesDemo) },
          { path: "misc", title: "Misc", loadComponent: () => import("./uikit/misc/misc.component").then(m => m.MiscDemo) },
        ],
      },
    ],
  },
  {
    path: "admin",
    component: AppLayoutComponent,
    canActivate: [loggedInGuard, roleGuard("Administrator")],
    children: [{ path: "", data: { breadcrumb: "Admin" }, children: AdminRoutes }],
  },
  { path: "identity", data: { breadcrumb: "Identity" }, children: IdentityRoutes },
  { path: "**", redirectTo: "/not-found" },
];
