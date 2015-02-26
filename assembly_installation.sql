set nocount on

exec sp_configure 'Show Advanced Options', 1
reconfigure
go

exec sp_configure 'CLR Enable', 1
reconfigure
go

if exists(select * from sys.types where name = 'MacAddressType')
	drop type dbo.MacAddressType
go

if exists(select * from sys.objects where name = 'Concatenate' and type = 'AF')
	drop aggregate Concatenate
go

if exists(select * from sys.objects where name = 'Xor' and type = 'AF')
	drop aggregate Xor
go

if exists(select * from sys.objects where name = 'And' and type = 'AF')
	drop aggregate [And]
go

if exists(select * from sys.objects where name = 'Or' and type = 'AF')
	drop aggregate [Or]
go

if exists(select * from sys.objects where name = 'GeometricMean' and type = 'AF')
	drop aggregate GeometricMean
go

if exists(select * from sys.objects where name = 'HarmonicMean' and type = 'AF')
	drop aggregate HarmonicMean
go

if exists(select * from sys.objects where name = 'MatchesPattern' and type = 'FS')	
	drop function dbo.MatchesPattern
go

if exists(select * from sys.objects where name = 'MinDT2' and type = 'FS')	
	drop function dbo.MinDT2
go

if exists(select * from sys.objects where name = 'CreateDate' and type = 'FS')	
	drop function dbo.CreateDate
go

if exists(select * from sys.objects where name = 'CreateDateExt' and type = 'FS')	
	drop function dbo.CreateDateExt
go

if exists(select * from sys.objects where name = 'CharAt' and type = 'FS')	
	drop function dbo.CharAt
go

if exists(select * from sys.objects where name = 'MaxDT2' and type = 'FS')	
	drop function dbo.MaxDT2
go

if exists(select * from sys.objects where name = 'MinDT3' and type = 'FS')	
	drop function dbo.MinDT3
go

if exists(select * from sys.objects where name = 'MaxDT3' and type = 'FS')	
	drop function dbo.MaxDT3
go

if exists(select * from sys.objects where name = 'ApplyLimitsInt' and type = 'FS')	
	drop function dbo.ApplyLimitsInt
go

if exists(select * from sys.objects where name = 'ApplyLimitsDateTime' and type = 'FS')	
	drop function dbo.ApplyLimitsDateTime
go

if exists(select * from sys.objects where name = 'ApplyLimitsByte' and type = 'FS')	
	drop function dbo.ApplyLimitsByte
go

if exists(select * from sys.objects where name = 'ApplyLimitsDouble' and type = 'FS')	
	drop function dbo.ApplyLimitsDouble
go

if exists(select * from sys.objects where name = 'LevenshteinDistance' and type = 'FS')	
	drop function dbo.LevenshteinDistance
go

if exists(select * from sys.objects where name = 'SwitchCase' and type = 'FS')	
	drop function dbo.SwitchCase
go

if exists(select * from sys.objects where name = 'CalculateDataMD5' and type = 'FS')	
	drop function dbo.CalculateDataMD5
go

if exists(select * from sys.objects where name = 'CalculateDataHash' and type = 'FS')	
	drop function dbo.CalculateDataHash
go

if exists(select * from sys.objects where name = 'CalculateDataSHA' and type = 'FS')	
	drop function dbo.CalculateDataSHA
go

if exists(select * from sys.objects where name = 'DataCompression' and type = 'FS')	
	drop function dbo.DataCompression
go

if exists(select * from sys.objects where name = 'HexBufferToString' and type = 'FS')	
	drop function dbo.HexBufferToString
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

if exists(select * from sys.objects where name = 'CalculateCharacters' and type = 'FT')	
	drop function dbo.CalculateCharacters
go

if exists(select * from sys.objects where name = 'BasicPivot' and type = 'PC')	
	drop procedure dbo.BasicPivot
go

if exists(select * from sys.objects where name = 'CalculateDataHash' and type = 'PC')	
	drop procedure dbo.CalculateDataHash
go

if exists(select * from sys.objects where name = 'MaxCommonDividor' and type = 'FS')	
	drop function dbo.MaxCommonDividor
go

if exists(select * from sys.objects where name = 'ShiftRightBigint' and type = 'FS')	
	drop function dbo.ShiftRightBigint
go

if exists(select * from sys.objects where name = 'ShiftLeftBigint' and type = 'FS')	
	drop function dbo.ShiftLeftBigint
go

