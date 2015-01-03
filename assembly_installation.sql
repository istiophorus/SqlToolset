
-- alter database SqlToolsetHost set trustworthy on 


exec sp_configure 'Show Advanced Options', 1
reconfigure
go

exec sp_configure 'CLR Enable', 1
reconfigure
go

-- create assembly SqlToolset from 'd:\sqlprojects\SqlToolset\bin\Debug\Skra.Sql.SqlToolset.dll'

if exists(select * from sys.types where name = 'MacAddressType')
	drop type dbo.MacAddressType
go

if exists(select * from sys.objects where name = 'Concatenate' and type = 'AF')
	drop aggregate Concatenate
go

if exists(select * from sys.objects where name = 'MatchesPattern' and type = 'FS')	
	drop function dbo.MatchesPattern
go

if exists(select * from sys.objects where name = 'CalculateFileMD5' and type = 'FS')	
	drop function dbo.CalculateFileMD5
go

if exists(select * from sys.objects where name = 'CalculateDataMD5' and type = 'FS')	
	drop function dbo.CalculateDataMD5
go

if exists(select * from sys.objects where name = 'CalculateDataSHA' and type = 'FS')	
	drop function dbo.CalculateDataSHA
go

if exists(select * from sys.objects where name = 'DataCompression' and type = 'FS')	
	drop function dbo.DataCompression
go

if exists(select * from sys.objects where name = 'DaysInMonths' and type = 'FT')	
	drop function dbo.DaysInMonths
go

if exists(select * from sys.objects where name = 'Split' and type = 'FT')	
	drop function dbo.Split
go

if exists(select * from sys.objects where name = 'SplitToInts' and type = 'FT')	
	drop function dbo.SplitToInts
go

if exists(select * from sys.objects where name = 'BasicPivot' and type = 'PC')	
	drop procedure dbo.BasicPivot
go

if exists(select * from sys.objects where name = 'MaxCommonDividor' and type = 'FS')	
	drop function dbo.MaxCommonDividor
go

if exists(select * from sys.objects where name = 'CreateSeries' and type = 'FT')	
	drop function dbo.CreateSeries
go

if exists(select * from sys.objects where name = 'CreateRandomIntSeries' and type = 'FT')	
	drop function dbo.CreateRandomIntSeries
go

if exists(select * from sys.objects where name = 'MovingAverage' and type = 'FS')	
	drop function dbo.MovingAverage
go

if exists(select * from sys.objects where name = 'RollingSum' and type = 'FS')	
	drop function dbo.RollingSum
go

if exists(select * from sys.objects where name = 'EMA' and type = 'FS')	
	drop function dbo.EMA
go

if exists(select * from sys.objects where name = 'ClearCache')	
	drop procedure dbo.ClearCache
go

if exists(select * from sys.objects where name = 'GetSystemUserNameFromBinary' and type = 'FS')	
	drop function dbo.GetSystemUserNameFromBinary
go

if exists(select * from sys.objects where name = 'GetSystemUserNameFromString' and type = 'FS')	
	drop function dbo.GetSystemUserNameFromString
go

if exists(select * from sys.objects where name = 'PreviousValue' and type = 'FS')	
	drop function dbo.PreviousValue
go

if exists(select * from sys.objects where name = 'ValueBack' and type = 'FS')	
	drop function dbo.ValueBack
go


if exists(select * from sys.assemblies where name = 'SqlToolset')
	drop assembly SqlToolset
go

create assembly SqlToolset from 'd:\SqlToolset\bin\Debug\Skra.Sql.SqlToolset.dll'
with PERMISSION_SET = safe
go

create aggregate Concatenate(@value nvarchar(4000))
returns nvarchar(4000)
external name [SqlToolset].[Skra.Sql.SqlToolset.Concatenate]
go

create function MatchesPattern(@pattern nvarchar(4000), @value nvarchar(4000))
returns integer
external name [SqlToolset].[Skra.Sql.SqlToolset.StringOperations].[MatchesPattern]
go

create function DaysInMonths(@year int)
returns table ([Month] int, [Days] int)
external name [SqlToolset].[Skra.Sql.SqlToolset.DateTimeOperations].DaysInMonths
go

create function CalculateDataMD5(@data varbinary(max))
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[Skra.Sql.SqlToolset.BlobOperations].CalculateDataMD5
go

create function CalculateDataSHA(@data varbinary(max))
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[Skra.Sql.SqlToolset.BlobOperations].CalculateDataSHA
go

create function DataCompression(@blob varbinary(max), @decompress bit)
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[Skra.Sql.SqlToolset.BlobOperations].DataCompression
go

create function Split(@input nvarchar(max), @separators nvarchar(max))
returns table 
(
			[value] nvarchar(max)
)
external name [SqlToolset].[Skra.Sql.SqlToolset.StringOperations].Split
go

create function SplitToInts(@input nvarchar(max), @separators nvarchar(max))
returns table 
(
			[value] bigint
)
external name [SqlToolset].[Skra.Sql.SqlToolset.StringOperations].SplitToInts
go

create type [dbo].MacAddressType
external name [SqlToolset].[Skra.Sql.SqlToolset.Types.MacAddressType]
go

create procedure BasicPivot
@cmd nvarchar(max)
as
external name [SqlToolset].[Skra.Sql.SqlToolset.MiscelaneusTools].BasicPivot
go

