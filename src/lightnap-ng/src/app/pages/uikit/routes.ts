import { Routes } from '@angular/router';
import { ButtonDemo } from './button/button.component';
import { ChartDemo } from './charts/charts.component';
import { FileDemo } from './file/file.component';
import { FormLayoutDemo } from './formlayout/formlayout.component';
import { InputDemo } from './input/input.component';
import { ListDemo } from './list/list.component';
import { MediaDemo } from './media/media.component';
import { MessagesDemo } from './message/message.component';
import { MiscDemo } from './misc/misc.component';
import { PanelsDemo } from './panel/panel.component';
import { TimelineDemo } from './timeline/timeline.component';
import { TableDemo } from './table/table.component';
import { CrudDemo } from './crud/crud.component';
import { OverlayDemo } from './overlay/overlay.component';
import { TreeDemo } from './tree/tree.component';
import { MenuDemo } from './menu/menu.component';

export default [
    { path: '', redirectTo: 'button', pathMatch: 'full' },
    { path: 'button', data: { breadcrumb: 'Button' }, component: ButtonDemo },
    { path: 'charts', data: { breadcrumb: 'Charts' }, component: ChartDemo },
    { path: 'file', data: { breadcrumb: 'File' }, component: FileDemo },
    { path: 'formlayout', data: { breadcrumb: 'Form Layout' }, component: FormLayoutDemo },
    { path: 'input', data: { breadcrumb: 'Input' }, component: InputDemo },
    { path: 'list', data: { breadcrumb: 'List' }, component: ListDemo },
    { path: 'media', data: { breadcrumb: 'Media' }, component: MediaDemo },
    { path: 'message', data: { breadcrumb: 'Message' }, component: MessagesDemo },
    { path: 'misc', data: { breadcrumb: 'Misc' }, component: MiscDemo },
    { path: 'panel', data: { breadcrumb: 'Panel' }, component: PanelsDemo },
    { path: 'timeline', data: { breadcrumb: 'Timeline' }, component: TimelineDemo },
    { path: 'table', data: { breadcrumb: 'Table' }, component: TableDemo },
    { path: 'crud', data: { breadcrumb: 'CRUD' }, component: CrudDemo },
    { path: 'overlay', data: { breadcrumb: 'Overlay' }, component: OverlayDemo },
    { path: 'tree', data: { breadcrumb: 'Tree' }, component: TreeDemo },
    { path: 'menu', data: { breadcrumb: 'Menu' }, component: MenuDemo },
    { path: '**', redirectTo: '/notfound' }
] as Routes;