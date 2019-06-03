import { Directive, Input, ViewContainerRef, TemplateRef, OnInit } from '@angular/core';
import { AuthService } from '_services/auth.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
@Input() appHasRole: string[];
isVisible = false ;
  constructor(
    private viewContainerRef: ViewContainerRef, private templaterdf: TemplateRef<any>,
    private authService: AuthService
    ) { } // ViewContainerRef:a container for two kinds of views


    ngOnInit() {
      const userRoles = this.authService.decodedToken.role as Array<string>;
      // 如果没有角色，则清除viewCotainerRef
      if (!userRoles) {
        this.viewContainerRef.clear(); // 清除我们把appHasRole放置在某个元素或者组件上，则这个组件或者元素被清除
      }
      // 如果用户有特别的角色，则作用以下规则
      if (this.authService.roleMatch(this.appHasRole)) {
        if (!this.isVisible) {
          this.isVisible = true;
          this.viewContainerRef.createEmbeddedView(this.templaterdf);
        } else {
        this.isVisible = false;
        this.viewContainerRef.clear();
        }
      }
    }
}
