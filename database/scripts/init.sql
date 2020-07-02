create database TodoList;
go
use TodoList;
go
create table TodoItems (
    id int identity primary key,
    title varchar(100) not null,
    completed bit not null,
    createdOn datetime2(2) default SYSDATETIME()
)
go
