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


