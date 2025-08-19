import { Routes } from '@angular/router';
import { Categories } from './components/guest/categories/categories';
import { Items } from './components/guest/items/items';
import { ItemDetail } from './components/guest/item-detail/item-detail';

export const routes: Routes = [
    { path: '', redirectTo: 'categories', pathMatch: 'full' },
    { path: 'categories', component: Categories },
    { path: 'items/:subCategoryId', component: Items },
    { path: 'item-search', component: Items },
    { path: 'item/:itemId', component: ItemDetail },

];
