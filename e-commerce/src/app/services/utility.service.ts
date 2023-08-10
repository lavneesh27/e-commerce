import {
  Injectable,
  OnDestroy,
  createEnvironmentInjector,
} from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Cart, Payment, Product, User } from '../models/models';
import { Subject } from 'rxjs';
import { NavigationService } from './navigation.service';

@Injectable({
  providedIn: 'root',
})
export class UtilityService implements OnDestroy {
  changeCart = new Subject();

  constructor(
    private jwt: JwtHelperService,
    private navigation: NavigationService
  ) {}
  applyDiscount(price: number, discount: number): number {
    let finalPrice: number = price - price * (discount / 100);
    return finalPrice;
  }

  getUser(): User {
    let token = this.jwt.decodeToken();

    let user: User = {
      id: token.id,
      firstName: token.firstName,
      lastName: token.lastName,
      address: token.address,
      mobile: token.mobile,
      email: token.email,
      password: token.password,
      createdAt: token.createdAt,
      modifiedAt: token.modifiedAt,
    };

    console.log(user);

    return user;
  }
  setUser(token: string) {
    localStorage.setItem('user', token);
  }
  isLoggedIn() {
    return localStorage.getItem('user') ? true : false;
  }
  logoutUser() {
    localStorage.removeItem('user');
  }

  addToCart(product: Product) {
    let productId = product.id;
    let userId = this.getUser().id;

    this.navigation.addToCart(userId, productId).subscribe((res) => {
      if (res.toString() === 'inserted') this.changeCart.next(1);
    });
  }
  removeFromCart(product: Product) {
    let productId = product.id;
    let userId = this.getUser().id;

    this.navigation.removeFromCart(userId, productId).subscribe((res) => {
      if (res.toString() === 'removed') {
        this.changeCart.next(-1);
        window.location.reload();
        this.navigation.getActiveCartOfUser(userId);
      }
    });
  }

  calculatePayment(cart: Cart, payment: Payment) {
    payment.totalAmount = 0;
    payment.amountPaid = 0;
    payment.amountReduced = 0;

    for (let cartitem of cart.cartItems) {
      payment.totalAmount += cartitem.product.price;

      payment.amountReduced +=
        cartitem.product.price -
        this.applyDiscount(
          cartitem.product.price,
          cartitem.product.offer.discount
        );

      payment.amountPaid += this.applyDiscount(
        cartitem.product.price,
        cartitem.product.offer.discount
      );

      if (payment.amountPaid > 50000) payment.shippingCharges = 2000;
      else if (payment.amountPaid > 20000) payment.shippingCharges = 1000;
      else if (payment.amountPaid > 5000) payment.shippingCharges = 500;
      else payment.shippingCharges = 200;
    }
  }
  calculatePricePaid(cart: Cart) {
    let pricepaid = 0;
    for (let cartitem of cart.cartItems) {
      pricepaid += this.applyDiscount(
        cartitem.product.price,
        cartitem.product.offer.discount
      );
    }
    return pricepaid;
  }
  ngOnDestroy() {
    this.changeCart.unsubscribe();
  }
}
