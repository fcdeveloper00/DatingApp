import { Routes } from '@angular/router';
import { Register } from './register/register';
import { Home } from './home/home';
import { MemberList } from './members/member-list/member-list';
import { MemberDetail } from './members/member-detail/member-detail';
import { Lists } from './lists/lists';
import { Messages } from './messages/messages';
import { authGuard } from './_guards/auth-guard';

export const routes: Routes = [
  { path: '',  component: Home,},
  {
    path:'',
    runGuardsAndResolvers:'always',
    canActivate:[authGuard],
    children:[
      { path: 'members', component: MemberList },
      { path: 'members/:id', component: MemberDetail },
      { path: 'lists', component: Lists },
      { path: 'messages', component: Messages },
    ]
      
  },
  
  { path:'**',component:Home, pathMatch:'full'}
];
