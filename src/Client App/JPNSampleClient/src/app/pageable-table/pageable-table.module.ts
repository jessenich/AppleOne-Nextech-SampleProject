import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';

import { PageableTableComponent } from './pageable-table.component';



@NgModule({
  declarations: [
    PageableTableComponent
  ],
  imports: [
    CommonModule,
    MatTableModule
  ],
  exports: [
    PageableTableComponent
  ]
})
export class PageableTableModule { }