if exists(select * from sys.objects where name = 'RotateLeftBinary' and type = 'FS')	
	drop function dbo.RotateLeftBinary
go

if exists(select * from sys.objects where name = 'RotateRightBinary' and type = 'FS')	
	drop function dbo.RotateRightBinary
go

if exists(select * from sys.objects where name = 'ShiftLeftBinary' and type = 'FS')	
	drop function dbo.ShiftLeftBinary
go

if exists(select * from sys.objects where name = 'ShiftRightBinary' and type = 'FS')	
	drop function dbo.ShiftRightBinary
go

if exists(select * from sys.objects where name = 'CreateSeries' and type = 'FT')	
	drop function dbo.CreateSeries
go

if exists(select * from sys.objects where name = 'CreateRandomIntSeries' and type = 'FT')	
	drop function dbo.CreateRandomIntSeries
go

if exists(select * from sys.objects where name = 'GetSystemUserNameFromBinary' and type = 'FS')	
	drop function dbo.GetSystemUserNameFromBinary
go

if exists(select * from sys.objects where name = 'GetSystemUserNameFromString' and type = 'FS')	
	drop function dbo.GetSystemUserNameFromString
go

if exists(select * from sys.objects where name = 'Digits' and type = 'FT')	
	drop function dbo.Digits
go

if exists(select * from sys.assemblies where name = 'SqlToolset')
	drop assembly SqlToolset
go

create assembly SqlToolset from 'd:\GitHub\SqlToolset\bin\Debug\SqlToolset.dll'
with PERMISSION_SET = safe
go

create function Digits(@value bigint, @base smallint)
returns table 
(
			[value] bigint,
			[multiplier] bigint
)
external name [SqlToolset].[SqlToolset.MathFunctions].Digits
go

create aggregate Concatenate(@value nvarchar(4000))
returns nvarchar(4000)
external name [SqlToolset].[SqlToolset.Concatenate]
go

create aggregate Xor(@value bigint)
returns bigint
external name [SqlToolset].[SqlToolset.Aggregations.Xor]
go

create aggregate [And](@value bigint)
returns bigint
external name [SqlToolset].[SqlToolset.Aggregations.And]
go

create aggregate [Or](@value bigint)
returns bigint
external name [SqlToolset].[SqlToolset.Aggregations.Or]
go

create aggregate HarmonicMean(@value float(53))
returns float(53)
external name [SqlToolset].[SqlToolset.Aggregations.HarmonicMean]
go

create aggregate GeometricMean(@value float(53))
returns float(53)
external name [SqlToolset].[SqlToolset.Aggregations.GeometricMean]
go

create function MatchesPattern(@pattern nvarchar(4000), @value nvarchar(4000))
returns integer
external name [SqlToolset].[SqlToolset.StringOperations].[MatchesPattern]
go

create function MinDT2(@a datetime, @b datetime)
returns datetime
external name [SqlToolset].[SqlToolset.DateTimeOperations].MinDT2
go

create function CreateDate(@year smallint, @month tinyint, @day tinyint)
returns datetime
external name [SqlToolset].[SqlToolset.DateTimeOperations].CreateDate
go

create function CreateDateExt(@year smallint, @month tinyint, @day tinyint)
returns datetime
external name [SqlToolset].[SqlToolset.DateTimeOperations].CreateDateExt
go

create function CharAt(@a NVARCHAR(max), @index int)
returns NCHAR(1)
external name [SqlToolset].[SqlToolset.StringOperations].CharAt
go

create function MaxDT2(@a datetime, @b datetime)
returns datetime
external name [SqlToolset].[SqlToolset.DateTimeOperations].MaxDT2
go

create function MinDT3(@a datetime, @b datetime, @c datetime)
returns datetime
external name [SqlToolset].[SqlToolset.DateTimeOperations].MinDT3
go

create function MaxDT3(@a datetime, @b datetime, @c datetime)
returns datetime
external name [SqlToolset].[SqlToolset.DateTimeOperations].MaxDT3
go

create function ApplyLimitsDateTime(@input datetime, @min datetime, @max datetime)
returns datetime
external name [SqlToolset].[SqlToolset.DateTimeOperations].ApplyLimitsDateTime
go

create function ApplyLimitsInt(@input bigint, @min bigint, @max bigint)
returns bigint
external name [SqlToolset].[SqlToolset.MathFunctions].ApplyLimitsInt
go

