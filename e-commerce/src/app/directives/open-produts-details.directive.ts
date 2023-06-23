import { Directive, HostListener, Input } from '@angular/core';
import { Router } from '@angular/router';

@Directive({
  selector: '[OpenProdutsDetails]',
})
export class OpenProdutsDetailsDirective {
  @Input() productId: number = 0;
  @HostListener('click') openProductDetails() {
    window.scrollTo(0, 0);
    this.router.navigate(['/product-details'], {
      queryParams: {
        id: this.productId,
      },
    });
  }
  constructor(private router: Router) {}
}
