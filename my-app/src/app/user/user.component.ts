import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import type { User } from './user.model';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.sass'],
})
export class UserComponent implements OnInit {
  constructor(private http: HttpClient) {}
  employees: User[] = []

  ngOnInit(): void {
    this.refreshList();
  }

   refreshList() {
      this.http.get<User[]>(environment.API_URL+'/User')
      .subscribe(
        data=> {
          this.employees = data
        }
      )
    }
}
