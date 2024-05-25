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

Insert into [FireDepartment] (Name, Location, Cmdr_ID, Active)
values 
('Zagreb FD1', 'Zagreb',1,1)
go

Insert into FireFighter (Name, Surname, ActiveDate, Rank_ID, FD_ID)
values
('Walter','White',CURRENT_TIMESTAMP,1,1)
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
('Saul','Goodman',CURRENT_TIMESTAMP,1,2)
go

update FireDepartment
set Cmdr_ID = 2
where FireDepartment.ID_FD = 2
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
('Zagreb',1,2,1)
go

Insert into FireFighter (Name, Surname, ActiveDate, Rank_ID, FD_ID)
values
('Jesse','Pinkman',CURRENT_TIMESTAMP,4,1),
('Hank','Schrader',CURRENT_TIMESTAMP,2,1),
('Skyler','White',CURRENT_TIMESTAMP,4,1)
go

select * from Intervention
select * from FireFighter
select * from FireDepartment
select * from Rank
insert into [FireFighter-Intervention](Int_ID, FF_ID)
values
(1,1),
(1,3),
(1,4),
(1,5)
go

--Example query
select Intervention.Location, InterventionType.Name as 'Intervention type',FireFighter.Name,FireFighter.Surname, Rank.Name as 'Rank', FireDepartment.Name as 'Fire Department', (select concat(FireFighter.Name,' ',FireFighter.Surname) from FireFighter where FireFighter.ID_FF = Intervention.Cmdr_ID) as 'Commander in charge of intervention'
from [FireFighter-Intervention] as fi
inner join Intervention on fi.Int_ID = Intervention.ID_Int
inner join InterventionType on Intervention.Type_ID = InterventionType.ID_Type
inner join FireFighter on fi.FF_ID = FireFighter.ID_FF
inner join Rank on FireFighter.Rank_ID = Rank.ID_Rank
inner join FireDepartment on FireFighter.FD_ID = FireDepartment.ID_FD
go