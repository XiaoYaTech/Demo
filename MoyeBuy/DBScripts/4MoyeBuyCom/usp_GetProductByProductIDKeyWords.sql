USE [MoyeBuyCom]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_GetProductByProductIDKeyWords')
BEGIN
	DROP PROCEDURE usp_GetProductByProductIDKeyWords
END
GO

CREATE PROCEDURE usp_GetProductByProductIDKeyWords
@FilterString nvarchar(4000) = NULL,  
@ProductIDs nvarchar(max) = NULL,
@SortField nvarchar(100)='ProductName', 
@IsAsc bit=1,  
@IsReturnAll bit=0,
@PageSize int=300,  
@PageIndex int=1,  
@MoyeBuyPriceMin decimal = NULL,  
@MoyeBuyPriceMax decimal = NULL  

AS
BEGIN
DECLARE @SQL1 nvarchar(max)  
DECLARE @SQLFrom nvarchar(max)  
DECLARE @SQLWhere nvarchar(max)  
	
	CREATE TABLE #ProductID
	(
		ProductID nvarchar(30)
	)
	IF @ProductIDs IS NOT NULL
	BEGIN
		DECLARE @iCount_ProductID int  
		DECLARE @ilength_ProductID int  
		  --start of format  
		SET @iCount_ProductID = 1  
		
		WHILE(@iCount_ProductID <> 0)
		BEGIN
			SET @iCount_ProductID = PATINDEX('%|||%',@ProductIDs)  
			SET @ilength_ProductID = CASE @iCount_ProductID WHEN 0 THEN DATALENGTH(@ProductIDs) ELSE @iCount_ProductID-1 END  

			DECLARE @ProductID nvarchar(30)  

			SET @ProductID = SUBSTRING(@ProductIDs,1,@ilength_ProductID)  
			--  
			INSERT INTO #ProductID (ProductID) VALUES (@ProductID)  
			
			SET @ProductIDs = SUBSTRING(@ProductIDs,@iCount_ProductID+3,DATALENGTH(@ProductIDs))  
		END
		--select * from #ProductID
	END
	ELSE
	BEGIN
		DECLARE @SQL nvarchar(max)  
		SET @SQL='INSERT INTO #ProductID  
		SELECT p.ProductID FROM tbl_Product p
		INNER JOIN tbl_ProductStore ps ON p.ProductID = ps.ProductID
		INNER JOIN tbl_Supplier supp ON supp.SupplierID = ps.SupplierID
		WHERE ('+ISNULL(@FilterString,'1=1')+')'  
		EXEC (@SQL)  
	END
	
	SET @SQL1='
	SELECT p.*,
			pcat.CategoryName,
			pcat.CategoryDesc,
			ps.ProductCount,
			spp.SupplierName,
			spp.SupplierPersonName,
			spp.SupplierAddress,
			spp.SupplierID,
			spp.SupplierPhoneNo,
			spp.SupplierFax'
	SET @SQLFrom='
			FROM tbl_Product p 
			INNER JOIN #ProductID tp ON p.ProductID = tp.ProductID
			RIGHT JOIN tbl_ProductCategory pcat ON p.CategoryID = pcat.CategoryID
			INNER JOIN tbl_ProductStore ps ON ps.ProductID = p.ProductID
			INNER JOIN tbl_Supplier spp ON spp.SupplierID = ps.SupplierID'
	SET @SQLWhere =' WHERE 1=1 '		
	IF @MoyeBuyPriceMin IS NOT NULL AND @MoyeBuyPriceMax IS NOT NULL
	BEGIN
		
		SET @SQLWhere= @SQLWhere +'AND ISNULL(p.MoyeBuyPrice,0)>='+@MoyeBuyPriceMin
				+'AND ISNULL(p.MoyeBuyPrice,0)<='+@MoyeBuyPriceMax
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
