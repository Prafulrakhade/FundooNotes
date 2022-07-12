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

--created store Procedure
create procedure spAddUser(
@FirstName varchar(50), 
@LastName varchar(50),
@Email varchar(50),
@Password varchar(100)
)
As
Begin try
insert into Users(FirstName,LastName,Email,Password) values(@FirstName,@LastName,@Email,@Password)
--select * from Users where Email=@Email
end try
Begin catch
SELECT 
	ERROR_NUMBER() AS ErrorNumber,
	ERROR_STATE() AS ErrorState,
	ERROR_PROCEDURE() AS ErrorProcedure,
	ERROR_LINE() AS ErrorLine,
	ERROR_MESSAGE() AS ErrorMessage;
END CATCH

--executing the spAddUser stored procedure
exec spAddUser 'Raj','kumar','Raj@gmail.com','Raj@123'
select * from Users

--Created Stored Procedure to get all records
create procedure spGetAllUser
As
Begin try
select * from Users
end try
Begin catch
SELECT 
	ERROR_NUMBER() AS ErrorNumber,
	ERROR_STATE() AS ErrorState,
	ERROR_PROCEDURE() AS ErrorProcedure,
	ERROR_LINE() AS ErrorLine,
	ERROR_MESSAGE() AS ErrorMessage;
END CATCH

exec spGetAllUser

create procedure spLoginUser(
@Email varchar(50),
@Password varchar(50)
)
As
Begin try
select * from Users where Email=@Email and password = @Password
end try
Begin catch
SELECT 
	ERROR_NUMBER() AS ErrorNumber,
	ERROR_STATE() AS ErrorState,
	ERROR_PROCEDURE() AS ErrorProcedure,
	ERROR_LINE() AS ErrorLine,
	ERROR_MESSAGE() AS ErrorMessage;
END CATCH

exec spLoginUser 'prafulrakhade02@gamil.com' ,'Praful@02'

--select * from userSignup
--truncate table userSignup
select * from Users

Create procedure spForgetPasswordUser(
@Email varchar(50)
)
As
Begin try
select * from Users where Email=@Email 
end try
Begin catch
SELECT 
	ERROR_NUMBER() AS ErrorNumber,
	ERROR_STATE() AS ErrorState,
	ERROR_PROCEDURE() AS ErrorProcedure,
	ERROR_LINE() AS ErrorLine,
	ERROR_MESSAGE() AS ErrorMessage;
END CATCH

exec spForgetPasswordUser 'pravinShellar@gamil.com'

alter procedure spResetPassword(
@Email varchar(50),
@Password varchar(50)
)
As
Begin try
update Users set Password=@Password where Email=@Email
end try
Begin catch
SELECT 
    ERROR_NUMBER() AS ErrorNumber,
	ERROR_STATE() AS ErrorState,
	ERROR_PROCEDURE() AS ErrorProcedure,
	ERROR_LINE() AS ErrorLine,
	ERROR_MESSAGE() AS ErrorMessage;
END CATCH


create table Note(
NoteId int identity(1,1) primary key,
Title varchar(20) not null,
Description varchar(max) not null,
Bgcolor varchar(50) not null,
IsPin bit,
IsArchive bit,
IsRemainder bit,
IsTrash bit,
UserId int not null foreign key references Users(UserId),
RegisteredDate datetime default GETDATE(),
Remainder datetime,
ModifiedDate datetime null
)
select * from Note

Create procedure spAddNote(
@Title varchar(20), 
@Description varchar(max),
@BgColor varchar(50),
@UserId int
)
As
Begin try
insert into Note(Title,Description,Bgcolor,UserId,ModifiedDate) values(@Title,@Description,@BgColor,@UserId,GetDate())
Select * from Note where UserId = @UserId
end try
Begin catch
SELECT 
	ERROR_NUMBER() AS ErrorNumber,
	ERROR_STATE() AS ErrorState,
	ERROR_PROCEDURE() AS ErrorProcedure,
	ERROR_LINE() AS ErrorLine,
	ERROR_MESSAGE() AS ErrorMessage;
END CATCH