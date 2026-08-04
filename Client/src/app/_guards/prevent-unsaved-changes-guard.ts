import { CanDeactivateFn } from '@angular/router';
import { MemberEdit } from '../members/member-edit/member-edit';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEdit> = (component) => {
  if (component.updateForm?.dirty) {
    return confirm('Are you sure you want to continue? Any unsaved changes will be lost.')
  }

  return true;
};
