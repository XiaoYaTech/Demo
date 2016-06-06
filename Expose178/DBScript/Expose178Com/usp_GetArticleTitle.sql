USE [Expose178Com]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetArticleTitle')
BEGIN
	DROP PROCEDURE usp_GetArticleTitle
END
GO

CREATE PROCEDURE usp_GetArticleTitle 
@AritcleTypeCode nvarchar(30) = null,
@UpdatedByUserID nvarchar(30) = null,
@SortField nvarchar(50)='ArticleDate', 
@IsAsc bit=1,  
@IsReturnAll bit=0,
@PageSize int=300,  
@PageIndex int=1,
@ArticleSummary int =100
AS
BEGIN
	DECLARE @SQL1 nvarchar(max)  
	DECLARE @SQLFrom nvarchar(max)  
	DECLARE @SQLWhere nvarchar(max)  
	
	IF @AritcleTypeCode IS NOT NULL
	BEGIN
		SET @SQL1 ='SELECT art.ArticleTile,
				art.ArticleID,
				art.ArticleDate,
				SUBSTRING(ArticleBody,1,'+CONVERT(nvarchar,@ArticleSummary)+') as ArticleSummary,
			   artadd.AdditionalID,
			   artadd.ReadNum,
			   artadd.ReplyNum'
		SET @SQLFrom='
			   FROM tbl_Article art INNER JOIN tbl_ArticleAdditional artadd
			ON art.ArticleID = artadd. ArticleID'
		SET @SQLWhere='
			WHERE art.AritcleTypeCode =''' + ISNULL(@AritcleTypeCode,'art.AritcleTypeCode') +'''
			AND art.UpdatedByUserID = '+ISNULL(@UpdatedByUserID,'art.UpdatedByUserID')
	END
	ELSE
	BEGIN
	SET @SQL1 ='SELECT art.ArticleTile,
				art.ArticleID,
				art.ArticleDate,
				SUBSTRING(ArticleBody,1,'+CONVERT(nvarchar,@ArticleSummary)+') as ArticleSummary,
			   artadd.AdditionalID,
			   artadd.ReadNum,
			   artadd.ReplyNum'
	SET @SQLFrom='
			   FROM tbl_Article art INNER JOIN tbl_ArticleAdditional artadd
			ON art.ArticleID = artadd. ArticleID'
	
		IF(@UpdatedByUserID IS NOT NULL)	
		BEGIN
			SET @SQLWhere='
				WHERE art.UpdatedByUserID =''' + @UpdatedByUserID +''''
		END
		ELSE
		BEGIN
			SET @SQLWhere='
				WHERE art.AritcleTypeCode = art.AritcleTypeCode'
		END
	END

	IF @IsReturnAll = 1 
	BEGIN
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
END
