SELECT Id,RefID,ReceiverAccount,ProcInstID,TypeCode,Url,Status,ActivityName FROM dbo.TaskWork WHERE TypeCode='Renewal_Analysis' AND Status=0

SELECT * FROM dbo.TaskWork WHERE RefID='MajorLease15030301' 

SELECT * FROM dbo.MajorLeaseConsInvtChecking WHERE ProjectId='MajorLease15030301' 

SELECT * FROM dbo.AttachmentRequirement WHERE Id='daa2badf-b2ee-4a83-a57e-913634390d14'

SELECT * FROM dbo.ReimageSummary WHERE IsHistory=1

SELECT * FROM dbo.ProjectInfo WHERE ProjectId='Reimage15030201'

SELECT * FROM dbo.MajorLeaseInfo WHERE ProjectId='MajorLease15013001'

SELECT * FROM dbo.FinancialPreanalysis ORDER BY Id DESC

SELECT * FROM dbo.ProjectUsers

SELECT * FROM dbo.ProjectNode WHERE ProjectId='MajorLease15021303'


SELECT * FROM dbo.StoreBasicInfo WHERE NameZHCN='兴宁第二分店'



