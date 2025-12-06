import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { timer, map, scan, takeWhile, startWith, Observable } from "rxjs";
import { AvatarModule } from "primeng/avatar";
import { AvatarGroupModule } from "primeng/avatargroup";
import { BadgeModule } from "primeng/badge";
import { ButtonModule } from "primeng/button";
import { ChipModule } from "primeng/chip";
import { OverlayBadgeModule } from "primeng/overlaybadge";
import { ProgressBarModule } from "primeng/progressbar";
import { ScrollPanelModule } from "primeng/scrollpanel";
import { ScrollTopModule } from "primeng/scrolltop";
import { SkeletonModule } from "primeng/skeleton";
import { TagModule } from "primeng/tag";

@Component({
  selector: "app-misc-demo",
  standalone: true,
  imports: [
    CommonModule,
    ProgressBarModule,
    BadgeModule,
    AvatarModule,
    ScrollPanelModule,
    TagModule,
    ChipModule,
    ButtonModule,
    SkeletonModule,
    AvatarGroupModule,
    ScrollTopModule,
    OverlayBadgeModule,
  ],
  templateUrl: "./misc.component.html",
})
export class MiscDemo implements OnInit {
  value$!: Observable<number>;

  ngOnInit() {
    this.value$ = timer(0, 2000).pipe(
      map(() => Math.floor(Math.random() * 10) + 1),
      scan((acc, current) => acc + current, 0),
      map(value => Math.min(value, 100)),
      takeWhile(value => value < 100, true)
    );
  }
}
