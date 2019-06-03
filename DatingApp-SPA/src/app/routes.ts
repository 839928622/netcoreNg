
import { AuthGuard } from './_guards/auth.guard';
import { Routes } from '@angular/router';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './Members/member-list/member-list.component';
import { HomeComponent } from './home/home.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './Members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberEditComponent } from './Members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { ListsResolver } from './_resolvers/lists.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent}, // 什么也不输入 localhost：port，命中此路由
    {
        path: '', // 如果单引号里有路径，比如 index,那么会匹配为 localhost:port/index/我们的路由 在这里，我们的路由均在localhost这一级，因此保持空字符串
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent,   resolve: { users : MemberListResolver } },
            { path: 'members/:id', component: MemberDetailComponent, resolve: { user: MemberDetailResolver } },
            // tslint:disable-next-line:max-line-length
            {path: 'member/edit', component: MemberEditComponent, resolve: {user: MemberEditResolver}, canDeactivate: [PreventUnsavedChanges]}, // 从token中获取用户信息，因此无需id
            { path: 'messages', component: MessagesComponent , resolve: {messages: MessagesResolver}},
            { path: 'lists', component: ListsComponent, resolve: {users: ListsResolver} },

            { path: 'admin', component: AdminPanelComponent, data: {roles: ['Admin', 'Moderator']}},
        ]
    },
    { path: '**', redirectTo: 'home' , pathMatch: 'full'} // pathMatch: 'full'表示路径有全对/全匹配，不匹配则重定向至home组件 这里顺序是比较重要的，如果第一个不匹配，会一直检索下去，直到最后一个
];
