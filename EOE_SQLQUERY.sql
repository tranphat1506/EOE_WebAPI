-- Tạo cơ sở dữ liệu
CREATE DATABASE EOEDB
GO

-- Sử dụng cơ sở dữ liệu
USE EOEDB
GO

-- Tạo bảng Account
CREATE TABLE [Account] (
    [AccountId] INT IDENTITY(1,1) PRIMARY KEY, -- Khóa chính cho bảng Account
    [Email] NVARCHAR(64) NOT NULL,
    [Password] NVARCHAR(64) NOT NULL,
    [CreatedAt] DATETIME DEFAULT GETDATE(),
    -- Ràng buộc duy nhất
    CONSTRAINT UN_Unique_Email UNIQUE (Email),
    -- Kiểm tra định dạng email hợp lệ
    CONSTRAINT CK_Valid_Email CHECK (Email LIKE '%_@__%.__%')
)
GO

-- Tạo bảng User với khóa chính hỗn hợp (UserId, AccountId)
CREATE TABLE [User] (
    [UserId] INT IDENTITY(1,1) PRIMARY KEY, -- Tự tăng UserId
    [AccountId] INT NOT NULL, -- Tham chiếu đến Account
    [DisplayName] NVARCHAR(30) NOT NULL,
    [Birth] DATETIME NULL,
    [Sex] BIT NULL CHECK (Sex IN (0,1)), -- 0: female, 1: male
    -- Thiết lập khóa ngoại từ User đến Account
    CONSTRAINT FK_User_Account FOREIGN KEY (AccountId) REFERENCES [Account](AccountId)
)
GO

-- Tạo bảng Game
CREATE TABLE [Game] (
    [GameId] INT IDENTITY(1,1) PRIMARY KEY,
    [Game_Name] NVARCHAR(100) NOT NULL,
    [Game_Desc] NVARCHAR(1000) NOT NULL,
    [Game_Image] NVARCHAR(100) NULL
)
GO

-- Tạo bảng GameScore với khóa ngoại từ Game và User
CREATE TABLE [GameScore] (
    [ScoreId] INT IDENTITY(1,1) PRIMARY KEY,
    [GameId] INT NOT NULL,
    [Score] INT NOT NULL,
    [UserId] INT NOT NULL,
    [PlayerName] NVARCHAR(100) NULL,
    -- Cột computed cho DefaultPlayerName
    [DefaultPlayerName] AS (CASE WHEN PlayerName IS NULL THEN N'Player' + CAST(ScoreId AS NVARCHAR(10)) ELSE PlayerName END),
    -- Thiết lập khóa ngoại từ GameScore đến Game
    CONSTRAINT FK_GameScore_Game FOREIGN KEY (GameId) REFERENCES [Game](GameId),
    -- Thiết lập khóa ngoại từ GameScore đến User (dựa trên composite key)
    CONSTRAINT FK_GameScore_User FOREIGN KEY (UserId) REFERENCES [User](UserId)
)
GO
-- Create Media Table
CREATE TABLE [Medias](
	[MediaId] varchar(16) NOT NULL PRIMARY KEY,
	[MediaName] nvarchar(100) NOT NULL,
	[MediaType] varchar(5) NOT NULL,
	[MediaUrl] nvarchar(500) NOT NULL,
	[Tags] nvarchar(MAX) NOT NULL DEFAULT N'',

	CONSTRAINT CK_CheckMediaId CHECK ([MediaId] LIKE ('M%')),
	CONSTRAINT CK_CheckMediaType CHECK ([MediaType] IN ('gif','image','video')),
)
GO
-- Create default profile image
INSERT INTO [Medias]([MediaId], [MediaName], MediaType, MediaUrl, Tags) VALUES 
(
	'M000000000000001',
	N'Default Profile Image', 
	'gif',
	N'DefaultProfileImage.gif', 
	N'avatar gif profile_image profile-image profile image'
)
GO
-- Add ProfileImageId to table Users
ALTER TABLE [User] ADD [ProfileImageId] varchar(16) NULL
GO
-- Add FK for ProfileImageId to Medias
ALTER TABLE [User] ADD CONSTRAINT FK_User_Medias FOREIGN KEY (ProfileImageId) REFERENCES [Medias](MediaId)
GO
-- Add default value for profile image id
ALTER TABLE [User] ADD CONSTRAINT DF_ProfileImageIdValue DEFAULT ('M000000000000001') FOR [ProfileImageId]
GO
-- Update profile image for all user
UPDATE [User]
SET [ProfileImageId] = 'M000000000000001';
GO
-------------------------------------------------------------------
-- // ADD GameImageId to Table Game
ALTER TABLE [Game] ADD [GameImageId] varchar(16) NULL;
GO
-- Add FK for GameImageId to Medias
ALTER TABLE [Game] ADD CONSTRAINT FK_Game_Medias FOREIGN KEY (GameImageId) REFERENCES [Medias](MediaId);
GO
-- // Create Emotion Wall Game Image
INSERT INTO [Medias]([MediaId], [MediaName], MediaType, MediaUrl, Tags) VALUES 
(
	'M000000000000002',
	N'Emotion Wall Game', 
	'image',
	N'EmotionWallGame.jpg', 
	N'emotion wall game image emotion_wall_game'
);
-- // ADD EMOTION DETECTIVE GAME
GO
INSERT INTO [Game](Game_Name, Game_Desc, GameImageId) VALUES
(
	N'Bức Tường Cảm Xúc',
	N'Vượt qua các bức tường bằng cách thể hiện cảm xúc trên khuôn mặt của mình sao cho giống với trên bức tường.',
	'M000000000000002'
);
-------------------------------------------------------------------
GO
-- Sử dụng cơ sở dữ liệu
USE EOEDB
GO
-- Test
SELECT [g].[GameId], [g].[GameImageId], [g].[Game_Desc], [g].[Game_Image], [g].[Game_Name], [m].[MediaId], [m].[MediaName], [m].[MediaType], [m].[MediaUrl], [m].[Tags]
      FROM [Game] AS [g]
      INNER JOIN [Medias] AS [m] ON [g].[GameImageId] = [m].[MediaId]