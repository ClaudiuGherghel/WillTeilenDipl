import { Routes } from '@angular/router';
import { Categories } from './components/guest/categories/categories';
import { Items } from './components/guest/items/items';
import { ItemDetail } from './components/guest/item-detail/item-detail';
import { Login } from './components/user/login/login';
import { Register } from './components/user/register/register';
import { adminGuard } from './guards/admin-guard';
import { authGuard } from './guards/auth-guard';
import { User } from './components/user/user/user';
import { AddItem } from './components/user/user/add-item/add-item';

export const routes: Routes = [
    { path: '', redirectTo: 'categories', pathMatch: 'full' },
    { path: 'categories', component: Categories },
    { path: 'items/:subCategoryId', component: Items },
    { path: 'item-search', component: Items },
    { path: 'item/:itemId', component: ItemDetail },
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    {
        path: 'user', component: User, canActivate: [authGuard],   //  nur eingeloggte User
        children: [
            { path: 'addItem', component: AddItem },
            //   { path: 'items', component: UserItemsComponent },
            //   { path: 'profile', component: UserProfileComponent },
        ],
    },

    // {
    //     path: 'admin', component: AdminDashboard, canActivate: [adminGuard]   //  nur Admins
    // }
];