create function ApplyLimitsByte(@input tinyint, @min tinyint, @max tinyint)
returns tinyint
external name [SqlToolset].[SqlToolset.MathFunctions].ApplyLimitsByte
go

create function ApplyLimitsDouble(@input float(53), @min float(53), @max float(53))
returns float(53)
external name [SqlToolset].[SqlToolset.MathFunctions].ApplyLimitsDouble
go

create function LevenshteinDistance(@pattern nvarchar(4000), @value nvarchar(4000))
returns integer
external name [SqlToolset].[SqlToolset.StringOperations].[LevenshteinDistance]
go

create function SwitchCase(@input nvarchar(4000))
returns nvarchar(4000)
external name [SqlToolset].[SqlToolset.StringOperations].[SwitchCase]
go

create function DaysInMonths(@year int)
returns table ([Month] int, [Days] int)
external name [SqlToolset].[SqlToolset.DateTimeOperations].DaysInMonths
go

create function CalculateDataMD5(@data varbinary(max))
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.BlobOperations].CalculateDataMD5
go

create function CalculateDataHash(@data varbinary(max), @hashName NVARCHAR(32))
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.BlobOperations].CalculateDataHash
go

create function CalculateDataSHA(@data varbinary(max))
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.BlobOperations].CalculateDataSHA
go

create function DataCompression(@blob varbinary(max), @compress bit)
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.BlobOperations].DataCompression
go

create function HexBufferToString(@blob varbinary(max))
returns nvarchar(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.BlobOperations].HexBufferToString
go

create function Split(@input nvarchar(max), @separators nvarchar(max))
returns table 
(
			[value] nvarchar(max)
)
external name [SqlToolset].[SqlToolset.StringOperations].Split
go

create function SplitToInts(@input nvarchar(max), @separators nvarchar(max))
returns table 
(
			[value] bigint
)
external name [SqlToolset].[SqlToolset.StringOperations].SplitToInts
go

create function CalculateCharacters(@input nvarchar(max))
returns table 
(
			[character] NCHAR(1),
			[count] INT
)
external name [SqlToolset].[SqlToolset.StringOperations].CalculateCharacters
go

create type [dbo].MacAddressType
external name [SqlToolset].[SqlToolset.Types.MacAddressType]
go

create procedure BasicPivot
@cmd nvarchar(max)
as
external name [SqlToolset].[SqlToolset.MiscelaneusTools].BasicPivot
go

create function MaxCommonDividor(@a bigint, @b bigint)
returns bigint
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.MathFunctions].MaxCommonDividor
go

create function ShiftLeftBigint(@a bigint, @b tinyint)
returns bigint
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.MathFunctions].ShiftLeftBigint
go

create function ShiftLeftBinary(@a varbinary(max), @b int)
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.MathFunctions].ShiftLeftBinary
go

create function RotateLeftBinary(@a varbinary(max), @b int)
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.MathFunctions].RotateLeftBinary
go

create function RotateRightBinary(@a varbinary(max), @b int)
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.MathFunctions].RotateRightBinary
go

create function ShiftRightBinary(@a varbinary(max), @b int)
returns varbinary(max)
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.MathFunctions].ShiftRightBinary
go

create function ShiftRightBigint(@a bigint, @b tinyint)
returns bigint
with RETURNS NULL ON NULL INPUT
external name [SqlToolset].[SqlToolset.MathFunctions].ShiftRightBigint
go

create function CreateSeries(@a int, @b int, @n1 int, @n2 int)
returns table ([n] int, [value] bigint)
external name [SqlToolset].[SqlToolset.Series].CreateSeries
go

create function CreateRandomIntSeries(@range int, @count int)
returns table ([n] int, [value] bigint)
external name [SqlToolset].[SqlToolset.Series].CreateRandomIntSeries
go

create function dbo.GetSystemUserNameFromBinary(@sid varbinary(max), @nullOnError bit)
returns sysname
as
external name [SqlToolset].[SqlToolset.MiscelaneusTools].GetSystemUserNameFromBinary
go

create function dbo.GetSystemUserNameFromString(@sid sysname)
returns sysname
as
external name [SqlToolset].[SqlToolset.MiscelaneusTools].GetSystemUserNameFromString
go

declare @w [dbo].MacAddressType
set @w = 'AABBCCFF9932'
select cast(@w as varchar)
select cast(@w as varbinary)

declare @w2 [dbo].MacAddressType
set @w2 = NULL
select cast(@w2 as varchar)
go

