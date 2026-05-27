USE VFilterMain;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='__Settings')
CREATE TABLE __Settings (SettingName NVARCHAR(200) PRIMARY KEY, Value NVARCHAR(500));
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='__ProxyServices')
CREATE TABLE __ProxyServices (ServiceId INT IDENTITY(1,1) PRIMARY KEY, Name NVARCHAR(100), ServerType INT, RemoteIP NVARCHAR(50), RemotePort INT, BindIP NVARCHAR(50), BindPort INT, ByteLimitation INT DEFAULT 0, AutoStart BIT DEFAULT 0);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_RefAchievement')
CREATE TABLE _RefAchievement (ID INT IDENTITY(1,1) PRIMARY KEY, Service BIT DEFAULT 1, Category TINYINT DEFAULT 0, Name NVARCHAR(200), RewardType TINYINT DEFAULT 0, RewardTitleID TINYINT DEFAULT 0, RewardSkillPoint INT DEFAULT 0, RewardGold BIGINT DEFAULT 0);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_RefAchievementCondition')
CREATE TABLE _RefAchievementCondition (ID INT IDENTITY(1,1) PRIMARY KEY, RefAchievementID INT, ConditionType NVARCHAR(100), ConditionValue NVARCHAR(200), TargetValue INT DEFAULT 0);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_RefAttendanceReward')
CREATE TABLE _RefAttendanceReward (ID INT IDENTITY(1,1) PRIMARY KEY, ItemID INT, ItemCodeName128 NVARCHAR(200), ItemCount INT DEFAULT 1, DayCount INT);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_RefEventRegister')
CREATE TABLE _RefEventRegister (ID INT IDENTITY(1,1) PRIMARY KEY, Name NVARCHAR(200), Description NVARCHAR(500));
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_RefEventSchedule')
CREATE TABLE _RefEventSchedule (ID INT IDENTITY(1,1) PRIMARY KEY, EventName NVARCHAR(200), Day TINYINT, Time NVARCHAR(10));
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_RefNewItemMall')
CREATE TABLE _RefNewItemMall (ID INT IDENTITY(1,1) PRIMARY KEY, Service BIT DEFAULT 1, CategoryName NVARCHAR(100), Type TINYINT DEFAULT 0, CodeName128 NVARCHAR(200), ItemID INT, ItemCount INT DEFAULT 1, Silk INT, ShowInNewBest BIT DEFAULT 0, ItemIndex INT DEFAULT 0, ShowInNewBestIndex INT DEFAULT 0);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_RefNewAvatarMall')
CREATE TABLE _RefNewAvatarMall (ID INT IDENTITY(1,1) PRIMARY KEY, Service BIT DEFAULT 1, CategoryName NVARCHAR(100), CodeName128 NVARCHAR(200), ItemID INT, Silk INT);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_LuckySpinRewards')
CREATE TABLE _LuckySpinRewards (ID INT IDENTITY(1,1) PRIMARY KEY, ItemID INT, Amount INT DEFAULT 1, Rate INT DEFAULT 100);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_ItemChest')
CREATE TABLE _ItemChest (ID INT IDENTITY(1,1) PRIMARY KEY, CharID INT, ItemCodeName NVARCHAR(200), ItemID INT, Quantity INT DEFAULT 1, Date NVARCHAR(50), Type NVARCHAR(50), Plus TINYINT DEFAULT 0);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_RefRankCategories')
CREATE TABLE _RefRankCategories (ID INT IDENTITY(1,1) PRIMARY KEY, CategoryName NVARCHAR(200), Query NVARCHAR(MAX), IsEnabled BIT DEFAULT 1);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_UniqueHistory')
CREATE TABLE _UniqueHistory (ID INT IDENTITY(1,1) PRIMARY KEY, UniqueName NVARCHAR(200), KillerName NVARCHAR(200), KillDate NVARCHAR(50));
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_Scheduler')
CREATE TABLE _Scheduler (Idx INT IDENTITY(1,1) PRIMARY KEY, Name NVARCHAR(200), Day INT, Time NVARCHAR(10), Query NVARCHAR(MAX), IsEnabled BIT DEFAULT 1, Comment NVARCHAR(500));
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_AsyncFilterCommands')
CREATE TABLE _AsyncFilterCommands (ID INT IDENTITY(1,1) PRIMARY KEY, Command NVARCHAR(500), Status INT DEFAULT 1, CreatedAt DATETIME DEFAULT GETDATE());
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_Notice')
CREATE TABLE _Notice (ID INT IDENTITY(1,1) PRIMARY KEY, Message NVARCHAR(500), IsActive BIT DEFAULT 1, CreatedAt DATETIME DEFAULT GETDATE());
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='_bypassHwidbyIP')
CREATE TABLE _bypassHwidbyIP (ID INT IDENTITY(1,1) PRIMARY KEY, IP NVARCHAR(50), HWID NVARCHAR(200), Reason NVARCHAR(200));
GO

