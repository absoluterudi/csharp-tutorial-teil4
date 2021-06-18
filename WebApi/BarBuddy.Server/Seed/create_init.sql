
SET IDENTITY_INSERT [dbo].[Locations] ON 

INSERT [dbo].[Locations] ([Id], [BarName], [Street], [AddressAddition], [PostalCode], [City], [Country], [Phone], [GooglePlusCode], [GeoLocation], [QRCodeSalt], [Owner_FirstName], [Owner_LastName], [Credentials_Login], [Credentials_PasswordHash], [IsActive], [CreatedBy], [CreationDate], [ModifiedBy], [ModificationDate]) VALUES (1, N'Absolute Software', N'Jungfernstieg 49', NULL, N'20354', N'Hamburg', N'DE', N'040 202020', NULL, 0xE6100000010C8B1A96500BC74A40D3DA34B6D7FA2340, N'021F937410EC4961B12120C7E4C8BB34', N'Susann', N'Karras', N'susi@absolute.de', N'$2a$11$78B4w1CY64crLACSasMKeevClBc6dATIYxZBaRp3pNHEccvgNmKbe', 1, 0, CAST(N'2021-05-11T10:15:48.8978013' AS DateTime2), 0, CAST(N'2021-05-11T12:55:13.3510439' AS DateTime2))

SET IDENTITY_INSERT [dbo].[Locations] OFF

SET IDENTITY_INSERT [dbo].[LocationSpots] ON 

INSERT [dbo].[LocationSpots] ([Id], [Name], [Description], [AreaType], [SpotType], [MaxPersons], [LocationId], [CreatedBy], [CreationDate], [ModifiedBy], [ModificationDate]) VALUES (1, N'Tisch 1', NULL, 0, 0, 4, 1, 0, CAST(N'2021-05-11T10:15:48.8982602' AS DateTime2), 0, CAST(N'2021-05-11T10:37:23.4556161' AS DateTime2))

INSERT [dbo].[LocationSpots] ([Id], [Name], [Description], [AreaType], [SpotType], [MaxPersons], [LocationId], [CreatedBy], [CreationDate], [ModifiedBy], [ModificationDate]) VALUES (2, N'Tisch 2', NULL, 0, 0, 4, 1, 0, CAST(N'2021-05-11T10:15:48.8982694' AS DateTime2), 0, CAST(N'2021-05-11T10:37:23.4556358' AS DateTime2))

SET IDENTITY_INSERT [dbo].[LocationSpots] OFF