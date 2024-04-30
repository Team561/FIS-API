--Example query
select Intervention.Location, InterventionType.Name as 'Intervention type',FireFighter.Name,FireFighter.Surname, Rank.Name as 'Rank', FireDepartment.Name as 'Fire Department', (select concat(FireFighter.Name,' ',FireFighter.Surname) from FireFighter where FireFighter.ID_FF = Intervention.Cmdr_ID) as 'Commander in charge of intervention'
from [FireFighter-Intervention] as fi
inner join Intervention on fi.Int_ID = Intervention.ID_Int
inner join InterventionType on Intervention.Type_ID = InterventionType.ID_Type
inner join FireFighter on fi.FF_ID = FireFighter.ID_FF
inner join Rank on FireFighter.Rank_ID = Rank.ID_Rank
inner join FireDepartment on FireFighter.FD_ID = FireDepartment.ID_FD
go