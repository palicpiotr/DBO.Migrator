IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Projects' and xtype='U')
	BEGIN

CREATE TABLE [Projects] (
	[ProjectId] bigint NOT NULL IDENTITY,
	[Name] nvarchar(max) NOT NULL,
	[Type] tinyint NOT NULL,
	[OwnerId] nvarchar(450) NOT NULL,
	[CreationDate] datetime NOT NULL,
	[UpdateDate] datetime NOT NULL,
	[Description] nvarchar(max) NULL
	CONSTRAINT [PK_Projects_ProjectId] PRIMARY KEY ([ProjectId])
);

END;