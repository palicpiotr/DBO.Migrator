IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DBConfigurations' and xtype='U')
	BEGIN

CREATE TABLE [RDBMSConfigurations] (
	[ConfigId] bigint NOT NULL IDENTITY,
	[ProjectId] bigint NOT NULL,
	[RDMBSId] tinyint NOT NULL,
	CONSTRAINT [PK_RDBMSConfigurations_ConfigId] PRIMARY KEY ([ConfigId])
);

END;