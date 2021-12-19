IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'door')
BEGIN
	CREATE TABLE [dbo].[door]
	(
		[Id] [bigint] NOT NULL,
		[Name] [nvarchar](30) NULL,
		[State] [tinyint] NOT NULL,
		[CreatedAt] [datetime2] NOT NULL,
		[UpdatedAt] [datetime2] NOT NULL,
		[IsDeactivated] [bit] NOT NULL,
		CONSTRAINT [PK_door] PRIMARY KEY CLUSTERED ([Id])
	)
END

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'door_access')
BEGIN
	CREATE TABLE [dbo].[door_access]
	(
		[DoorId] [bigint] NOT NULL,
		[UserId] [bigint] NOT NULL,
		[IsOwner] [bit] NOT NULL,
		[CreatedAt] [datetime2] NOT NULL,
		[UpdatedAt] [datetime2] NOT NULL,
		[IsDeactivated] [bit] NOT NULL,
		CONSTRAINT [PK_door_access] PRIMARY KEY CLUSTERED ([DoorId], [UserId]),
		CONSTRAINT [FK_door_access_door] FOREIGN KEY ([DoorId]) REFERENCES [dbo].[door] ([Id])
	)
END

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'door_event_log')
BEGIN
	CREATE TABLE [dbo].[door_event_log]
	(
		[DoorId] [bigint] NOT NULL,
		[UserId] [bigint] NOT NULL,
		[EventType] [tinyint] NOT NULL,
		[TimeStamp] [datetime2] NOT NULL,
		CONSTRAINT [FK_door_event_log_door] FOREIGN KEY ([DoorId]) REFERENCES [dbo].[door] ([Id])
	)
END