IF NOT EXISTS (SELECT * FROM __Settings WHERE SettingName='ServerName')
BEGIN
INSERT INTO __Settings VALUES ('ServerName', 'KitoGuard-S500');
INSERT INTO __Settings VALUES ('ServerIP', '81.24.15.62');
INSERT INTO __Settings VALUES ('MaxLevel', '140');
INSERT INTO __Settings VALUES ('EnablePvP', 'true');
INSERT INTO __Settings VALUES ('EnableBotProtection', 'true');
INSERT INTO __Settings VALUES ('EnableHWIDCheck', 'true');
INSERT INTO __Settings VALUES ('IP_LimitPerIP', '3');
INSERT INTO __Settings VALUES ('HWID_LimitPerHWID', '2');
INSERT INTO __Settings VALUES ('GoldCapRate', '10');
INSERT INTO __Settings VALUES ('SilkPerHour', '5');
INSERT INTO __Settings VALUES ('AlchemyMaxPlus', '14');
INSERT INTO __Settings VALUES ('AlchemyLuckyRate', '0');
INSERT INTO __Settings VALUES ('JobTradeEnabled', 'true');
INSERT INTO __Settings VALUES ('StallEnabled', 'true');
INSERT INTO __Settings VALUES ('ExchangeEnabled', 'true');
INSERT INTO __Settings VALUES ('DiscordWebhook', '');
INSERT INTO __Settings VALUES ('WebsiteURL', '');
INSERT INTO __Settings VALUES ('ShowOldUI', 'false');
INSERT INTO __Settings VALUES ('ShowNewUI', 'true');
INSERT INTO __Settings VALUES ('FixStuck', 'true');
INSERT INTO __Settings VALUES ('AutoNoticeDelay', '300');
INSERT INTO __Settings VALUES ('EventTimerInterval', '60');
INSERT INTO __Settings VALUES ('DBConnectionPool', '100');
INSERT INTO __Settings VALUES ('PvPKillReward', '100');
INSERT INTO __Settings VALUES ('CapeEnabled', 'true');
INSERT INTO __Settings VALUES ('BattleRoyaleEnabled', 'false');
INSERT INTO __Settings VALUES ('CharLevelLimit', '140');
INSERT INTO __Settings VALUES ('ExpRate', '100');
INSERT INTO __Settings VALUES ('SPRate', '100');
INSERT INTO __Settings VALUES ('GoldDropRate', '100');
INSERT INTO __Settings VALUES ('ItemDropRate', '100');
INSERT INTO __Settings VALUES ('DailyLoginReward', 'true');
INSERT INTO __Settings VALUES ('AttendanceEnabled', 'true');
INSERT INTO __Settings VALUES ('LuckySpinEnabled', 'true');
INSERT INTO __Settings VALUES ('MacroDetection', 'true');
INSERT INTO __Settings VALUES ('AutoBanMacro', 'false');
END
GO
