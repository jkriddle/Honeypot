IF OBJECT_ID('BuildVersions') IS NOT NULL  
  DROP TABLE dbo.BuildVersions

CREATE TABLE dbo.BuildVersions(
	BuildVersionID [int] IDENTITY(1,1) NOT NULL,
	BuildLabel varchar(20) NOT NULL,
	BuildDate datetime CONSTRAINT [df_BuildVersion_BuildDate] DEFAULT getdate() NOT NULL,
	CheckSumConstraint bigint NOT NULL,
	CheckSumTableViewColumn bigint NOT NULL,
	CheckSumRoutine bigint NOT NULL
,
PRIMARY KEY CLUSTERED 
(
	[BuildVersionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY])



INSERT INTO BuildVersions (BuildLabel, CheckSumConstraint, CheckSumTableViewColumn, 
CheckSumRoutine)
VALUES ('1.0.0',  0, 0, 0);