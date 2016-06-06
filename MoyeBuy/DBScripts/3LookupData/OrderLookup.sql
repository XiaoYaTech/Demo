USE [MoyeBuyComOrder]
GO

DELETE tbl_OrderState
GO

INSERT INTO tbl_OrderState (OrderStateDesc,LastUpdatedDate) VALUES ('拍下商品',GETDATE())
INSERT INTO tbl_OrderState (OrderStateDesc,LastUpdatedDate) VALUES ('订单支付',GETDATE())
INSERT INTO tbl_OrderState (OrderStateDesc,LastUpdatedDate) VALUES ('商家发货',GETDATE())
INSERT INTO tbl_OrderState (OrderStateDesc,LastUpdatedDate) VALUES ('确认收货',GETDATE())
INSERT INTO tbl_OrderState (OrderStateDesc,LastUpdatedDate) VALUES ('评价',GETDATE())


------------
DELETE tbl_OrderTrack
GO

--INSERT INTO tbl_OrderTrack (OrderTrackStatus,LastUpdatedDate) VALUES ('',GETDATE())
