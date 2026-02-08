select 
	-- M.Id,
	MAX(M.Name),
	count(S.Id) 
from 
	Sales S
	JOIN Managers M ON S.ManagerId = M.Id
where 
	Moment BETWEEN '2025-01-01' AND DATEADD(MONTH, 1, '2025-01-01')
GROUP BY
	M.Id
ORDER BY 
	2 DESC