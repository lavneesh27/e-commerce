import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { User } from '../models/models';
import { NavigationService } from '../services/navigation.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  registerForm!: FormGroup;
  submitted: boolean = false;
  constructor(
    private fb: FormBuilder,
    private navigationService: NavigationService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.registerForm = this.fb.group(
      {
        firstName: [
          '',
          [
            Validators.required,
            Validators.minLength(2),
            Validators.pattern('[a-zA-Z].*'),
          ],
        ],
        lastName: [
          '',
          [
            Validators.required,
            Validators.minLength(2),
            Validators.pattern('[a-zA-Z].*'),
          ],
        ],
        email: ['', [Validators.required, Validators.email]],
        address: ['', [Validators.required]],
        mobile: ['', [Validators.required, Validators.minLength(10)]],
        pwd: [
          '',
          [
            Validators.required,
            Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$'),
          ],
        ],
        rpwd: ['', Validators.required],
      },
      {
        validators: this.passwordMatchValidator,
      }
    );
  }
  register() {
    this.submitted = true;
    if (this.registerForm.valid) {
      let user: User = {
        id: 0,
        firstName: this.FirstName.value,
        lastName: this.LastName.value,
        email: this.Email.value,
        address: this.Address.value,
        mobile: this.Mobile.value,
        password: this.PWD.value,
        createdAt: '',
        modifiedAt: '',
      };
      this.navigationService.registerUser(user).subscribe((res: any) => {
        this.toastr.success('Registration Successful!');
      });
    }
  }
  passwordMatchValidator(formGroup: FormGroup) {
    const passwordControl = formGroup.get('pwd');
    const repeatPasswordControl = formGroup.get('rpwd');

    if (passwordControl?.value !== repeatPasswordControl?.value) {
      repeatPasswordControl?.setErrors({ passwordMismatch: true });
    } else {
      repeatPasswordControl?.setErrors(null);
    }
  }
  get FirstName(): FormControl {
    return this.registerForm.get('firstName') as FormControl;
  }
  get LastName(): FormControl {
    return this.registerForm.get('lastName') as FormControl;
  }
  get Email(): FormControl {
    return this.registerForm.get('email') as FormControl;
  }
  get Address(): FormControl {
    return this.registerForm.get('address') as FormControl;
  }
  get Mobile(): FormControl {
    return this.registerForm.get('mobile') as FormControl;
  }
  get PWD(): FormControl {
    return this.registerForm.get('pwd') as FormControl;
  }
  get RPWD(): FormControl {
    return this.registerForm.get('rpwd') as FormControl;
  }
}
