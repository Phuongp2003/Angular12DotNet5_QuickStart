import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import type { Department, DepartmentRequest } from './department.model';

@Component({
  selector: 'app-department',
  templateUrl: './department.component.html',
  styleUrls: ['./department.component.sass'],
})
export class DepartmentComponent implements OnInit {
  constructor(private http: HttpClient) {}

  departments: Department[] = [];
  modalTitle = 'No Title';
  modalClass = 'd-none';
  selectedDepartmentId = -1;
  selectedDepartment: DepartmentRequest = {};
  errorMessage = '';
  editMode: 'create' | 'edit' | 'delete' = 'create';

  ngOnInit(): void {
    this.refreshList();
  }

  refreshList() {
    this.http
      .get<Department[]>(environment.API_URL + '/Department')
      .subscribe((data) => {
        this.departments = data;
        console.log(data);
      });
  }

  // UI support
  hideModal() {
    this.modalClass = 'd-none';
  }

  showModal(type: 'create' | 'edit' | 'delete', id = -1) {
    this.editMode = type;
    if (type === 'edit' || type === 'delete') {
      this.http
        .get<Department>(
          environment.API_URL + `/Department/withId?departmentId=${id}`
        )
        .subscribe((data) => {
          this.selectedDepartment = data;
          this.selectedDepartmentId = id;
          this.modalTitle =
            type === 'edit'
              ? `Editing department with id ${id}`
              : `Deleting department with id ${id}`;
        });
    } else this.modalTitle = 'Create new department';

    this.modalClass = 'd-flex';
  }

  resetModal() {
    this.selectedDepartment = {};
    this.selectedDepartmentId = -1;
    this.errorMessage = '';
    this.modalTitle = '';
    this.editMode = 'create';
  }

  // Button logic
  createDepartment() {
    if (this.departmentValidated(this.selectedDepartment))
      this.http
        .post<Department>(
          environment.API_URL + '/Department',
          this.selectedDepartment
        )
        .subscribe((data) => {
          if (!data) this.errorMessage = 'Fail to create new Department!';
          else {
            this.departments.push(data);
            this.hideModal();
          }
        });
  }

  updateDepartment() {
    if (this.departmentValidated(this.selectedDepartment))
      this.http
        .put<Department>(
          environment.API_URL +
            `/Department?oldDepartmentId=${this.selectedDepartmentId}`,
          this.selectedDepartment
        )
        .subscribe((data) => {
          if (!data)
            this.errorMessage = `Fail to update Department with id ${this.selectedDepartmentId}!`;
          else {
            let od = this.departments.find(
              (d) => d.DepartmentId === this.selectedDepartmentId
            );
            if (od) {
              od.DepartmentName = data.DepartmentName;
              od.DepartmentSize = data.DepartmentSize;
            }
            this.hideModal();
          }
        });
  }

  deleteDepartment() {
    this.http
      .delete<Department>(
        environment.API_URL +
          `/Department?departmentId=${this.selectedDepartmentId}`
      )
      .subscribe((data) => {
        if (!data)
          this.errorMessage = `Fail to delete Department with id ${this.selectedDepartmentId}!`;
        else {
          this.departments = this.departments.filter(
            (d) => d.DepartmentId !== this.selectedDepartmentId
          );

          this.hideModal();
        }
      });
  }

  // Helper
  private departmentValidated(depp: DepartmentRequest): boolean {
    // Name check
    if (
      !depp.DepartmentName ||
      depp.DepartmentName?.length < 2 ||
      depp.DepartmentName.charAt(0).includes(' ')
    ) {
      this.errorMessage = 'Department name invalid!';
      return false;
    }

    if (!depp.DepartmentSize || depp.DepartmentSize < 0) {
      this.errorMessage = 'Department size invalid!';
      return false;
    }

    return true;
  }
}
