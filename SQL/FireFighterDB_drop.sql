Alter table dbo.FireFighter
drop CONSTRAINT FK_Rank
go

Alter table dbo.FireFighter
drop CONSTRAINT FK_FireDepartment
go

Alter table dbo.FireDepartment
drop CONSTRAINT FK_FireFighterCommander
go

Alter table dbo.Intervention
drop CONSTRAINT FK_InterventionCommander
go

Alter table dbo.[FireFighter-Intervention]
drop CONSTRAINT FK_FI_Intervention
go

Alter table dbo.[FireFighter-Intervention]
drop CONSTRAINT FK_FI_FireFighter
go

Alter table dbo.[Intervention]
drop CONSTRAINT FK_InterventionType
go

drop table dbo.FireDepartment
drop table dbo.FireFighter
drop table dbo.Rank
drop table dbo.Intervention
drop table dbo.[FireFighter-Intervention]
drop table dbo.InterventionType
drop table dbo.Login