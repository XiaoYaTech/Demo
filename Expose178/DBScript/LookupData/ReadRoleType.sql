USE [Expose178Com]
GO
DECLARE @UID nvarchar(50)
SELECT @UID = UserID FROM tbl_User WHERE UserEmail='Admin@expose178.com'

exec usp_AddUpdateReadRoleType @ReadRoleTypeDesc='����',  @UpdatedByUserID =@UID
exec usp_AddUpdateReadRoleType @ReadRoleTypeDesc='������',  @UpdatedByUserID =@UID
