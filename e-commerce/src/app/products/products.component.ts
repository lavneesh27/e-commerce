import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NavigationService } from '../services/navigation.service';
import { UtilityService } from '../services/utility.service';
import { Product } from '../models/models';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css'],
})
export class ProductsComponent implements OnInit {
  view: 'grid' | 'list' = 'grid';
  sortBy: 'default' | 'htl' | 'lth' = 'default';
  products: Product[] = [];
  searchTerm: string = '';

  constructor(
    private activatedRoute: ActivatedRoute,
    private navigationService: NavigationService,
    private utilityService: UtilityService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params: any) => {
      let category = params.category;
      let subCategory = params.subcategory;

      if (category && subCategory) {
        this.navigationService
          .getProducts(category, subCategory, 10)
          .subscribe((res: any) => {
            this.products = res;
            this.view = 'grid';
            this.sortBy = 'default';
            this.searchTerm = '';
          });
      }
    });
  }

  sortByPrice(sortKey: string) {
    this.products.sort((a, b) => {
      if (sortKey === 'default') {
        return a.id > b.id ? 1 : -1;
      }

      return (
        (sortKey === 'htl' ? 1 : -1) *
        (this.utilityService.applyDiscount(a.price, a.offer.discount) >
        this.utilityService.applyDiscount(b.price, b.offer.discount)
          ? -1
          : 1)
      );
    });
  }
  find() {
    this.activatedRoute.queryParams.subscribe((params: any) => {
      let category = params.category;
      let subCategory = params.subcategory;

      if (category && subCategory) {
        this.navigationService
          .getProducts(category, subCategory, 10)
          .subscribe((res: any) => {
            this.products = res.filter((product: any) => {
              const searchField = [product.title];

              const searchText = this.searchTerm.toLowerCase();

              return searchField.some((field) =>
                field.toLowerCase().includes(searchText)
              );
            });
          });
      }
    });
  }
}
