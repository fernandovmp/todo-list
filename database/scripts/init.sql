create database TodoList;
go
use TodoList;
go
create table Users (
    id int identity primary key,
    username varchar(100) unique not null,
    password varchar(75) not null,
    createdOn datetime2(2) default SYSDATETIME()
)
go
create table TodoItems (
    id int identity primary key,
    title varchar(100) not null,
    completed bit not null,
    userId int foreign key references Users not null,
    createdOn datetime2(2) default SYSDATETIME()
)
go
