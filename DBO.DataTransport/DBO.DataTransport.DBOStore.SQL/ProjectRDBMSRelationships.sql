IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ProjectRDBMSRelationships' and xtype='U')
	BEGIN

CREATE TABLE [ProjectRDBMSRelationships] (
	[RelationId] bigint NOT NULL IDENTITY,
	[ProjectId] bigint NOT NULL,
	[RDBMSOwnerId] tinyint NOT NULL,
	[RDBMSConsumerId] tinyint NOT NULL,
	[CreatedDate] datetime NOT NULL,
	[UpdatedDate] datetime NULL,
	CONSTRAINT [PK_ProjectRDBMSRelationships_RelationId] PRIMARY KEY ([RelationId])
);

END;