select 'DataCompression', dbo.DataCompression(dbo.DataCompression(0x1234567890, 1), 0)
select 'DataCompression', dbo.DataCompression(null, 0)
go

select 'CalculateDataMD5', dbo.CalculateDataMD5(cast(N'aaaa' as varbinary(max)))
select 'CalculateDataMD5', dbo.CalculateDataMD5(null)
go

select 'CalculateDataSHA', dbo.CalculateDataSHA(cast(N'aaaa' as varbinary(max)))
select 'CalculateDataSHA', dbo.CalculateDataSHA(dbo.CalculateDataMD5(cast(N'aaaa' as varbinary(max))))
select 'CalculateDataSHA', dbo.CalculateDataSHA(null)
go

select 'CalculateDataHash', dbo.CalculateDataHash(NULL, NULL)
select 'CalculateDataHash', dbo.CalculateDataHash(cast(N'aaaa' as varbinary(max)), NULL)
select 'CalculateDataHash', dbo.CalculateDataHash(cast(N'aaaa' as varbinary(max)), 'SHA512')
select 'CalculateDataHash', dbo.CalculateDataHash(cast(N'aaaa' as varbinary(max)), 'MD5')
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

select 'GetSystemUserNameFromBinary', dbo.GetSystemUserNameFromBinary(0x010100000000000512000000, 1);
select 'GetSystemUserNameFromBinary', dbo.GetSystemUserNameFromBinary(0x0, 1);
go

select 'DataCompression', dbo.DataCompression(0x010100000000000512000000, 1)
go

select * from dbo.Digits(123456789, 10)
select * from dbo.Digits(123456789, 8)
select * from dbo.Digits(0, 8)

select * from dbo.Digits(16, 16)
Go


select dbo.LevenshteinDistance(N'abcdefgh', N'abcdefg')
select dbo.LevenshteinDistance(N'bcdefgh', N'abcdefg')
select dbo.LevenshteinDistance(NULL, N'abcdefg')
select dbo.LevenshteinDistance(N'bcdefgh', NULL)
select dbo.LevenshteinDistance(N'bcdefgh', NULL)
select dbo.LevenshteinDistance(N'tron', N'trop')
go

select dbo.HexBufferToString(0x1234) as hex
select dbo.HexBufferToString(0x01234) as hex
select dbo.HexBufferToString(0x) as hex
select dbo.HexBufferToString(NULL) as hex
go

select * from dbo.CalculateCharacters('abcdefgh')
select * from dbo.CalculateCharacters('abbcccddddeeeeeffffff')
select * from dbo.CalculateCharacters(N'aaπππ•••')
select * from dbo.CalculateCharacters(N'')
select * from dbo.CalculateCharacters(NULL)
go

select dbo.MinDT2('2014-01-01 12:04:12', '2014-01-02 11:04:12')
select dbo.MinDT2('2014-01-01 12:04:12', NULL)

select dbo.MaxDT2('2014-01-01 12:04:12', '2014-01-02 11:04:12')
select dbo.MaxDT2('2014-01-01 12:04:12', NULL)
go

select dbo.MinDT3('2011-01-01 12:04:12', '2010-01-02 11:04:12', '2013-01-02 11:04:12')
select dbo.MinDT3('2010-01-01 12:04:12', '2012-01-02 11:04:12', '2013-01-02 11:04:12')
select dbo.MinDT3('2011-01-01 12:04:12', '2012-01-02 11:04:12', '2010-01-02 11:04:12')

select dbo.MaxDT3('2011-01-01 12:04:12', '2010-01-02 11:04:12', '2013-01-02 11:04:12')
select dbo.MaxDT3('2010-01-01 12:04:12', '2012-01-02 11:04:12', '2013-01-02 11:04:12')
select dbo.MaxDT3('2011-01-01 12:04:12', '2013-01-02 11:04:12', '2010-01-02 11:04:12')
go

select dbo.CharAt('abcdefghij', 0)
select dbo.CharAt('abcdefghij', 1)
select dbo.CharAt('abcdefghij', 2)
select dbo.CharAt('abcdefghij', 3)
select dbo.CharAt('abcdefghij', 4)
select dbo.CharAt('abcdefghij', 5)
select dbo.CharAt('abcdefghij', 6)


select dbo.CharAt('abcdefghij', 100)
select dbo.CharAt('abcdefghij', -1)
select dbo.CharAt('abcdefghij', NULL)
select dbo.CharAt(NULL, 1)
select dbo.CharAt('', 0)
GO

