import { CanDeactivateFn } from '@angular/router';
import { MemberEdit } from '../members/member-edit/member-edit';
import { inject } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEdit> = (component) => {
  const confirmService = inject(ConfirmService);
  if (component.updateForm?.dirty) {
    return confirmService.confirm() ?? false;
  }else{
    return true;
  }
};
