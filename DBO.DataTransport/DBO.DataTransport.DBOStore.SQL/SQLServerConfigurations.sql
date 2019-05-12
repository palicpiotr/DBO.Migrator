IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SQLServerConfigurations' and xtype='U')
	BEGIN

CREATE TABLE [SQLServerConfigurations] (
	[ConnectId] bigint NOT NULL IDENTITY,
	[ConfigId] bigint NOT NULL,
	[DataSource] varchar(100) NOT NULL,
	[ProtectedPassword] varchar(250) NOT NULL,
	[UserId] varchar(250) NOT NULL,
	[Application] varchar(250) NULL,
	[IsSecurityIntegrated] bit DEFAULT(0) NOT NULL,
	[Provider] varchar(250) NOT NULL,
	[AsynchronousProcessing] bit DEFAULT(0) NOT NULL
	CONSTRAINT [PK_SQLServerConfigurations_ConnectId] PRIMARY KEY ([ConnectId])
);

END;