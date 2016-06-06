-- ================================================
-- Template generated from Template Explorer using:
-- Create Trigger TRG_TaskWork_Update.SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- See additional Create Trigger templates for more
-- examples of different Trigger statements.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If Exists(Select 1 From sys.triggers Where name='TRG_TaskWork_Update')
    Drop  Trigger TRG_TaskWork_Update 
Go

-- =============================================
-- Author:		Victor.Huang
-- Create date: 2014-08-08
-- Description:	The Trigger for update taskwork refer data
-- =============================================
CREATE TRIGGER TRG_TaskWork_Update
   ON  TaskWork
   AFTER UPDATE
AS
if update([Status])
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
	DECLARE @projectId nvarchar(50), 
			@flowCode nvarchar(50),
			@taskStatus int,	-- Taskwork 状态
			@unFinishCount int,	-- 已完成的子流程和
			@createrAccount nvarchar(50),
			@returnCode nvarchar(50),
			@message nvarchar(max)


	SELECT @taskStatus = [Status],
	@projectId = RefID,
	@flowCode = TypeCode,
	@createrAccount = ReceiverAccount
	FROM inserted

	if ( @taskStatus = 200 )
	Begin
	begin try
	BEGIN TRANSACTION


		--更新子流程
		UPDATE  ProjectInfo
		SET LastUpdateTime = GETDATE(),
		NodeCode = 'Finish',NodeNameENUS = 'Finish',NodeNameZHCN='完成',[Status] =2
		WHERE ProjectId = @projectId AND FlowCode = @flowCode

		set @returnCode = '200'
		set @message = '更新项目 ' + @projectId +  '的子流程 ' + @flowCode + ' 成功'

		--获取未完成的子项目
		SELECT @unFinishCount = COUNT(1) FROM ProjectInfo
		WHERE ProjectId = @projectId AND [Status] =0

		--如果没有未完成的子项目更新主流程状态为完成
		IF(@unFinishCount = 0)
		BEGIN
			UPDATE  p
			SET p.LastUpdateTime = GETDATE(),
				p.NodeCode = 'Finish',
				p.NodeNameENUS = 'Finish',
				p.NodeNameZHCN='完成',
				p.[Status] =2
			From ProjectInfo p, FlowInfo f
			WHERE p.ProjectId = @projectId AND p.FlowCode = f.ParentCode AND f.Code = @flowCode

			set @returnCode = '201'
			set @message = '更新项目 ' + @projectId +  '的子流程 ' + @flowCode + ' 成功,并且整支项目正常结束'
		End

		INSERT INTO [SysApplicationLog]
			   ([ProjectId]
			   ,[LogType]
			   ,[FlowCode]
			   ,[CreateUser]
			   ,[CreateFunction]
			   ,[ReturnCode]
			   ,[ReturnMessage])
		 VALUES
			   (@projectId
			   ,'Info'
			   ,@flowCode
			   ,@createrAccount
			   ,'TaskWork.Triggers.TRG_TaskWork_Update'
			   ,@returnCode
			   ,@message)

	COMMIT TRAN
	end try
	begin catch
		rollback tran

		Set @returnCode = '-200'
		set @message= '更新项目状态失败,ProjectId=' + @projectId  + ', FlowCode=' + @flowCode + '. Error:' + ERROR_MESSAGE() 
        

		INSERT INTO [SysApplicationLog]
			   ([ProjectId]
			   ,[LogType]
			   ,[FlowCode]
			   ,[CreateUser]
			   ,[CreateFunction]
			   ,[ReturnCode]
			   ,[ReturnMessage])
		 VALUES
			   (@projectId
			   ,'Error'
			   ,@flowCode
			   ,@createrAccount
			   ,'TaskWork.Triggers.TRG_TaskWork_Update'
			   ,@returnCode
			   ,@message)

		print ERROR_MESSAGE()
	end catch
	End -- End if( @taskStatus = 200 )

END
GO
