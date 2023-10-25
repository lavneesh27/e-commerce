import { Component, OnInit } from '@angular/core';
import { UtilityService } from '../services/utility.service';
import { User } from '../models/models';
import { NavigationService } from '../services/navigation.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-edit-account',
  templateUrl: './edit-account.component.html',
  styleUrls: ['./edit-account.component.css'],
})
export class EditAccountComponent implements OnInit {
  constructor(
    public utility: UtilityService,
    public navigation: NavigationService,
    private toastr: ToastrService
  ) {}
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
    isVerified:false,
    createdAt: '',
    modifiedAt: '',
  };
  updateUser(): void {
    this.navigation.updateUser(this.user).subscribe((res) => {
      if (res) {
        this.navigation
          .loginUser(this.user.email, this.user.password)
          .subscribe((loginRes) => {
            this.utility.setUser(loginRes.toString());
            this.toastr.success('User Updated!');
          });
      } else {
        this.toastr.warning('Some Error Occuured!');
      }
    });
  }
}
