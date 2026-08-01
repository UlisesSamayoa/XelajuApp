CREATE TABLE Users
(
    IdUser              INT IDENTITY PRIMARY KEY,
    Username            NVARCHAR(50) NOT NULL,
    PasswordHash        NVARCHAR(500) NOT NULL,
    FirstName           NVARCHAR(100) NOT NULL,
    LastName            NVARCHAR(100) NOT NULL,
    Email               NVARCHAR(150) NULL,
    IsActive            BIT NOT NULL DEFAULT(1),
    FailedAttempts      INT NOT NULL DEFAULT(0),
    LockedUntil         DATETIME NULL,
    LastLogin           DATETIME NULL,
    PasswordChangedDate DATETIME NOT NULL DEFAULT(GETDATE()),
    MustChangePassword BIT NOT NULL DEFAULT(1),
    CreatedDate         DATETIME NOT NULL DEFAULT(GETDATE()),
    CreatedBy           INT NULL
)


go

CREATE UNIQUE INDEX UX_Users_Username
ON Users(Username);



go

CREATE TABLE UserRoles
(
    IdUser INT NOT NULL,
    IdRole INT NOT NULL,
    PRIMARY KEY(IdUser,IdRole),
    FOREIGN KEY(IdUser) REFERENCES Users(IdUser),
    FOREIGN KEY(IdRole) REFERENCES Roles(IdRole)
)

go

CREATE TABLE Permissions
(
    IdPermission INT IDENTITY PRIMARY KEY,
    Module NVARCHAR(80),
    Action NVARCHAR(80),
    Description NVARCHAR(250)
)

go

CREATE TABLE RolePermissions
(
    IdRole INT,
    IdPermission INT,
    PRIMARY KEY(IdRole,IdPermission),
    FOREIGN KEY(IdRole) REFERENCES Roles(IdRole),
    FOREIGN KEY(IdPermission) REFERENCES Permissions(IdPermission)
)

go

CREATE TABLE LoginHistory
(
    IdLoginHistory INT IDENTITY PRIMARY KEY,
    IdUser INT,
    LoginDate DATETIME NOT NULL DEFAULT(GETDATE()),
    Success BIT NOT NULL,
    IpAddress NVARCHAR(50),
    Browser NVARCHAR(250),
    FOREIGN KEY(IdUser) REFERENCES Users(IdUser)
)

go

--=======================================================
--=======================================================
--             PROCEDIMIENTOS ALMACENADOS              --
--=======================================================
--=======================================================

CREATE PROC sp_GetUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.IdUser,
        U.Username,
        U.FirstName,
        U.LastName,
        U.Email,
        U.IsActive,
        U.LastLogin,
        U.MustChangePassword
    FROM Users U
    ORDER BY U.FirstName, U.LastName;
END

GO

