use FireFighterDB
go
----------------------------- RUN THIS AFTER ---version 2 part of FireFighterDB.sql
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
drop CONSTRAINT FK_FireFighterCommander

Insert into [FireDepartment] (Name, Location, Active)
values 
('Zagreb FD1', 'Zagreb',1)
go

Insert into FireFighter (Name, Surname, ActiveDate, Rank_ID, FD_ID)
values
('Walter','White',CURRENT_TIMESTAMP,1,(select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zagreb FD1'))
go

update FireDepartment
set Cmdr_ID = (select FireFighter.ID_FF from FireFighter where FireFighter.Name = 'Walter' and FireFighter.Surname = 'White')
where FireDepartment.ID_FD = (select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zagreb FD1')
go

Alter table dbo.FireDepartment
ADD CONSTRAINT FK_FireFighterCommander
FOREIGN KEY (Cmdr_ID) REFERENCES FireFighter(ID_FF);
go

Insert into [FireDepartment] (Name, Location, Active)
values 
('Zadar FD5', 'Zadar',1)
go


select * from FireDepartment
select * from FireFighter
go

Insert into FireFighter (Name, Surname, ActiveDate, Rank_ID, FD_ID)
values
('Saul','Goodman',CURRENT_TIMESTAMP,1,(select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zadar FD5'))
go

update FireDepartment
set Cmdr_ID = (select FireFighter.ID_FF from FireFighter where FireFighter.Name = 'Saul' and FireFighter.Surname = 'Goodman')
where FireDepartment.ID_FD = (select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zadar FD5')
go

select * from FireDepartment
select * from FireFighter
go

insert into InterventionType(Name)
values
('Fire'),
('Electrical Fire')
go

select * from InterventionType
insert into Intervention(Location, Cmdr_ID, Type_ID, Active)
values
('Zagreb',(select FireFighter.ID_FF from FireFighter where FireFighter.Name = 'Walter' and FireFighter.Surname = 'White'),2,1)
go

Insert into FireFighter (Name, Surname, ActiveDate, Rank_ID, FD_ID)
values
('Jesse','Pinkman',CURRENT_TIMESTAMP,4,(select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zagreb FD1')),
('Hank','Schrader',CURRENT_TIMESTAMP,2,(select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zagreb FD1')),
('Skyler','White',CURRENT_TIMESTAMP,4,(select FireDepartment.ID_FD from FireDepartment where FireDepartment.Name = 'Zagreb FD1'))
go

select * from Intervention
select * from FireFighter
select * from FireDepartment
select * from Rank
insert into [FireFighter-Intervention](Int_ID, FF_ID)
values
(1,(select FireFighter.ID_FF from FireFighter where FireFighter.Name = 'Walter' and FireFighter.Surname = 'White')),
(1,(select FireFighter.ID_FF from FireFighter where FireFighter.Name = 'Jesse' and FireFighter.Surname = 'Pinkman')),
(1,(select FireFighter.ID_FF from FireFighter where FireFighter.Name = 'Hank' and FireFighter.Surname = 'Schrader')),
(1,(select FireFighter.ID_FF from FireFighter where FireFighter.Name = 'Skyler' and FireFighter.Surname = 'White'))
go