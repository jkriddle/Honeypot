DECLARE @expectedBuildVersion varchar(20)
DECLARE @expectedCheckSumTableViewColumn bigint
DECLARE @expectedCheckSumConstraints bigint
DECLARE @expectedCheckSumRoutine bigint

SET @expectedBuildVersion = '1.0.0.0'
SET @expectedCheckSumTableViewColumn = 0
SET @expectedCheckSumConstraints = 0
SET @expectedCheckSumRoutine = 0

SELECT BuildLabel
FROM dbo.BuildVersions
WHERE BuildDate = (SELECT MAX(BuildDate) FROM dbo.BuildVersions)
AND BuildLabel = @expectedBuildVersion

