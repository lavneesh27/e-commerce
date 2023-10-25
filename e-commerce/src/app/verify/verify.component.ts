import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NavigationService } from '../services/navigation.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-verify',
  templateUrl: './verify.component.html',
  styleUrls: ['./verify.component.css'],
})
export class VerifyComponent {
  constructor(private fb: FormBuilder, private navigationService: NavigationService,private route: ActivatedRoute, private toastr: ToastrService,) {}
  verifyForm!: FormGroup;
  email:string='';
  isVerified: boolean = false;

  ngOnInit() {
    this.verifyForm = this.fb.group({
      otp: ['', Validators.required, Validators.minLength(6)],
    });
    this.route.queryParams.subscribe(params => {
      this.email = params['email'].toString();
    });

    this.navigationService.getOTP(this.email).subscribe({
      next: (res: any) => {
        console.log(res);
        if(res==true){
          this.toastr.success('OTP Sent!');
        }
      },
      error: (err) => {
        console.error(err); 
      },
    });
  }

  verify(){
    const otp = this.verifyForm.get('otp')?.value.toString();
    this.navigationService.verifyOTP(otp,this.email).subscribe({
      next: (res: any) => {
        console.log(res); 
        if(res==true){
          this.toastr.success('Email Verified!');
        }
      },
      error: (err) => {
        console.error(err);
      },
    });
  }

  resend(){
    this.navigationService.getOTP(this.email).subscribe({
      next: (res: any) => {
        console.log(res);
        if(res==true){
          this.toastr.success('OTP Sent!');
        }
      },
      error: (err) => {
        console.error(err); 
      },
    });
  }
}
