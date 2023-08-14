import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { UtilityService } from '../services/utility.service';
import { NavigationService } from '../services/navigation.service';
import { Cart, Order, Payment, Product } from '../models/models';
import { timer } from 'rxjs';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

declare var Razorpay: any;

@Component({
  selector: 'app-order',
  templateUrl: './order.component.html',
  styleUrls: ['./order.component.css'],
})
export class OrderComponent implements OnInit {
  selectedPaymentMethodName = '';
  selectedPaymentMethod = new FormControl('0');
  address = '';
  mobileNumber = '';
  displaySpinner = false;
  message = '';
  className = '';

  usersCart: Cart = {
    id: 0,
    user: this.utilityService.getUser(),
    cartItems: [],
    ordered: false,
    orderedOn: '',
  };
  usersPaymentInfo: Payment = {
    id: 0,
    user: this.utilityService.getUser(),
    totalAmount: 0,
    shippingCharges: 0,
    amountReduced: 0,
    amountPaid: 0,
    createdAt: '',
  };
  constructor(
    public utilityService: UtilityService,
    private navigationService: NavigationService,
    private router: Router,
    private toastr: ToastrService
  ) {}
  ngOnInit(): void {
    this.selectedPaymentMethod.valueChanges.subscribe((res: any) => {
      if (res === '0') this.selectedPaymentMethodName = '';
      else {
        this.selectedPaymentMethodName = res.toString();
      }
    });

    this.navigationService
      .getActiveCartOfUser(this.utilityService.getUser().id)
      .subscribe((res: any) => {
        this.usersCart = res;
        this.utilityService.calculatePayment(res, this.usersPaymentInfo);
      });

    this.address = this.utilityService.getUser().address;
    this.mobileNumber = this.utilityService.getUser().mobile;
  }
  placeOrder() {
    const RazorpayOptions = {
      description: 'Sample Razorpay demo',
      currency: 'INR',
      amount: this.usersPaymentInfo.amountPaid * 100,
      name:
        this.utilityService.getUser().firstName +
        ' ' +
        this.utilityService.getUser().lastName,
      key: 'rzp_test_PpV8ZNgcL7WlXH',
      image:
        'https://www.ecommerce-nation.com/wp-content/uploads/2019/02/razorpay.webp',
      prefill: {
        name:
          this.utilityService.getUser().firstName +
          this.utilityService.getUser().lastName,
        email: this.utilityService.getUser().email,
        phone: this.utilityService.getUser().mobile,
      },
      handler: (res: any) => {
        successCallback(res.razorpay_payment_id);
      },
      theme: {
        color: '#6466e3',
      },
      modal: {
        ondismiss: () => {
          console.log('dismissed');
        },
      },
    };

    const successCallback = (paymentid: any) => {
      this.usersCart.cartItems.forEach((item: any) => {
        this.navigationService
          .updateProduct(item.product.id)
          .subscribe((res) => {
            console.log(res);
          });
      });
      this.message = 'Your Order has been placed';
      this.toastr.success('Your Order has been placed !');
      this.storeOrder();

      setTimeout(() => {
        this.router.navigateByUrl('/home');
      }, 1000);
    };

    const failureCallback = (e: any) => {
      console.log(e);
      console.log('error occured');
    };

    const updateItems = () => {
      this.usersCart.cartItems.forEach((element) => {
        element.product.quantity--;
        console.log(element.product.quantity);
      });
    };

    Razorpay.open(RazorpayOptions, successCallback, failureCallback);
  }

  storeOrder() {
    let payment: Payment;
    let pmid = 0;
    if (this.selectedPaymentMethod.value)
      pmid = parseInt(this.selectedPaymentMethod.value);

    payment = {
      id: 0,
      user: this.utilityService.getUser(),
      totalAmount: this.usersPaymentInfo.totalAmount,
      shippingCharges: this.usersPaymentInfo.shippingCharges,
      amountReduced: this.usersPaymentInfo.amountReduced,
      amountPaid: this.usersPaymentInfo.amountPaid,
      createdAt: '',
    };
    this.navigationService
      .insertPayment(payment)
      .subscribe((paymentResponse: any) => {
        payment.id = parseInt(paymentResponse);
        let order: Order = {
          id: 0,
          user: this.utilityService.getUser(),
          cart: this.usersCart,
          payment: payment,
          createdAt: '',
        };
        this.navigationService.insertOrder(order).subscribe((orderResponse) => {
          this.utilityService.changeCart.next(0);
        });
      });
  }
}
