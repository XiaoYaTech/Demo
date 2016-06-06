USE [Expose178Com]
GO
DECLARE @UID nvarchar(50)
SELECT @UID = UserID FROM tbl_User WHERE UserEmail='Admin@expose178.com'

exec usp_AddUpdateAritcleType @AritcleTypeDesc='医疗行业',@UpdatedByUserID=@UID
exec usp_AddUpdateAritcleType @AritcleTypeDesc='食品行业',@UpdatedByUserID=@UID
exec usp_AddUpdateAritcleType @AritcleTypeDesc='销售行业',@UpdatedByUserID=@UID
exec usp_AddUpdateAritcleType @AritcleTypeDesc='公司企业',@UpdatedByUserID=@UID