CREATE PROC sp_GetUserById
(
    @IdUser INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdUser,
        Username,
        PasswordHash,
        FirstName,
        LastName,
        Email,
        IsActive,
        FailedAttempts,
        LockedUntil,
        LastLogin,
        PasswordChangedDate,
        CreatedDate,
        CreatedBy,
        MustChangePassword
    FROM Users
    WHERE IdUser = @IdUser;
END

GO

CREATE OR ALTER PROC sp_SaveUser
(
    @IdUser INT = NULL,
    @Username NVARCHAR(50),
    @PasswordHash NVARCHAR(500),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(150) = NULL,
    @IsActive BIT,
    @MustChangePassword BIT,
    @CreatedBy INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewId INT = @IdUser;

    -------------------------------------------------------
    -- Validate duplicate Username
    -------------------------------------------------------
    IF EXISTS
    (
        SELECT 1
        FROM Users
        WHERE Username = @Username
          AND (@IdUser IS NULL OR IdUser <> @IdUser)
    )
    BEGIN
        SELECT
            -1 AS Result,
            NULL AS IdUser,
            'Username already exists.' AS Message;

        RETURN;
    END

    -------------------------------------------------------
    -- INSERT
    -------------------------------------------------------
    IF @IdUser IS NULL
    BEGIN
        INSERT INTO Users
        (
            Username,
            PasswordHash,
            FirstName,
            LastName,
            Email,
            IsActive,
            FailedAttempts,
            LockedUntil,
            LastLogin,
            PasswordChangedDate,
            CreatedDate,
            CreatedBy,
            MustChangePassword
        )
        VALUES
        (
            @Username,
            @PasswordHash,
            @FirstName,
            @LastName,
            @Email,
            @IsActive,
            0,
            NULL,
            NULL,
            GETDATE(),
            GETDATE(),
            @CreatedBy,
            @MustChangePassword
        );

        SET @NewId = SCOPE_IDENTITY();
    END

    -------------------------------------------------------
    -- UPDATE
    -------------------------------------------------------
    ELSE
    BEGIN
        UPDATE Users
        SET
            Username = @Username,
            FirstName = @FirstName,
            LastName = @LastName,
            Email = @Email,
            IsActive = @IsActive,
            MustChangePassword = @MustChangePassword
        WHERE IdUser = @IdUser;
    END

    -------------------------------------------------------
    -- SUCCESS
    -------------------------------------------------------
    SELECT
        1 AS Result,
        @NewId AS IdUser,
        'User saved successfully.' AS Message;
END


GO

CREATE PROC sp_DeleteUser
(
    @IdUser INT
)
AS
BEGIN
    UPDATE Users
    SET
        IsActive = 0
    WHERE IdUser = @IdUser;
END

GO

CREATE PROC sp_ValidateUsername
(
    @Username NVARCHAR(50),
    @IdUser INT = NULL
)
AS
BEGIN
    SELECT COUNT(*)
    FROM Users
    WHERE Username = @Username
      AND (@IdUser IS NULL OR IdUser <> @IdUser);
END


GO

CREATE OR ALTER PROC sp_GetUserByUsername
(
    @Username NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        IdUser,
        Username,
        PasswordHash,
        FirstName,
        LastName,
        Email,
        IsActive,
        FailedAttempts,
        LockedUntil,
        LastLogin,
        PasswordChangedDate,
        MustChangePassword
    FROM Users
    WHERE Username = @Username;
END


GO

-- CREATE OR ALTER PROC sp_ProcessFailedLogin
-- (
--     @IdUser INT
-- )
-- AS
-- BEGIN
--     SET NOCOUNT ON;
--     DECLARE @MaxAttempts INT = 5;
--     DECLARE @LockMinutes INT = 30;
--     UPDATE Users
--     SET
--         FailedAttempts = FailedAttempts + 1,
--         LockedUntil =
--             CASE
--                 WHEN FailedAttempts + 1 >= @MaxAttempts
--                 THEN DATEADD(MINUTE, @LockMinutes, GETDATE())
--                 ELSE LockedUntil
--             END
--     WHERE IdUser = @IdUser;
-- END
-- GO
CREATE OR ALTER PROC sp_ProcessFailedLogin
(
    @IdUser INT
)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @MaxAttempts INT = 5;
    DECLARE @LockMinutes INT = 30;

    UPDATE Users
    SET
        FailedAttempts = FailedAttempts + 1,
        LockedUntil =
            CASE
                WHEN FailedAttempts + 1 >= @MaxAttempts
                THEN DATEADD(MINUTE, @LockMinutes, GETDATE())
                ELSE LockedUntil
            END
    WHERE IdUser = @IdUser;

    SELECT
        FailedAttempts,
        LockedUntil
    FROM Users
    WHERE IdUser = @IdUser;

END
GO


GO

CREATE OR ALTER PROC sp_ResetLoginAttempts
(
    @IdUser INT
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET
        FailedAttempts = 0,
        LockedUntil = NULL,
        LastLogin = GETDATE()
    WHERE IdUser = @IdUser;
END


GO

CREATE OR ALTER PROC sp_ChangePassword
(
    @IdUser INT,
    @PasswordHash NVARCHAR(500)
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users
    SET
        PasswordHash = @PasswordHash,
        MustChangePassword = 0,
        PasswordChangedDate = GETDATE(),
        FailedAttempts = 0,
        LockedUntil = NULL
    WHERE IdUser = @IdUser AND MustChangePassword = 1 AND IsActive = 1;
    SELECT @@ROWCOUNT AS RowsAffected;
END

GO

CREATE OR ALTER PROC sp_GetRoles
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdRole,
        Name,
        Description,
        IsActive,
        CreatedDate,
        CreatedBy,
        IsSystem
    FROM Roles
    ORDER BY Name;
END
GO

CREATE OR ALTER PROC sp_GetRoleById
(
    @IdRole INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP(1)
        IdRole,
        Name,
        Description,
        IsActive,
        CreatedDate,
        CreatedBy,
        IsSystem
    FROM Roles
    WHERE IdRole = @IdRole;
END
GO


CREATE OR ALTER PROC sp_SaveRole
(
    @IdRole INT = NULL,
    @Name NVARCHAR(50),
    @Description NVARCHAR(200) = NULL,
    @IsActive BIT,
    @CreatedBy INT = NULL
)
AS
BEGIN

    SET NOCOUNT ON;

    IF EXISTS
    (
        SELECT 1
        FROM Roles
        WHERE Name = @Name
          AND (@IdRole IS NULL OR IdRole <> @IdRole)
    )
    BEGIN
        SELECT
            -1 AS Result,
            'Role already exists.' AS Message;
        RETURN;
    END
    IF @IdRole IS NULL
    BEGIN
        INSERT INTO Roles
        (
            Name,
            Description,
            IsActive,
            CreatedDate,
            CreatedBy
        )
        VALUES
        (
            @Name,
            @Description,
            @IsActive,
            GETDATE(),
            @CreatedBy
        );
        SET @IdRole = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE Roles
        SET Name = @Name, Description = @Description, IsActive = @IsActive WHERE IdRole = @IdRole;
    END
    SELECT
        1 AS Result,
        'Role saved successfully.' AS Message;
END


GO


CREATE OR ALTER PROC sp_GetUserRoles
(
    @IdUser INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        R.IdRole,
        R.Name,
        R.Description
    FROM UserRoles UR
        INNER JOIN Roles R
            ON UR.IdRole = R.IdRole
    WHERE UR.IdUser = @IdUser
    ORDER BY R.Name;
END

GO

CREATE OR ALTER PROC sp_DeleteRole
(
    @IdRole INT
)
AS
BEGIN
    SET NOCOUNT ON;

    --------------------------------------------------------
    -- Validar que el rol exista
    --------------------------------------------------------
    IF NOT EXISTS
    (
        SELECT 1
        FROM Roles
        WHERE IdRole = @IdRole
    )
    BEGIN
        SELECT
            -1 AS Result,
            'Role not found.' AS Message;
        RETURN;
    END

    --------------------------------------------------------
    -- Validar si es un rol del sistema
    --------------------------------------------------------
    IF EXISTS
    (
        SELECT 1
        FROM Roles
        WHERE IdRole = @IdRole
          AND IsSystem = 1
    )
    BEGIN
        SELECT
            -2 AS Result,
            'System roles cannot be deleted.' AS Message;
        RETURN;
    END

    --------------------------------------------------------
    -- Validar si el rol está asignado a usuarios
    --------------------------------------------------------
    IF EXISTS
    (
        SELECT 1
        FROM UserRoles
        WHERE IdRole = @IdRole
    )
    BEGIN
        SELECT
            -3 AS Result,
            'The role is assigned to one or more users and cannot be deleted.' AS Message;
        RETURN;
    END

    --------------------------------------------------------
    -- Validar si el rol tiene permisos asignados
    --------------------------------------------------------
    IF EXISTS
    (
        SELECT 1
        FROM RolePermissions
        WHERE IdRole = @IdRole
    )
    BEGIN
        SELECT
            -4 AS Result,
            'The role has permissions assigned. Remove them before deleting the role.' AS Message;
        RETURN;
    END

    --------------------------------------------------------
    -- Eliminar
    --------------------------------------------------------
    DELETE FROM Roles
    WHERE IdRole = @IdRole;

    SELECT
        1 AS Result,
        'Role deleted successfully.' AS Message;
END


GO

ALTER TABLE Roles
ADD IsSystem BIT NOT NULL
CONSTRAINT DF_Roles_IsSystem DEFAULT(0);

GO

UPDATE Roles
SET IsSystem = 1
WHERE Name IN
(
    'Administrator',
    'Operator',
    'Supervisor',
    'Auditor'
);

select * from roles
insert into Roles (Name, Description, IsActive, CreatedDate, CreatedBy, IsSystem) values ('Administrator', 'System Administrator', 1, GETDATE(), NULL, 1);
insert into Roles (Name, Description, IsActive, CreatedDate, CreatedBy, IsSystem) values ('Operator', 'System Operator', 1, GETDATE(), NULL, 1);
insert into Roles (Name, Description, IsActive, CreatedDate, CreatedBy, IsSystem) values ('Supervisor', 'System Supervisor', 1, GETDATE(), NULL, 1);
insert into Roles (Name, Description, IsActive, CreatedDate, CreatedBy, IsSystem) values ('Auditor', 'System Auditor', 1, GETDATE(), NULL, 1);
insert into Roles (Name, Description, IsActive, CreatedDate, CreatedBy, IsSystem) values ('Auditor Out', 'System Auditor Out', 1, GETDATE(), NULL, 1);


go

CREATE OR ALTER PROC sp_SaveUserRoles
(
    @IdUser INT,
    @IdRoles NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    --------------------------------------------------------
    -- Validar que exista el usuario
    --------------------------------------------------------
    IF NOT EXISTS
    (
        SELECT 1
        FROM Users
        WHERE IdUser = @IdUser
    )
    BEGIN
        SELECT
            -1 AS Result,
            'User not found.' AS Message;
        RETURN;
    END

    --------------------------------------------------------
    -- Eliminar roles actuales
    --------------------------------------------------------
    DELETE
    FROM UserRoles
    WHERE IdUser = @IdUser;

    --------------------------------------------------------
    -- Si vienen roles, insertarlos
    --------------------------------------------------------
    IF NULLIF(LTRIM(RTRIM(@IdRoles)), '') IS NOT NULL
    BEGIN
        INSERT INTO UserRoles
        (
            IdUser,
            IdRole
        )
        SELECT
            @IdUser,
            CAST(value AS INT)
        FROM STRING_SPLIT(@IdRoles, ',');
    END

    --------------------------------------------------------
    -- Resultado
    --------------------------------------------------------
    SELECT
        1 AS Result,
        'User roles saved successfully.' AS Message;
END


GO

CREATE OR ALTER PROC sp_GetUserPermissions
(
    @IdUser INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT

        P.IdPermission,
        P.Module,
        P.Action,
        P.Description
    FROM UserRoles UR
        INNER JOIN RolePermissions RP ON UR.IdRole = RP.IdRole
        INNER JOIN Permissions P ON RP.IdPermission = P.IdPermission
    WHERE UR.IdUser = @IdUser
    ORDER BY P.Module,P.Action;

END
GO


CREATE OR ALTER PROC sp_GetPermissions
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPermission,
        Module,
        Action,
        Description
    FROM Permissions
    ORDER BY Module, Action;
END
GO

CREATE OR ALTER PROC sp_GetRolePermissions
(
    @IdRole INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.IdPermission,
        p.Module,
        p.Action,
        p.Description
    FROM RolePermissions rp
    INNER JOIN Permissions p
        ON p.IdPermission = rp.IdPermission
    WHERE rp.IdRole = @IdRole
    ORDER BY p.Module, p.Action;
END
GO

CREATE OR ALTER PROC sp_SaveRolePermissions
(
    @IdRole INT,
    @Permissions NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;


    DELETE FROM RolePermissions
    WHERE IdRole = @IdRole;


    IF NULLIF(@Permissions,'') IS NULL
    BEGIN
        SELECT
            1 AS Result,
            'Permissions removed successfully.' AS Message;

        RETURN;
    END


    INSERT INTO RolePermissions
    (
        IdRole,
        IdPermission
    )
    SELECT
        @IdRole,
        TRY_CAST(value AS INT)
    FROM STRING_SPLIT(@Permissions, ',')
    WHERE TRY_CAST(value AS INT) IS NOT NULL;


    SELECT
        1 AS Result,
        'Permissions saved successfully.' AS Message;

END
GO




select * from users
select * from userroles
select * from roles
select * from permissions where module = 'Transactions'
select * from permissions where module = 'DocumentsTypes'
select * from rolepermissions where idrole = 2

@using TransferApp.Extensions

insert into Permissions (Module, Action, Description) values ('Clients', 'View', 'Permission to view clients');

insert into rolepermissions (IdRole, IdPermission) values (1, 54);
insert into rolepermissions (IdRole, IdPermission) values (1, 55);
insert into rolepermissions (IdRole, IdPermission) values (1, 56);
insert into rolepermissions (IdRole, IdPermission) values (1, 57);

select * from roles
delete from roles where idRole = 5

SELECT
'INSERT INTO Roles
(
    Name,
    Description,
    IsActive,
    IsSystem
)
VALUES
(''' +
REPLACE(Name,'''','''''') + ''',''' +
ISNULL(REPLACE(Description,'''',''''''),'') + ''',' +
CAST(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END AS VARCHAR(1)) + ',' +
CAST(CASE WHEN IsSystem = 1 THEN 1 ELSE 0 END AS VARCHAR(1)) +
');'
FROM Roles

INSERT INTO Roles (Name,Description,IsActive,IsSystem ) VALUES ('Administrator','System Administrator',1,1);
INSERT INTO Roles (Name,Description,IsActive,IsSystem ) VALUES ('Operator','System Operator',1,1);
INSERT INTO Roles (Name,Description,IsActive,IsSystem ) VALUES ('Supervisor','System Supervisor',1,1);
INSERT INTO Roles (Name,Description,IsActive,IsSystem ) VALUES ('Auditor','System Auditor',1,1);

select * from permissions
select * from rolepermissions

SELECT
'INSERT INTO Permissions
(
    IdPermission,
    Module,
    Action,
    Description
)
VALUES
(''' +
REPLACE(IdPermission,'''','''''') + ''',''' +
REPLACE(Module,'''','''''') + ''',''' +
REPLACE(Action,'''','''''') + ''',''' +
ISNULL(REPLACE(Description,'''',''''''),'') +
''');'
FROM Permissions
ORDER BY IdPermission;


INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('1','Users','View','Permission to view users');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('2','Clients','View','Permission to view clients');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('3','Users','Create','Create users');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('4','Users','Edit','Edit users');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('5','Users','Delete','Delete users');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('6','Clients','Create','Create clients');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('7','Clients','Edit','Edit clients');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('8','Clients','Delete','Delete users');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('9','Transactions','View','Create Transactions');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('10','Transactions','Create','Create Transactions');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('11','Transactions','Edit','Edit Transactions');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('12','Transactions','Delete','Delete Transactions');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('13','ClientCompanies','View','Create ClientCompanies');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('14','ClientCompanies','Create','Create ClientCompanies');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('15','ClientCompanies','Edit','Edit ClientCompanies');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('16','ClientCompanies','Delete','Delete ClientCompanies');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('17','Beneficiaries','View','Create Beneficiaries');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('18','Beneficiaries','Create','Create Beneficiaries');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('19','Beneficiaries','Edit','Edit Beneficiaries');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('20','Beneficiaries','Delete','Delete Beneficiaries');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('21','Reports','View','Create Reports');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('22','Reports','Create','Create Reports');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('23','Reports','Edit','Edit Reports');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('24','Reports','Delete','Delete Reports');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('25','Reviews','View','Create Reviews');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('26','Reviews','Create','Create Reviews');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('27','Reviews','Edit','Edit Reviews');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('28','Reviews','Delete','Delete Reviews');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('29','TrainingsLicenses','View','Create TrainingsLicenses');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('30','TrainingsLicenses','Create','Create TrainingsLicenses');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('31','TrainingsLicenses','Edit','Edit TrainingsLicenses');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('32','TrainingsLicenses','Delete','Delete TrainingsLicenses');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('33','Countries','View','Create Countries');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('34','Countries','Create','Create Countries');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('35','Countries','Edit','Edit Countries');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('36','Countries','Delete','Delete Countries');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('37','Companies','View','Create Companies');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('38','Companies','Create','Create Companies');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('39','Companies','Edit','Edit Companies');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('40','Companies','Delete','Delete Companies');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('41','DocumentsTypes','View','Create DocumentsTypes');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('42','DocumentsTypes','Create','Create DocumentsTypes');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('43','DocumentsTypes','Edit','Edit DocumentsTypes');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('44','DocumentsTypes','Delete','Delete DocumentsTypes');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('45','Parameters','View','Create Parameters');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('46','Parameters','Create','Create Parameters');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('47','Parameters','Edit','Edit Parameters');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('48','Parameters','Delete','Delete Parameters');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('49','TransactionsTypes','View','Create TransactionsTypes');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('50','TransactionsTypes','Create','Create TransactionsTypes');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('51','TransactionsTypes','Edit','Edit TransactionsTypes');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('52','TransactionsTypes','Delete','Delete TransactionsTypes');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('53','Home','View','View Home');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('54','Roles','View','View Roles');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('55','Roles','Create','Create Roles');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('56','Roles','Edit','Edit Roles');
INSERT INTO Permissions (     IdPermission,     Module,     Action,     Description ) VALUES ('57','Roles','Delete','Delete Roles');


select * from rolepermissions

SELECT
'INSERT INTO RolePermissions
(
    IdRole,
    IdPermission
)
SELECT ' +
CAST(rp.IdRole AS VARCHAR(10)) +
', IdPermission
FROM Permissions
WHERE IdPermission = ''' +
REPLACE(p.IdPermission,'''','''''') +
''';'
FROM RolePermissions rp
INNER JOIN Permissions p
ON rp.IdPermission = p.IdPermission
ORDER BY rp.IdRole, p.IdPermission;

INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '1';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '2';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '3';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '4';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '5';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '6';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '7';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '8';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '9';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '10';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '11';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '12';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '13';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '14';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '15';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '16';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '17';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '18';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '19';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '20';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '21';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '22';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '23';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '24';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '25';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '26';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '27';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '28';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '29';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '30';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '31';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '32';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '33';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '34';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '35';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '36';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '37';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '38';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '39';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '40';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '41';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '42';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '43';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '44';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '45';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '46';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '47';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '48';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '49';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '50';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '51';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '52';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '53';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '54';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '55';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '56';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 1, IdPermission FROM Permissions WHERE IdPermission = '57';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '2';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '6';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '9';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '10';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '11';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '13';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '14';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '17';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '18';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '33';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '37';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '38';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '41';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '45';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '49';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 2, IdPermission FROM Permissions WHERE IdPermission = '53';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 3, IdPermission FROM Permissions WHERE IdPermission = '1';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '1';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '2';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '9';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '13';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '17';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '21';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '25';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '29';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '33';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '37';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '41';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '45';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '49';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '53';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 4, IdPermission FROM Permissions WHERE IdPermission = '54';
INSERT INTO RolePermissions (     IdRole,     IdPermission ) SELECT 5, IdPermission FROM Permissions WHERE IdPermission = '2';