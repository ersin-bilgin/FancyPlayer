import { Injectable } from "@angular/core";
import { BlockUiParams } from "@core/models";
import { Subject } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class BlockUiService {
  #showEmitter = new Subject<BlockUiParams>();
  #hideEmitter = new Subject<void>();

  onShow$ = this.#showEmitter.asObservable();
  onHide$ = this.#hideEmitter.asObservable();

  show(blockUiParams: BlockUiParams) {
    this.#showEmitter.next(blockUiParams);
  }

  hide() {
    this.#hideEmitter.next();
  }
}
