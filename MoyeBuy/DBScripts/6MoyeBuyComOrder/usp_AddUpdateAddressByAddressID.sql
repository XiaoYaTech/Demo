USE [MoyeBuyComOrder]
GO

IF EXISTS (SELECT 1 FROM sys.all_objects WHERE NAME = 'usp_AddUpdateAddressByAddressID')
BEGIN
	DROP PROCEDURE usp_AddUpdateAddressByAddressID
END
GO

CREATE PROCEDURE usp_AddUpdateAddressByAddressID
@AddressID nvarchar(30)=null,
@Name nvarchar(20)=null,
@AddressLabel nvarchar(20)=null,
@Province nvarchar(50)=null,
@City nvarchar(50)=null,
@District nvarchar(50)=null,
@AddressDetail nvarchar(100)=null,
@ZipCode int=null,
@MobilePhone nvarchar(30)=null,
@TelPhone nvarchar(30)=null,
@UID nvarchar(30)=null
AS
BEGIN
	DECLARE @RandNum NVARCHAR(30)
	IF(EXISTS(SELECT 1 FROM tbl_Address WHERE AddressID=@AddressID))
	BEGIN
		SET @RandNum =@AddressID
		UPDATE tbl_Address SET Name=@Name,
		AddressLabel=@AddressLabel,
		Province=@Province,
		District=@District,
		City=@City,
		AddressDetail=@AddressDetail,
		ZipCode=@ZipCode,
		MobilePhone=@MobilePhone,
		TelPhone=@TelPhone,
		UpdateByUserID=@UID,
		LastUpdatedDate=GETDATE()
		WHERE AddressID=@AddressID
	END
	ELSE
	BEGIN
		EXEC MoyeBuyComServices.dbo.usp_GetRandNum @RandNum out
		set @RandNum='AdID'+@RandNum
		INSERT INTO tbl_Address(AddressID,Name,AddressLabel,Province,District,City,AddressDetail,ZipCode,MobilePhone,TelPhone,UpdateByUserID,LastUpdatedDate)
		VALUES(@RandNum,@Name,@AddressLabel,@Province,@District,@City,@AddressDetail,@ZipCode,@MobilePhone,@TelPhone,@UID,GETDATE())
	END
	
	IF(@@ERROR <> 0)
	BEGIN
		SELECT StatusCode=@@ERROR,StatusDesc='Fail to INSERT tbl_Address.' 
	END
	ELSE
	BEGIN
	IF(@@ERROR=0)
		SELECT AddressID=@RandNum ,StatusCode='0',StatusDesc='Success'
		return 0
	END
END
