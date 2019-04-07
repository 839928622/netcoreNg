import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/User';

@Component({
  selector: 'app-member-card',
  templateUrl: './Member-card.component.html',
  styleUrls: ['./Member-card.component.css']
})
export class MemberCardComponent implements OnInit {
@Input() user: User; // 我们想要从父组件上获取
  constructor() { }

  ngOnInit() {
  }

}
