import { inject, Injectable } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { RouteAliasService } from "@core";
import { MenuItem } from "primeng/api";
import { BehaviorSubject, combineLatest, debounceTime, Subject, tap } from "rxjs";
import { IdentityService } from "@core/services/identity.service";
import { MenuChangeEvent } from "../models/menu-change-event";

@Injectable({
  providedIn: "root",
})
export class MenuService {
  #routeAlias = inject(RouteAliasService);
  #identityService = inject(IdentityService);

  #menuSource = new Subject<MenuChangeEvent>();
  menuSource$ = this.#menuSource.asObservable();

  #defaultMenuItems = new Array<MenuItem>({
    label: "Home",
    items: [{ label: "Home", icon: "pi pi-fw pi-home", routerLink: this.#routeAlias.getRoute("user-home") }],
  }, {
    label: "UI Components",
    items: [
      { label: "Form Layout", icon: "pi pi-fw pi-id-card", routerLink: ["/uikit", "formlayout"] },
      { label: "Input", icon: "pi pi-fw pi-check-square", routerLink: ["/uikit", "input"] },
      { label: "Button", icon: "pi pi-fw pi-mobile", class: "rotated-icon", routerLink: ["/uikit", "button"] },
      { label: "Table", icon: "pi pi-fw pi-table", routerLink: ["/uikit", "table"] },
      { label: "CRUD", icon: "pi pi-fw pi-database", routerLink: ["/uikit", "crud"] },
      { label: "List", icon: "pi pi-fw pi-list", routerLink: ["/uikit", "list"] },
      { label: "Tree", icon: "pi pi-fw pi-share-alt", routerLink: ["/uikit", "tree"] },
      { label: "Panel", icon: "pi pi-fw pi-tablet", routerLink: ["/uikit", "panel"] },
      { label: "Overlay", icon: "pi pi-fw pi-clone", routerLink: ["/uikit", "overlay"] },
      { label: "Media", icon: "pi pi-fw pi-image", routerLink: ["/uikit", "media"] },
      { label: "Menu", icon: "pi pi-fw pi-bars", routerLink: ["/uikit", "menu"] },
      { label: "Message", icon: "pi pi-fw pi-comment", routerLink: ["/uikit", "message"] },
      { label: "File", icon: "pi pi-fw pi-file", routerLink: ["/uikit", "file"] },
      { label: "Chart", icon: "pi pi-fw pi-chart-bar", routerLink: ["/uikit", "charts"] },
      { label: "Timeline", icon: "pi pi-fw pi-calendar", routerLink: ["/uikit", "timeline"] },
      { label: "Misc", icon: "pi pi-fw pi-circle", routerLink: ["/uikit", "misc"] }
    ]
  });

  #loggedInMenuItems = new Array<MenuItem>({
    label: "Profile",
    items: [
      { label: "Profile", icon: "pi pi-fw pi-user", routerLink: this.#routeAlias.getRoute("profile") },
      { label: "Devices", icon: "pi pi-fw pi-mobile", routerLink: this.#routeAlias.getRoute("devices") },
      { label: "Change Password", icon: "pi pi-fw pi-lock", routerLink: this.#routeAlias.getRoute("change-password") },
    ],
  });

  #adminMenuItems = new Array<MenuItem>({
    label: "Admin",
    items: [
      { label: "Home", icon: "pi pi-fw pi-home", routerLink: this.#routeAlias.getRoute("admin-home") },
      { label: "Users", icon: "pi pi-fw pi-users", routerLink: this.#routeAlias.getRoute("admin-users") },
      { label: "Roles", icon: "pi pi-fw pi-lock", routerLink: this.#routeAlias.getRoute("admin-roles") },
      { label: "Claims", icon: "pi pi-fw pi-shield", routerLink: this.#routeAlias.getRoute("admin-claims") },
    ],
  });

  #menuItemSubject = new BehaviorSubject<Array<MenuItem>>(this.#defaultMenuItems);

  #isLoggedIn = false;
  #isAdminLoggedIn = false;

  constructor() {
    combineLatest([
      this.#identityService.watchLoggedIn$().pipe(tap(isLoggedIn => (this.#isLoggedIn = isLoggedIn))),
      this.#identityService.watchUserRole$("Administrator").pipe(tap(isAdminLoggedIn => (this.#isAdminLoggedIn = isAdminLoggedIn))),
    ])
      .pipe(takeUntilDestroyed(), debounceTime(100))
      .subscribe(() => this.#refreshMenuItems());
  }

  onMenuStateChange(event: MenuChangeEvent) {
    this.#menuSource.next(event);
  }

  #refreshMenuItems() {
    var menuItems = [...this.#defaultMenuItems];

    if (this.#isLoggedIn) {
      menuItems.push(...this.#loggedInMenuItems);
    }

    if (this.#isAdminLoggedIn) {
      menuItems.push(...this.#adminMenuItems);
    }

    this.#menuItemSubject.next(menuItems);
  }

  watchMenuItems$() {
    return this.#menuItemSubject.asObservable();
  }
}
