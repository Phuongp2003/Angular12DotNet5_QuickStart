# Quick start - Angular 12 + .Net 5 API

## Setup
### Database
- Import or just run [sql file](Angular_quickStart_hrm.sql) 
> WARNING: it will drop table User and Department if it already exists
### Backend
- Setup DB Url at [.net config file](.\MyCURDApp\MyCURDApp\appsettings.json)
### Frontend
- Setup Backend Endpoint for API and Photos at [environment file](my-app\src\environments\environment.ts)

## Angular scirpt
### Start
```bash
npm start
```
> Remember to install all dependencies 

## State
- [X] CURD 2 User (missing get 1) and Department 
- [X] Show and edit Department in client side
- [ ] Show and filter User with department Id (or not)
- [ ] CURD user in client side