create function MaxCommonDividor(@a bigint, @b bigint)
returns bigint
external name [SqlToolset].[Skra.Sql.SqlToolset.MathFunctions].MaxCommonDividor
go

create function CreateSeries(@a int, @b int, @n1 int, @n2 int)
returns table ([n] int, [value] bigint)
external name [SqlToolset].[Skra.Sql.SqlToolset.Series].CreateSeries
go

create function CreateRandomIntSeries(@range int, @count int)
returns table ([n] int, [value] bigint)
external name [SqlToolset].[Skra.Sql.SqlToolset.Series].CreateRandomIntSeries
go

create function dbo.MovingAverage(@id uniqueidentifier, @length int, @value decimal(38, 4))
returns decimal(38, 4)
external name [SqlToolset].[Skra.Sql.SqlToolset.MathFunctions].MovingAverage
go

create procedure dbo.ClearCache
@id uniqueidentifier
as
external name [SqlToolset].[Skra.Sql.SqlToolset.MathFunctions].ClearCache
go

create function dbo.GetSystemUserNameFromBinary(@sid varbinary(max), @nullOnError bit)
returns sysname
as
external name [SqlToolset].[Skra.Sql.SqlToolset.MiscelaneusTools].GetSystemUserNameFromBinary
go

create function dbo.GetSystemUserNameFromString(@sid sysname)
returns sysname
as
external name [SqlToolset].[Skra.Sql.SqlToolset.MiscelaneusTools].GetSystemUserNameFromString
go

create function dbo.RollingSum(@id uniqueidentifier, @value decimal(38, 4))
returns decimal(38, 4)
external name [SqlToolset].[Skra.Sql.SqlToolset.MathFunctions].RollingSum
go

create function dbo.EMA(@id uniqueidentifier, @length int, @value decimal(38, 4))
returns decimal(38, 4)
external name [SqlToolset].[Skra.Sql.SqlToolset.MathFunctions].EMA
go

create function dbo.PreviousValue(@id uniqueidentifier, @value sql_variant)
returns sql_variant
external name [SqlToolset].[Skra.Sql.SqlToolset.MathFunctions].PreviousValue
go

create function dbo.ValueBack(@id uniqueidentifier, @length int, @value sql_variant)
returns sql_variant
external name [SqlToolset].[Skra.Sql.SqlToolset.MathFunctions].ValueBack
go

declare @w [dbo].MacAddressType
set @w = 'AABBCCFF9932'
select cast(@w as varchar)
select cast(@w as varbinary)

declare @w2 [dbo].MacAddressType
set @w2 = NULL
select cast(@w2 as varchar)
go

select dbo.DataCompression(dbo.DataCompression(0x1234567890, 1), 0)
select dbo.DataCompression(null, 0)
go

select dbo.CalculateDataMD5(cast(N'aaaa' as varbinary(max)))
select dbo.CalculateDataMD5(null)
go

select dbo.CalculateDataSHA(cast(N'aaaa' as varbinary(max)))
select dbo.CalculateDataSHA(dbo.CalculateDataMD5(cast(N'aaaa' as varbinary(max))))
select dbo.CalculateDataSHA(null)
go

select cast(value as int) from dbo.Split('1,2,3,4,5,6,7,8,9', ',')

select * from dbo.SplitToInts('1,2,3,4,5,6,7,8,9,fsd,fsd,423,dfas,423423', ',') where value is not null
select * from dbo.SplitToInts(null, ',')
go

select dbo.MaxCommonDividor(968, 128)
select dbo.MaxCommonDividor(NULL, 128)
select dbo.MaxCommonDividor(NULL, NULL)
go

select * from dbo.DaysInMonths(2014)
select * from dbo.DaysInMonths(null)
go

select * from dbo.CreateSeries(3, 0, 1, 31)
select * from dbo.CreateSeries(3, 0, 1, 33)
go

select * from dbo.CreateRandomIntSeries(10, 100);
go

select dbo.GetSystemUserNameFromBinary(0x010100000000000512000000, 1);
select dbo.GetSystemUserNameFromBinary(0x0, 1);
go

select dbo.DataCompression(0x010100000000000512000000, 1)
go



/*
declare @id uniqueidentifier

set @id = NEWID()

select n, value, dbo.MovingAverage(@id, 20, value) from dbo.CreateSeries(1, 0, 1, 255)

exec dbo.ClearCache @id
*/

--- select dbo.GetSystemUserNameFromBinary(sid) from sys.syslogins

--- select dbo.GetSystemUserNameFromString(N'S-1-5-18')

/*
declare @id uniqueidentifier

set @id = NEWID()

select n, value, dbo.RollingSum(@id, value) from dbo.CreateSeries(1, 0, 1, 255)

exec dbo.ClearCache @i
*/

/*
declare @id uniqueidentifier

set @id = NEWID()

select n, value, dbo.EMA(@id, 5, value) from dbo.CreateSeries(1, 0, 1, 555)

exec dbo.ClearCache @id
*/

/*
declare @id uniqueidentifier

set @id = NEWID()

select n, value, dbo.PreviousValue(@id, value) from dbo.CreateSeries(1, 0, 1, 555)

exec dbo.ClearCache @id
*/

--declare @id uniqueidentifier

--set @id = NEWID()

--select n, value, dbo.ValueBack(@id, 10, value) from dbo.CreateSeries(1, 0, 1, 555)

--exec dbo.ClearCache @id



select * from sys.syslogins