select dbo.HarmonicMean(num)
from
(
select 2 as num union all select 2 union all select 5 union all select 7
) as inn

select dbo.HarmonicMean(num)
from
(
select 2 AS num
) as inn

select dbo.HarmonicMean(num)
from
(
select NULL AS num
) as inn

select dbo.HarmonicMean(num)
from
(
select 2 as num union all select 2
) as inn

select dbo.HarmonicMean(num)
from
(
select 1 as num union all select 2 union all select 3 union all select 4
) as inn
GO

select dbo.GeometricMean(num)
from
(
select 2 as num union all select 2 union all select 5 union all select 7
) as inn

select dbo.GeometricMean(num)
from
(
select 0 as num union all select 2 union all select 5 union all select 7
) as inn

select dbo.GeometricMean(num)
from
(
select 4 as num union all select 9 union all select 16 union all select 69 union all select 42
) as inn
go

select dbo.ApplyLimitsInt(10, 20, 1000)
select dbo.ApplyLimitsInt(10, 2, 1000)
select dbo.ApplyLimitsInt(10, NULL, 1000)
select dbo.ApplyLimitsInt(10000, NULL, 1000)
select dbo.ApplyLimitsInt(NULL, NULL, 1000)
go

select dbo.ApplyLimitsDouble(10.5, 10, 1000)
select dbo.ApplyLimitsDouble(9.56789, 10, 1000)
select dbo.ApplyLimitsDouble(9.1234, NULL, 1000)
select dbo.ApplyLimitsDouble(10000, NULL, 1000)
select dbo.ApplyLimitsDouble(NULL, NULL, 1000)
go

select dbo.ApplyLimitsDateTime('2011-01-01 12:04:12', '2010-01-02 11:04:12', '2013-01-02 11:04:12')
select dbo.ApplyLimitsDateTime('2010-01-01 12:04:12', '2012-01-02 11:04:12', '2013-01-02 11:04:12')
select dbo.ApplyLimitsDateTime('1900-01-01 12:04:12', NULL, '2013-01-02 11:04:12')
go
/*
 * changed: 2015.01.17
 * description: adding new aggregations and datetime functions 
 */

select dbo.Xor(num)
from
(
select 4 as num union all select 9 union all select 16 union all select 69 union all select 42
) as inn
go

select dbo.[And](num)
from
(
select 6 as num union all select 10 --union all select 16 union all select 70 union all select 42 union all select 2
) as inn
go

select dbo.[Or](num)
from
(
select 4 as num union all select 9 --union all select 16 union all select 69 union all select 42
) as inn
go 

select dbo.SwitchCase(N'AbCdEfGhIjKl•ÊèÒ')
select dbo.SwitchCase(N'11111AbCdEfGhIjKl•ÊèÒ')
select dbo.SwitchCase(N'')
select dbo.SwitchCase(null)
go

select dbo.CreateDate(0,1,1)
select dbo.CreateDate(2014,12,12)
select dbo.CreateDate(2014,1,1)
select dbo.CreateDate(2014,2,28)
select dbo.CreateDate(0,0,0)
go

select dbo.CreateDateExt(2014, 12, 229)
go

select dbo.ShiftLeftBigint(2, 60)
select dbo.ShiftLeftBigint(2, 64)
go
select dbo.ShiftRightBigint(2, 65)
go

select dbo.ShiftRightBigint(dbo.ShiftLeftBigint(2324, 17), 17)
go

select dbo.ShiftLeftBinary(0x0001, 1)
select dbo.ShiftLeftBinary(0x0001, 4)
select dbo.ShiftLeftBinary(0x0001, 15)
go

select dbo.ShiftRightBinary(0x1000, 1)
select dbo.ShiftRightBinary(0x1000, 4)
select dbo.ShiftRightBinary(0x8000, 15)
go

select dbo.RotateLeftBinary(0x12345678, 1)
select dbo.RotateLeftBinary(0x12345678, 2)
select dbo.RotateLeftBinary(0x12345678, 3)
select dbo.RotateLeftBinary(0x12345678, 4)
select dbo.RotateLeftBinary(0x12345678, 1)
select dbo.RotateLeftBinary(dbo.RotateLeftBinary(0x12345678, 1), 3)
select dbo.RotateLeftBinary(0x12345678, 250)
select dbo.RotateRightBinary(dbo.RotateLeftBinary(0x12345678, 250), 250)
select dbo.RotateRightBinary(0x01, 2)
go