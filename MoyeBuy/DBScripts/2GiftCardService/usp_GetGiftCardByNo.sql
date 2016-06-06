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

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetGiftCardByNo')
BEGIN
	DROP PROCEDURE usp_GetGiftCardByNo
END
GO

CREATE PROCEDURE usp_GetGiftCardByNo
@GiftCardNo nvarchar(100)=null,
@PageSize int=300,  
@PageIndex int=1,
@IsAsc bit=1, 
@SortField nvarchar(100)='GiftCardNo'
WITH ENCRYPTION
AS
BEGIN
DECLARE @SQL1 nvarchar(max)  
DECLARE @SQLFrom nvarchar(max)  
DECLARE @SQLWhere nvarchar(max)  

	OPEN SYMMETRIC KEY GiftCardSymmetricKeyPwd
	DECRYPTION BY PASSWORD= 'MoyeBuy.Com'
	
	SET @SQL1='
	SELECT gc.GiftCardNoID,
			gc.GiftCardNo,
			CONVERT(NVARCHAR(30),DecryptByKey(gc.GifCardPwdHash)) AS GifCardPwd,
			gc.GiftCardAmount,
			gc.StartDate,
			gc.ExpireDate,
			gc.UpdateByUserID,
			gc.LastUpdatedDate,
			CASE ISNULL(gci.GiftCardInvalidID,''0'') WHEN ''0'' THEN 0 ELSE 1 END AS IsInvalid,
			gci.GiftCardInvalidID'
	SET @SQLFrom='
	 FROM tbl_GiftCard gc
	 LEFT JOIN tbl_GiftCardInvalid gci
	 ON gc.GiftCardNo = gci.GiftCardNo'
	 
	SET @SQLWhere=' WHERE 1=1 '
	IF @GiftCardNo IS NOT NULL
	BEGIN
		SET @SQLWhere=@SQLWhere+' AND gc.GiftCardNo ='+@GiftCardNo
		EXEC (@SQL1+@SQLFrom+@SQLWhere)
	END
	ELSE
	BEGIN
		DECLARE @SQL2 nvarchar(max)  
		SET @SQL2 = ' ;  
		DECLARE @totalPage int  
		DECLARE @totalrows int  

		SELECT  
		@totalPage = CEILING(count(*)/'+ CONVERT(nvarchar, @PageSize) +'.),  
		@totalrows = count(*)  
		FROM #TempResult  

		DECLARE @PageSize int  
		DECLARE @PageIndex int  

		SET  @PageSize='+ CONVERT(nvarchar,@PageSize) + '  
		SET  @PageIndex=' + CONVERT(nvarchar,@PageIndex) + '  

		IF @PageIndex > @totalPage  
		BEGIN  
		SET @PageIndex = 1  
		END  
		;  
		WITH FinalResult AS  
		(  
		SELECT  
		@totalPage as TotalPages,  
		@totalrows as TotalRows,  
		 * ,  
		ROW_NUMBER() OVER ( ORDER BY '  
						+ RIGHT(@SortField, LEN(@SortField) - PATINDEX('%.%', @SortField))  
			+ CASE @IsAsc WHEN '0' THEN ' DESC ' ELSE '' END + ') AS Row  
		FROM #TempResult  
		)  
		SELECT * FROM finalResult  
		WHERE 1=1  '  
		
		--now add the page condition  
		IF @PageIndex  > 0  
		BEGIN  
			SET @SQL2 = @SQL2 + '  
			AND Row BETWEEN  
			(@PageSize * (@PageIndex - 1) + 1) and (@PageSize * @PageIndex) '  
		END  
		SET @SQL2 = @SQL2 + ' ORDER BY Row ASC '  
		DECLARE @S nvarchar(max)  
		SET @S = @SQL1 + ' INTO #TempResult ' + @SQLFrom +  @SQLWhere + @SQL2
		
		--print @S
		EXEC (@S)
	END
	 CLOSE SYMMETRIC KEY GiftCardSymmetricKeyPwd 
END



