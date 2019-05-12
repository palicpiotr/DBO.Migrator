IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ActionTypes' and xtype='U')
	BEGIN

CREATE TABLE [ActionTypes] (
	[TypeId] tinyint NOT NULL IDENTITY,
	[Name] nvarchar(50) NOT NULL,
	[Description] varchar(max) NULL,
	CONSTRAINT [PK_ActionTypes_TypeId] PRIMARY KEY ([TypeId])
);

END;