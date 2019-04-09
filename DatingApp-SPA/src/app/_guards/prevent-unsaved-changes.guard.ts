import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../Members/member-edit/member-edit.component';


@Injectable()
export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent> {
    canDeactivate(Component: MemberEditComponent) {
        if (Component.editForm.dirty) {
            return confirm('您的个人资料还没保存，确定要离开吗？');
        }

        return true;
    }
}
