create database if not exists hrm_db;
use hrm_db;
drop table if exists Department;
create table Department(
	DepartmentID int auto_increment,
    DepartmentName varchar(128) unique,
    DepartmentSize smallint,
    primary key(DepartmentID)
);
insert into Department(DepartmentName, DepartmentSize)
	values ("IT", 25), ("QC", 5);
    
drop table if exists User;
create table User(
	UserId int auto_increment,
    FullName varchar(36) unique,
    DateOfBirth Date,
    Avatar text,
    DepartmentId int,
    primary key(UserId),
    foreign key(DepartmentId) references Department(DepartmentId)
);
insert into User(FullName, DateOfBirth, Avatar, DepartmentId)
	values ("Nguyen Van A", "2003-09-25", "anonymous.png", 1), ("Nguyen Van C", "2003-03-25", "anonymous.png", 2);
    