IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DBOTransportHistory' and xtype='U')
	BEGIN

CREATE TABLE [DBOTransportHistory] (
	[Id] bigint NOT NULL IDENTITY,
	[ActionTypeId] tinyint NOT NULL,
	[ProjectId] bigint NOT NULL,
	[Notes] varchar(max) NULL,
	CONSTRAINT [PK_DBOTransportHistory_Id] PRIMARY KEY ([Id])
);

END;