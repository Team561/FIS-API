use master
go

create database FireFighterDB
go

use FireFighterDB -- PAAD project
go

---version 2 (simpler) <--- this one

CREATE TABLE dbo.FireDepartment
(
    [ID_FD] uniqueidentifier primary key default NEWSEQUENTIALID(),
    [Name] nvarchar(50) not null,
	[Location] nvarchar(50) not null, --simple
	[Cmdr_ID] uniqueidentifier null,
	[Active] bit not null,
);
go

CREATE TABLE dbo.FireFighter
(
    [ID_FF] uniqueidentifier primary key default NEWSEQUENTIALID(),
    [Name] nvarchar(50) not null,
	[Surname] nvarchar(50) not null,
	[ActiveDate] datetime null,
	[Rank_ID] int not null,
	[FD_ID] uniqueidentifier not null
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
    [Location] nvarchar(150) not null,
	[Cmdr_ID] uniqueidentifier not null,
	[Type_ID] int not null,
	[Active] bit not null,
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
	[FF_ID] uniqueidentifier not null,
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
    [Name] nvarchar(50) not null,
);
go

Alter table dbo.[Intervention]
ADD CONSTRAINT FK_InterventionType
FOREIGN KEY ([Type_ID]) REFERENCES InterventionType(ID_Type);
go

CREATE TABLE dbo.Login
(
	[Username] nvarchar(50),
    [Password] BINARY(64),
    [UserGUID] uniqueidentifier,
);
go