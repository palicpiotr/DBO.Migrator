IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PostgreSQLConfigurations' and xtype='U')
	BEGIN

CREATE TABLE [PostgreSQLConfigurations] (
	[ConnectId] bigint NOT NULL IDENTITY,
	[ConfigId] bigint NOT NULL,
	[UserId] varchar(100) NOT NULL,
	[ProtectedPassword] varchar(250) NOT NULL,
	[Host] varchar(250) NOT NULL,
	[Port] int NOT NULL,
	[Database] varchar(250) NOT NULL,
	[Pooling] bit NULL,
	[MinPoolSize] tinyint NULL,
	[MaxPoolSize] tinyint NULL,
	[ConnectionLifetime] tinyint NULL
	CONSTRAINT [PK_PostgreSQLConfigurations_ConnectId] PRIMARY KEY ([ConnectId])
);

END;