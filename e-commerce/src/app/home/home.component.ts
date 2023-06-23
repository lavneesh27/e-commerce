import { Component } from '@angular/core';
import { suggestedProduct } from '../models/models';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent {
  suggestedProducts: suggestedProduct[] = [
    {
      banerimage: 'Baner/Baner_Mobile.png',
      category: {
        id: 0,
        category: 'electronics',
        subCategory: 'mobiles',
      },
    },
    {
      banerimage: 'Baner/Baner_Laptop.png',
      category: {
        id: 1,
        category: 'electronics',
        subCategory: 'laptops',
      },
    },
    {
      banerimage: 'Baner/Baner_Chair.png',
      category: {
        id: 2,
        category: 'furniture',
        subCategory: 'chairs',
      },
    },
  ];
}
