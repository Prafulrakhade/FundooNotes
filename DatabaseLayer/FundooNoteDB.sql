use FundooNotes
create table Users(
UserID int identity(1,1) primary key,
FirstName varchar (50),
LastName varchar (50),
Email varchar (50) unique,
Password varchar (100) not null,
CreatedDate datetime default getdate(),
ModifiedDate datetime default getdate()
)

--we can use 

select sysdatetime();

-- or 
select getdate();

-- to fetch system time
insert into Users(FirstName, LastName, Email, Password) values('Akash','Lanjewar','akashlanjewar01@gamil.com','akash@123')
select *from Users

select @@SERVICENAME