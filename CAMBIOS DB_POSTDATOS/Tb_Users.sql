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

CREATE TABLE Roles
(
    IdRole INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(80) NOT NULL,
    Description NVARCHAR(250),
    IsActive BIT NOT NULL DEFAULT(1)
)

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

    -------------------------------------------------------
    -- Validar Username duplicado
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
    END
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
    -- OK
    -------------------------------------------------------
    SELECT
        1 AS Result,
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

