IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SupportedRDBMS' and xtype='U')
	BEGIN

CREATE TABLE [SupportedRDBMS] (
	[RDBMSId] tinyint NOT NULL IDENTITY,
	[Name] nvarchar(100) NOT NULL	
	CONSTRAINT [PK_SupportedRDBMS_RDBMSId] PRIMARY KEY ([RDBMSId])
);

END;