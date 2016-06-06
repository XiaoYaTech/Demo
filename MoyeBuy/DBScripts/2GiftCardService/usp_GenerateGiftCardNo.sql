USE [GiftCardService]
GO
IF NOT EXISTS(SELECT 1 FROM sys.symmetric_keys WHERE NAME = 'GiftCardSymmetricKeyPwd')
BEGIN

	CREATE MASTER KEY ENCRYPTION BY PASSWORD ='MoyeBuy.Com'

	CREATE SYMMETRIC KEY GiftCardSymmetricKeyPwd
	AUTHORIZATION GiftCardService
	WITH ALGORITHM = RC4
	ENCRYPTION BY PASSWORD = 'MoyeBuy.Com'
END
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GenerateGiftCardNo')
BEGIN
	DROP PROCEDURE usp_GenerateGiftCardNo
END
GO

CREATE PROCEDURE usp_GenerateGiftCardNo
@CardNum int,
@CardAmount decimal,
@CardPreNo nvarchar(10),
@PwdLenth int = 6,
@CardNoLenth int = 5,
@StartDate datetime,
@ExpireDate datetime,
@UID nvarchar(30)
WITH ENCRYPTION
AS
BEGIN
DECLARE @RandNum NVARCHAR(30)
DECLARE @RandCardNo nvarchar(30)
DECLARE @tmpPwd nvarchar(30)
DECLARE @RandCardPWD varbinary(max)
DECLARE @SqlStrRandCardNo nvarchar(max)
DECLARE @SqlStrtmpPwd nvarchar(max)
declare @ParmDefinition nvarchar(max);

CREATE TABLE #TableCard
(
	GiftCardNoID nvarchar(30),
	GiftCardNo nvarchar(100),
	GifCardPwdHash varbinary(max),
	GiftCardAmount decimal,
	UpdateByUserID nvarchar(30),
	StartDate datetime,
	[ExpireDate] datetime,
	LastUpdatedDate datetime default(GETDATE())
)

OPEN SYMMETRIC KEY GiftCardSymmetricKeyPwd
DECRYPTION BY PASSWORD= 'MoyeBuy.Com'
	
	WHILE(@CardNum<>0)
	BEGIN
	EXEC MoyeBuyComServices.dbo.usp_GetRandNum @RandNum out
	SET @RandNum='Gift'+@RandNum
		
	SET @ParmDefinition = N'@RandCardNoOut nvarchar(30) OUTPUT'
	SET @SqlStrRandCardNo ='SET @RandCardNoOut = CONVERT(NVARCHAR('+CONVERT(nvarchar(15),@CardNoLenth)+'),RIGHT(CONVERT(decimal,RAND(ABS(CHECKSUM(NEWID())))*100000000000000000),'+CONVERT(nvarchar(15),@CardNoLenth)+'))'
	EXEC SP_EXECUTESQL @SqlStrRandCardNo,@ParmDefinition,@RandCardNoOut = @RandCardNo OUTPUT
	SET @RandCardNo = @CardPreNo + @RandCardNo
	
	SET @ParmDefinition = N'@tmpPwdOut nvarchar(30) OUTPUT'
	SET @SqlStrtmpPwd =N'SET @tmpPwdOut = CONVERT(nvarchar('+CONVERT(nvarchar(15),@PwdLenth)+'),RIGHT(CONVERT(int,ABS(CHECKSUM(NEWID()))),'+CONVERT(nvarchar(15),@PwdLenth)+'))'
	EXEC SP_EXECUTESQL @SqlStrtmpPwd,@ParmDefinition,@tmpPwdOut = @tmpPwd OUTPUT
	SET @RandCardPWD = EncryptByKey(Key_GUID('GiftCardSymmetricKeyPwd'),@tmpPwd)
	
	--SET @RandCardNo = CONVERT(NVARCHAR(100),CONVERT(decimal,RAND(ABS(CHECKSUM(NEWID())))*100000000000000000))
	--SET @tmpPwd = CONVERT(nvarchar(30),RIGHT(CONVERT(int,ABS(CHECKSUM(NEWID()))),8))
	
	IF NOT EXISTS(SELECT 1 FROM #TableCard WHERE GiftCardNo = @RandCardNo) 
	   AND 
	   NOT EXISTS(SELECT 1 FROM tbl_GiftCard WHERE GiftCardNo = @RandCardNo)
		BEGIN
			INSERT INTO #TableCard(
					GiftCardNoID,
					GiftCardNo,
					GifCardPwdHash,
					GiftCardAmount,
					StartDate,
					[ExpireDate],
					UpdateByUserID
					)
					VALUES(
					@RandNum,
					@RandCardNo,
					@RandCardPWD,
					@CardAmount,
					@StartDate,
					@ExpireDate,
					@UID)
			SET @CardNum = @CardNum - 1
		END
		
	END
--SELECT *,CONVERT(nVARCHAR(30),DecryptByKey(GifCardPwdHash)) FROM #TableCard
CLOSE SYMMETRIC KEY GiftCardSymmetricKeyPwd

	INSERT INTO tbl_GiftCard (
	GiftCardNoID,
	GiftCardNo,
	GifCardPwdHash,
	GiftCardAmount,
	UpdateByUserID,
	StartDate,
	[ExpireDate],
	LastUpdatedDate)
	SELECT * FROM #TableCard
	DROP TABLE #TableCard
	
	IF @@ERROR <> 0
	BEGIN
		SELECT @@ERROR AS StatusCode,  'System Error' AS StatusDesc
	END
	ELSE
	BEGIN
		SELECT StatusCode='0',StatusDesc='Success'
	END
END


