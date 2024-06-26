USE [master]
GO
CREATE LOGIN [InventoryUser] WITH PASSWORD=N'Pw123' MUST_CHANGE, DEFAULT_DATABASE=[inventory], CHECK_EXPIRATION=ON, CHECK_POLICY=ON
GO
ALTER SERVER ROLE [##MS_DatabaseConnector##] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [##MS_DatabaseManager##] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [##MS_DefinitionReader##] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [##MS_LoginManager##] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [##MS_PerformanceDefinitionReader##] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [##MS_SecurityDefinitionReader##] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [##MS_ServerPerformanceStateReader##] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [##MS_ServerSecurityStateReader##] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [##MS_ServerStateManager##] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [##MS_ServerStateReader##] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [bulkadmin] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [dbcreator] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [diskadmin] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [processadmin] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [securityadmin] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [serveradmin] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [setupadmin] ADD MEMBER [InventoryUser]
GO
ALTER SERVER ROLE [sysadmin] ADD MEMBER [InventoryUser]
GO
USE [inventory]
GO
CREATE USER [InventoryUser] FOR LOGIN [InventoryUser]
GO
