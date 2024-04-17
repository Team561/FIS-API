use master
go

create database FireFighterDB
go

use FireFighterDB -- PAAD project
go

--- version 1 (doesn't need to be this complex, best to use version 2 below)
CREATE TABLE dbo.FireDepartment
(
    [ID_FD] int identity primary key,
    [Name] int not null,
	[Location] nvarchar(50), --simple
	[Cmdr_ID] int,
	[Active] bit,
);
go

CREATE TABLE dbo.FireFighter
(
    [ID_FF] int identity primary key,
    [Name] int not null,
	[Surname] int not null,
	[ActiveDate] datetime null,
	[Rank_ID] int not null,
	[ID_Cmdr] int null,
	[FD_ID] int not null
);
go

CREATE TABLE dbo.[Rank]
(
    [ID_Rank] int identity primary key,
    [Name] int not null,
);
go

Alter table dbo.FireFighter
ADD CONSTRAINT FK_Commander --drop constraint
FOREIGN KEY (ID_Cmdr) REFERENCES FireFighter(ID_FF);
go

Alter table dbo.FireFighter
ADD CONSTRAINT FK_Rank --drop constraint
FOREIGN KEY (Rank_ID) REFERENCES [Rank](ID_Rank);
go

Alter table dbo.FireFighter
ADD CONSTRAINT FK_FireDepartment --drop constraint
FOREIGN KEY (FD_ID) REFERENCES FireDepartment(ID_FD);
go

Alter table dbo.FireDepartment
ADD CONSTRAINT FK_FireFighterCommander --drop constraint
FOREIGN KEY (Cmdr_ID) REFERENCES FireFighter(ID_FF);
go

--- end of version 1

--drop table FireDepartment
--drop table FireFighter
--drop table [Rank]



---version 2 (simpler) <--- this one

CREATE TABLE dbo.FireDepartment
(
    [ID_FD] int identity primary key,
    [Name] int not null,
	[Location] nvarchar(50), --simple
	[Cmdr_ID] int,
	[Active] bit,
);
go

CREATE TABLE dbo.FireFighter
(
    [ID_FF] int identity primary key,
    [Name] int not null,
	[Surname] int not null,
	[ActiveDate] datetime null,
	[Rank_ID] int not null,
	[FD_ID] int not null
);
go

CREATE TABLE dbo.[Rank]
(
    [ID_Rank] int identity primary key,
    [Name] nvarchar(50) not null,
);
go

Alter table dbo.FireFighter
ADD CONSTRAINT FK_Rank
FOREIGN KEY (Rank_ID) REFERENCES [Rank](ID_Rank);
go

Alter table dbo.FireFighter
ADD CONSTRAINT FK_FireDepartment
FOREIGN KEY (FD_ID) REFERENCES FireDepartment(ID_FD);
go

Alter table dbo.FireDepartment
ADD CONSTRAINT FK_FireFighterCommander
FOREIGN KEY (Cmdr_ID) REFERENCES FireFighter(ID_FF);
go

CREATE TABLE dbo.Intervention
(
    [ID_Int] int identity primary key,
    [Location] nvarchar(50) not null,
	[Cmdr_ID] int not null,
	[Type_ID] int not null,
	[Active] bit,
);
go

Alter table dbo.Intervention
ADD CONSTRAINT FK_InterventionCommander
FOREIGN KEY (Cmdr_ID) REFERENCES FireFighter(ID_FF);
go

CREATE TABLE dbo.[FireFighter-Intervention]
(
    [ID] int identity primary key,
    [Int_ID] int not null,
	[FF_ID] int not null,
);
go

Alter table dbo.[FireFighter-Intervention]
ADD CONSTRAINT FK_FI_Intervention
FOREIGN KEY (Int_ID) REFERENCES Intervention(ID_Int);
go

Alter table dbo.[FireFighter-Intervention]
ADD CONSTRAINT FK_FI_FireFighter
FOREIGN KEY (FF_ID) REFERENCES FireFighter(ID_FF);
go

CREATE TABLE dbo.[InterventionType]
(
    [ID_Type] int identity primary key,
    [Name] int not null,
);
go

Alter table dbo.[Intervention]
ADD CONSTRAINT FK_InterventionType
FOREIGN KEY ([Type_ID]) REFERENCES InterventionType(ID_Type);
go
---

--version 2 removal (uncomment to use)

--Alter table dbo.FireFighter
--drop CONSTRAINT FK_Rank
--go

--Alter table dbo.FireFighter
--drop CONSTRAINT FK_FireDepartment
--go

--Alter table dbo.FireDepartment
--drop CONSTRAINT FK_FireFighterCommander
--go

--Alter table dbo.Intervention
--drop CONSTRAINT FK_InterventionCommander
--go

--Alter table dbo.[FireFighter-Intervention]
--drop CONSTRAINT FK_FI_Intervention
--go

--Alter table dbo.[FireFighter-Intervention]
--drop CONSTRAINT FK_FI_FireFighter
--go

--Alter table dbo.[Intervention]
--drop CONSTRAINT FK_InterventionType
--go

--drop table dbo.FireDepartment
--drop table dbo.FireFighter
--drop table dbo.Rank
--drop table dbo.Intervention
--drop table dbo.[FireFighter-Intervention]
--drop table dbo.InterventionType