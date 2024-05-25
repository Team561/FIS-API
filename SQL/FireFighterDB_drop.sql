use FirefighterDB
go

Alter table dbo.Firefighter
drop CONSTRAINT FK_Rank
go

Alter table dbo.Firefighter
drop CONSTRAINT FK_FireDepartment
go

Alter table dbo.FireDepartment
drop CONSTRAINT FK_FirefighterCommander
go

Alter table dbo.Intervention
drop CONSTRAINT FK_InterventionCommander
go

Alter table dbo.[Firefighter-Intervention]
drop CONSTRAINT FK_FI_Intervention
go

Alter table dbo.[Firefighter-Intervention]
drop CONSTRAINT FK_FI_Firefighter
go

Alter table dbo.[Intervention]
drop CONSTRAINT FK_InterventionType
go

drop table dbo.FireDepartment
drop table dbo.Firefighter
drop table dbo.Rank
drop table dbo.Intervention
drop table dbo.[Firefighter-Intervention]
drop table dbo.InterventionType
drop table dbo.Login

--open connections won't let you drop so it's commented
--drop database FirefighterDB