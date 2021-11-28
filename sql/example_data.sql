USE [MyApp]
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([User_Id], [Username], [Password]) VALUES (1, N'iamball', N'12345678')
INSERT [dbo].[User] ([User_Id], [Username], [Password]) VALUES (2, N'admin', N'admin')
SET IDENTITY_INSERT [dbo].[User] OFF
GO
INSERT [dbo].[User_Role] ([User_Id], [Role]) VALUES (1, N'user')
INSERT [dbo].[User_Role] ([User_Id], [Role]) VALUES (2, N'admin')
GO
INSERT [dbo].[Refresh_Token] ([Refresh_Token_Id], [Refresh_Token], [Username], [Issued_At_Utc], [Expired_At_Utc]) VALUES (N'c322f8d3873a4626a749669ff6832ebf', N'aUeMOephMfs8vdP29zWFWHcAur69SttmOyQd6Q7plY0rGaaf2I280ycHINbqS5bVcyOwa2WKKVDNwJhd--8HJC6611_lH5mrh1QNm1VviOWECXaPT5EoldvcsugycvQc61GHheBoglBvI0rY52jVYBvngSg_-3TQsqmR-X-bnpHv_Ioz3AqB6bIwPpmXiGwQdPthWTNP2ALJX2_qJ0ebHrCe8ArwZ9y44oVL0vLE87u3-w3vwQ39TrpqLCQFXKd2srQTyH9195CcYdg2o23Cii0w8WkcQJhAA7YyadgLvdDZfY3ksvywTANYPMXNrG0I9FvvY8Nc4dYcuV3NxV3xzA', N'iamball', CAST(N'2021-11-28T09:29:33.350' AS DateTime), CAST(N'2021-11-28T10:39:33.350' AS DateTime))
GO
INSERT [dbo].[Token_Audience] ([Client_Id], [Client_Secret], [Description]) VALUES (N'WebApp', N'WebAppSecret12345', N'Web App Backend for test token-based auth')
GO
