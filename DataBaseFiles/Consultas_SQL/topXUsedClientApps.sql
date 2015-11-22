USE AzuPass
DECLARE @IdPerfil int
SET @IdPerfil = 1

SELECT TOP 3 COUNT(IdPerfil) AS Cuenta, MIN(FechaHora) AS Desde, MAX(FechaHora) AS Hasta, clientAppName, clientAppURL
FROM         tbl_RegistroUso
WHERE IdPerfil = @IdPerfil
GROUP BY clientAppName, clientAppURL
ORDER BY Cuenta DESC, Hasta DESC 

SELECT * FROM tbl_RegistroUso