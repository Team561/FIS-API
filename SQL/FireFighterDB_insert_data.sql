use FirefighterDB
go
----------------------------- RUN THIS AFTER version 2 part of FirefighterDB.sql
Insert into [Rank] (Name)
values 
('Fire Fighter Commander'),
('Fire Fighter First Class'),
('Fire Fighter Second Class'),
('Fire Fighter Initiate')
go
--select * from Rank
--delete from Rank

Alter table dbo.FireDepartment
drop CONSTRAINT FK_FirefighterCommander

Insert into [FireDepartment] (Name, Location, Active)
values 
('Zagreb FD1', 'Zagreb',1)
go

Insert into Firefighter (Name, Surname, ActiveDate, Rank_ID, FD_ID, Active)
values
('Walter','White',CURRENT_TIMESTAMP,1,(select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zagreb FD1'),1)
go

update FireDepartment
set Cmdr_ID = (select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Walter' and Firefighter.Surname = 'White')
where FireDepartment.ID_FD = (select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zagreb FD1')
go

Alter table dbo.FireDepartment
ADD CONSTRAINT FK_FirefighterCommander
FOREIGN KEY (Cmdr_ID) REFERENCES Firefighter(ID_FF);
go

Insert into [FireDepartment] (Name, Location, Active)
values 
('Zadar FD5', 'Zadar',1)
go


select * from FireDepartment
select * from Firefighter
go

Insert into Firefighter (Name, Surname, ActiveDate, Rank_ID, FD_ID, Active)
values
('Saul','Goodman',CURRENT_TIMESTAMP,1,(select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zadar FD5'),1)
go

update FireDepartment
set Cmdr_ID = (select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Saul' and Firefighter.Surname = 'Goodman')
where FireDepartment.ID_FD = (select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zadar FD5')
go

select * from FireDepartment
select * from Firefighter
go

insert into InterventionType(Name)
values
('Fire'),
('Electrical Fire')
go

select * from InterventionType
insert into Intervention(Location, Cmdr_ID, Type_ID, Active)
values
('Zagreb',(select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Walter' and Firefighter.Surname = 'White'),2,1)
go

Insert into Firefighter (Name, Surname, ActiveDate, Rank_ID, FD_ID, Active)
values
('Jesse','Pinkman',CURRENT_TIMESTAMP,4,(select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zagreb FD1'),1),
('Hank','Schrader',CURRENT_TIMESTAMP,2,(select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zagreb FD1'),1),
('Skyler','White',CURRENT_TIMESTAMP,4,(select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zagreb FD1'),1)
go

select * from Intervention
select * from Firefighter
select * from FireDepartment
select * from Rank
insert into [Firefighter-Intervention](Int_ID, FF_ID)
values
(1,(select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Walter' and Firefighter.Surname = 'White')),
(1,(select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Jesse' and Firefighter.Surname = 'Pinkman')),
(1,(select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Hank' and Firefighter.Surname = 'Schrader')),
(1,(select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Skyler' and Firefighter.Surname = 'White'))
go

insert into Login(Username, PasswordHash, PasswordSalt, UserGUID)
values(
	'WalterWhite1',
	'UpEaTlAoTDVDS4l3/CWXMOl6rI67CLyoqJBgz4J4ltI=',
	'+C5TLnxyy6C82SJlC6PIyw==',
	(select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Walter' and Firefighter.Surname = 'White')
), (
	'SaulTheGoodMan',
	'eyy2XIzR0q+QXYt4iiYD0jL7l2qRp4LPu0Z8UHhbMZ0=',
	'on6DmlO0Yz/tQmWRxdIlpA==',
	(select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Saul' and Firefighter.Surname = 'Goodman')
), (
	'JPinkman ',
	'LLLcJSxCPBjPI3smBVoTr4JPDU0naSI2Qbgdfiqaefw=',
	'uL2x7MvQvc35F0FYz67n/Q==',
	(select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Jesse' and Firefighter.Surname = 'Pinkman')
), (
	'SchraderHa',
	'YMUbgsMf0QoxU8XaWreA8ApqDxjBNQmXWfdbFj2pOOQ=',
	'2jROlTdNlAEd0IHZXsrhvw==',
	(select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Hank' and Firefighter.Surname = 'Schrader')
), (
	'WhiteFF ',
	'LJ/Oz3WdTB48FVXripZdAKSsS0t1rJJy4zBRN7g5KaQ=',
	'+IgfRhbouffh69Sv+T/Tjw==',
	(select Firefighter.ID_FF from Firefighter where Firefighter.Name = 'Skyler' and Firefighter.Surname = 'White')
)
