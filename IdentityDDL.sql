START TRANSACTION;
CREATE TABLE IF NOT EXISTS `AspNetRoles` (
    `Id` nvarchar(450) NOT NULL,
    `Name` nvarchar(256) NULL,
    `NormalizedName` nvarchar(256) NULL,
    `ConcurrencyStamp` nvarchar(512) NULL,
    CONSTRAINT `PK_AspNetRoles` PRIMARY KEY (`Id`)
);

CREATE TABLE IF NOT EXISTS `AspNetUsers` (
    `UserName` nvarchar(256) NOT NULL,
    `first_name` nvarchar(64) NOT NULL,
    `last_name` nvarchar(64) NOT NULL,
    `Id` nvarchar(512) NULL,
    `NormalizedUserName` nvarchar(256) NULL,
    `Email` nvarchar(256) NULL,
    `NormalizedEmail` nvarchar(256) NULL,
    `EmailConfirmed` bit NOT NULL,
    `PasswordHash` nvarchar(512) NULL,
    `SecurityStamp` nvarchar(512) NULL,
    `ConcurrencyStamp` nvarchar(512) NULL,
    `PhoneNumber` nvarchar(512) NULL,
    `PhoneNumberConfirmed` bit NOT NULL,
    `TwoFactorEnabled` bit NOT NULL,
    `LockoutEnd` datetime NULL,
    `LockoutEnabled` bit NOT NULL,
    `AccessFailedCount` int NOT NULL,
    CONSTRAINT `PK_AspNetUsers` PRIMARY KEY (`UserName`)
);

CREATE TABLE IF NOT EXISTS `AspNetRoleClaims` (
    `Id` int NOT NULL,
    `RoleId` nvarchar(450) NOT NULL,
    `ClaimType` nvarchar(512) NULL,
    `ClaimValue` nvarchar(512) NULL,
    CONSTRAINT `PK_AspNetRoleClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `AspNetUserClaims` (
    `Id` int NOT NULL,
    `UserId` nvarchar(256) NOT NULL,
    `ClaimType` nvarchar(512) NULL,
    `ClaimValue` nvarchar(512) NULL,
    CONSTRAINT `PK_AspNetUserClaims` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`UserName`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `AspNetUserLogins` (
    `LoginProvider` nvarchar(128) NOT NULL,
    `ProviderKey` nvarchar(128) NOT NULL,
    `ProviderDisplayName` nvarchar(512) NULL,
    `UserId` nvarchar(256) NOT NULL,
    CONSTRAINT `PK_AspNetUserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`UserName`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `AspNetUserRoles` (
    `UserId` nvarchar(256) NOT NULL,
    `RoleId` nvarchar(450) NOT NULL,
    CONSTRAINT `PK_AspNetUserRoles` PRIMARY KEY (`UserId`, `RoleId`),
    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`UserName`) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS `AspNetUserTokens` (
    `UserId` nvarchar(256) NOT NULL,
    `LoginProvider` nvarchar(128) NOT NULL,
    `Name` nvarchar(128) NOT NULL,
    `Value` nvarchar(512) NULL,
    CONSTRAINT `PK_AspNetUserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`UserName`) ON DELETE CASCADE
);

INSERT INTO `AspNetRoles` (`Id`, `ConcurrencyStamp`, `Name`, `NormalizedName`)
VALUES ('1', NULL, 'User', 'USER'),
('2', NULL, 'Kitchen', 'KITCHEN'),
('3', NULL, 'Administrator', 'ADMINISTRATOR');

CREATE INDEX `IX_AspNetRoleClaims_RoleId` ON `AspNetRoleClaims` (`RoleId`);

CREATE UNIQUE INDEX `RoleNameIndex` ON `AspNetRoles` (`NormalizedName`);

CREATE INDEX `IX_AspNetUserClaims_UserId` ON `AspNetUserClaims` (`UserId`);

CREATE INDEX `IX_AspNetUserLogins_UserId` ON `AspNetUserLogins` (`UserId`);

CREATE INDEX `IX_AspNetUserRoles_RoleId` ON `AspNetUserRoles` (`RoleId`);

CREATE INDEX `EmailIndex` ON `AspNetUsers` (`NormalizedEmail`);

CREATE UNIQUE INDEX `IX_AspNetUsers_UserName` ON `AspNetUsers` (`UserName`);

CREATE UNIQUE INDEX `UserNameIndex` ON `AspNetUsers` (`NormalizedUserName`);

COMMIT;

