USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetRandNum')
BEGIN
	DROP PROCEDURE usp_GetRandNum
END
GO

CREATE PROCEDURE usp_GetRandNum
@RandNum nvarchar(30) out
AS
BEGIN
DECLARE @randomChar varchar(30)
DECLARE @randomChar1 varchar(30)
DECLARE @randomChar2 varchar(30)
DECLARE @randomChar3 varchar(30)

set @randomChar=convert(varchar(40),NEWID())
set @randomChar1=REPLACE(LEFT(@randomChar,8),'-','')
set @randomChar2=REPLACE(RIGHT(@randomChar,8),'-','')
set @randomChar3=RIGHT(CONVERT(NVARCHAR(50),CONVERT(decimal,RAND(ABS(CHECKSUM(NEWID())))*1000000000000000000)),11)
set @RandNum=@randomChar1+@randomChar3+@randomChar2
END
