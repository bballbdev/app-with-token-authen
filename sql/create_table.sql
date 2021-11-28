
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/28/2021 17:06:40
-- Generated from EDMX file: D:\Playground\AppWithTokenAuthen\AppWithTokenAuthen\Database\MyAppEntities.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [MyApp];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_User_Role_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[User_Role] DROP CONSTRAINT [FK_User_Role_User];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Refresh_Token]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Refresh_Token];
GO
IF OBJECT_ID(N'[dbo].[Token_Audience]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Token_Audience];
GO
IF OBJECT_ID(N'[dbo].[User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User];
GO
IF OBJECT_ID(N'[dbo].[User_Role]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_Role];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'User'
CREATE TABLE [dbo].[User] (
    [User_Id] int IDENTITY(1,1) NOT NULL,
    [Username] varchar(200)  NOT NULL,
    [Password] varchar(200)  NOT NULL
);
GO

-- Creating table 'Token_Audience'
CREATE TABLE [dbo].[Token_Audience] (
    [Client_Id] varchar(200)  NOT NULL,
    [Client_Secret] varchar(200)  NOT NULL,
    [Description] varchar(max)  NULL
);
GO

-- Creating table 'User_Role'
CREATE TABLE [dbo].[User_Role] (
    [User_Id] int  NOT NULL,
    [Role] varchar(20)  NOT NULL
);
GO

-- Creating table 'Refresh_Token'
CREATE TABLE [dbo].[Refresh_Token] (
    [Refresh_Token_Id] varchar(50)  NOT NULL,
    [Refresh_Token1] varchar(max)  NOT NULL,
    [Username] varchar(200)  NOT NULL,
    [Issued_At_Utc] datetime  NOT NULL,
    [Expired_At_Utc] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [User_Id] in table 'User'
ALTER TABLE [dbo].[User]
ADD CONSTRAINT [PK_User]
    PRIMARY KEY CLUSTERED ([User_Id] ASC);
GO

-- Creating primary key on [Client_Id] in table 'Token_Audience'
ALTER TABLE [dbo].[Token_Audience]
ADD CONSTRAINT [PK_Token_Audience]
    PRIMARY KEY CLUSTERED ([Client_Id] ASC);
GO

-- Creating primary key on [User_Id], [Role] in table 'User_Role'
ALTER TABLE [dbo].[User_Role]
ADD CONSTRAINT [PK_User_Role]
    PRIMARY KEY CLUSTERED ([User_Id], [Role] ASC);
GO

-- Creating primary key on [Refresh_Token_Id], [Username] in table 'Refresh_Token'
ALTER TABLE [dbo].[Refresh_Token]
ADD CONSTRAINT [PK_Refresh_Token]
    PRIMARY KEY CLUSTERED ([Refresh_Token_Id], [Username] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [User_Id] in table 'User_Role'
ALTER TABLE [dbo].[User_Role]
ADD CONSTRAINT [FK_User_Role_User]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[User]
        ([User_Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------