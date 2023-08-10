import { Component, OnInit } from '@angular/core';
import { UtilityService } from '../services/utility.service';
import { User } from '../models/models';
import { NavigationService } from '../services/navigation.service';

@Component({
  selector: 'app-edit-account',
  templateUrl: './edit-account.component.html',
  styleUrls: ['./edit-account.component.css'],
})
export class EditAccountComponent implements OnInit {
  constructor(
    public utility: UtilityService,
    public navigation: NavigationService
  ) {}
  ngOnInit() {
    this.user = this.utility.getUser();
  }

  message: string = '';

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
  updateUser(): void {
    console.log(this.user);

    this.navigation.updateUser(this.user).subscribe((res) => {
      if (res) {
        this.message = 'User Updated Successfully';
        this.navigation
          .loginUser(this.user.email, this.user.password)
          .subscribe((loginRes) => {
            this.utility.setUser(loginRes.toString());
          });
      } else {
        this.message = 'Some Error Occuured';
      }
    });
  }
}
