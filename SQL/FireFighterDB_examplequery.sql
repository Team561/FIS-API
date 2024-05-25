--Example query

use FirefighterDB
go
select Intervention.Location, InterventionType.Name as 'Intervention type',Firefighter.Name as 'Firefighter Name',Firefighter.Surname as 'Firefighter Surname',Firefighter.Active as 'Active?', Rank.Name as 'Rank', FireDepartment.Name as 'Fire Department',FireDepartment.Active as 'Active?', (select concat(Firefighter.Name,' ',Firefighter.Surname) from Firefighter where Firefighter.ID_FF = Intervention.Cmdr_ID) as 'Commander in charge of intervention'
from [Firefighter-Intervention] as fi
inner join Intervention on fi.Int_ID = Intervention.ID_Int
inner join InterventionType on Intervention.Type_ID = InterventionType.ID_Type
inner join Firefighter on fi.FF_ID = Firefighter.ID_FF
inner join Rank on Firefighter.Rank_ID = Rank.ID_Rank
inner join FireDepartment on Firefighter.FD_ID = FireDepartment.ID_FD
go