import { Component } from '@angular/core';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { UserManagement } from '../user-management/user-management';
import { PhotoManagement } from '../photo-management/photo-management';
import { HasRoleDirective } from "../../_directives/has-role.directive";

@Component({
  selector: 'app-admin-panel',
  imports: [TabsModule, UserManagement, PhotoManagement, HasRoleDirective],
  templateUrl: './admin-panel.html',
  styleUrl: './admin-panel.css',
})
export class AdminPanel {}
