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
select * from permissions where module = 'TransactionsTypes'
select * from rolepermissions where idrole = 2

@using TransferApp.Extensions

insert into Permissions (Module, Action, Description) values ('Clients', 'View', 'Permission to view clients');

insert into rolepermissions (IdRole, IdPermission) values (1, 54);
insert into rolepermissions (IdRole, IdPermission) values (1, 55);
insert into rolepermissions (IdRole, IdPermission) values (1, 56);
insert into rolepermissions (IdRole, IdPermission) values (1, 57);

select * from roles
delete from roles where idRole = 5

