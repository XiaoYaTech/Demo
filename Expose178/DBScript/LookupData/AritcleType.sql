USE [Expose178Com]
GO
DECLARE @UID nvarchar(50)
SELECT @UID = UserID FROM tbl_User WHERE UserEmail='Admin@expose178.com'

exec usp_AddUpdateAritcleType @AritcleTypeDesc='ҽ����ҵ',@UpdatedByUserID=@UID
exec usp_AddUpdateAritcleType @AritcleTypeDesc='ʳƷ��ҵ',@UpdatedByUserID=@UID
exec usp_AddUpdateAritcleType @AritcleTypeDesc='������ҵ',@UpdatedByUserID=@UID
exec usp_AddUpdateAritcleType @AritcleTypeDesc='��˾��ҵ',@UpdatedByUserID=@UID
