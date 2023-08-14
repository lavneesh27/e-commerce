import { Component, OnInit } from '@angular/core';
import { UtilityService } from '../services/utility.service';
import { User } from '../models/models';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css'],
})
export class AccountComponent implements OnInit {
  constructor(public utility: UtilityService) {}
  ngOnInit() {
    this.user = this.utility.getUser();
  }

  user: User = {
    id: 0,
    firstName: '',
    lastName: '',
    email: '',
    address: '',
    mobile: '',
    password: '',
    createdAt: '',
    modifiedAt: '',
  };
}
