

----- version 1 (don't use, includes involute relation, use version 2)


--CREATE TABLE dbo.FireDepartment
--(
--    [ID_FD] int identity primary key,
--    [Name] nvarchar(50) not null,
--	[Location] nvarchar(50), --simple
--	[Cmdr_ID] int,
--	[Active] bit,
--);
--go

--CREATE TABLE dbo.FireFighter
--(
--    [ID_FF] int identity primary key,
--    [Name] nvarchar(50) not null,
--	[Surname] int not null,
--	[ActiveDate] datetime null,
--	[Rank_ID] int not null,
--	[ID_Cmdr] int null,
--	[FD_ID] int not null
--);
--go

--CREATE TABLE dbo.[Rank]
--(
--    [ID_Rank] int identity primary key,
--    [Name] nvarchar(50) not null,
--);
--go

--Alter table dbo.FireFighter
--ADD CONSTRAINT FK_Commander --drop constraint
--FOREIGN KEY (ID_Cmdr) REFERENCES FireFighter(ID_FF);
--go

--Alter table dbo.FireFighter
--ADD CONSTRAINT FK_Rank --drop constraint
--FOREIGN KEY (Rank_ID) REFERENCES [Rank](ID_Rank);
--go

--Alter table dbo.FireFighter
--ADD CONSTRAINT FK_FireDepartment --drop constraint
--FOREIGN KEY (FD_ID) REFERENCES FireDepartment(ID_FD);
--go

--Alter table dbo.FireDepartment
--ADD CONSTRAINT FK_FireFighterCommander --drop constraint
--FOREIGN KEY (Cmdr_ID) REFERENCES FireFighter(ID_FF);
--go

--- end of version 1

--drop table FireDepartment
--drop table FireFighter
--drop table [Rank]


