USE [MoyeBuyComOrder]
GO

DELETE tbl_OrderState
GO

INSERT INTO tbl_OrderState (OrderStateDesc,LastUpdatedDate) VALUES ('������Ʒ',GETDATE())
INSERT INTO tbl_OrderState (OrderStateDesc,LastUpdatedDate) VALUES ('����֧��',GETDATE())
INSERT INTO tbl_OrderState (OrderStateDesc,LastUpdatedDate) VALUES ('�̼ҷ���',GETDATE())
INSERT INTO tbl_OrderState (OrderStateDesc,LastUpdatedDate) VALUES ('ȷ���ջ�',GETDATE())
INSERT INTO tbl_OrderState (OrderStateDesc,LastUpdatedDate) VALUES ('����',GETDATE())


------------
DELETE tbl_OrderTrack
GO

--INSERT INTO tbl_OrderTrack (OrderTrackStatus,LastUpdatedDate) VALUES ('',GETDATE())
