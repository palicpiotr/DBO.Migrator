
ALTER TABLE DBOTransportHistory
ADD CONSTRAINT FK_DBOTransportHistory_ActionTypes FOREIGN KEY(ActionTypeId)
	REFERENCES ActionTypes(TypeId)

ALTER TABLE DBOTransportHistory
ADD CONSTRAINT FK_DBOTransportHistory_Projects FOREIGN KEY(ProjectId)
	REFERENCES Projects(ProjectId)

ALTER TABLE PostgreSQLConfigurations
ADD CONSTRAINT FK_PostgreSQLConfigurations_RDBMSConfigurations FOREIGN KEY(ConfigId)
	REFERENCES RDBMSConfigurations(ConfigId)

ALTER TABLE SQLServerConfigurations
ADD CONSTRAINT FK_QLServerConfigurations_RDBMSConfigurations FOREIGN KEY(ConfigId)
	REFERENCES RDBMSConfigurations(ConfigId)

ALTER TABLE RDBMSConfigurations
ADD CONSTRAINT FK_RDBMSConfigurations_Projects FOREIGN KEY(ProjectId)
	REFERENCES Projects(ProjectId)

ALTER TABLE ProjectRDBMSRelationships
ADD CONSTRAINT FK_ProjectRDBMSRelationships_SupportedRDBMS_Owner FOREIGN KEY(RDBMSOwnerId)
	REFERENCES SupportedRDBMS(RDBMSId)

ALTER TABLE ProjectRDBMSRelationships
ADD CONSTRAINT FK_ProjectRDBMSRelationships_SupportedRDBMS_Consumer FOREIGN KEY(RDBMSConsumerId)
	REFERENCES SupportedRDBMS(RDBMSId)

ALTER TABLE ProjectRDBMSRelationships
ADD CONSTRAINT FK_ProjectRDBMSRelationships_Projects FOREIGN KEY(ProjectId)
	REFERENCES Projects(ProjectId)