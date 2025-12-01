CREATE DATABASE [AlertifyDB];
GO

USE [AlertifyDB];
GO

CREATE TABLE [User] (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    FirstLastName NVARCHAR(100) NOT NULL,
    SecondLastName NVARCHAR(100) NULL,
    NationalID NVARCHAR(20) NULL,
    Phone NVARCHAR(20),
    Role NVARCHAR(50) NOT NULL,
    LastAccess DATETIME2 NULL,
    ProfilePhotoURL NVARCHAR(500) NULL,
    RegistrationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    MustChangePassword BIT NOT NULL DEFAULT 0,
    CreatedBy INT NOT NULL,
    CreationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ModifiedBy INT NULL,
    ModificationDate DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active'
);

CREATE TABLE Station (
    StationID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Latitude DECIMAL(10, 8) NOT NULL,
    Longitude DECIMAL(11, 8) NOT NULL,
    ServiceType NVARCHAR(50) NOT NULL,
    Address NVARCHAR(255),
    Phone NVARCHAR(20),
    CreatedBy INT NOT NULL,
    CreationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ModifiedBy INT NULL,
    ModificationDate DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active'
);

CREATE TABLE Unit (
    UnitID INT PRIMARY KEY IDENTITY(1,1),
    StationID INT NOT NULL,
    Code NVARCHAR(50) NOT NULL UNIQUE,
    Name NVARCHAR(100),
    ServiceType NVARCHAR(50) NOT NULL,
    UnitStatus NVARCHAR(50) NOT NULL DEFAULT 'Available',
    ResponsiblePerson NVARCHAR(100),
    ContactEmail NVARCHAR(255),
    ContactPhone NVARCHAR(20),
    CreatedBy INT NOT NULL,
    CreationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ModifiedBy INT NULL,
    ModificationDate DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
    FOREIGN KEY (StationID) REFERENCES Station(StationID)
);

CREATE TABLE Emergency (
    EmergencyID INT PRIMARY KEY IDENTITY(1,1),
    CitizenID INT NOT NULL,
    EmergencyCategory NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX),
    Latitude DECIMAL(10, 8) NOT NULL,
    Longitude DECIMAL(11, 8) NOT NULL,
    Address NVARCHAR(255),
    LocationReference NVARCHAR(500) NULL,
    ImageURL NVARCHAR(500) NULL,
    EmergencyStatus NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    Priority NVARCHAR(20) NOT NULL DEFAULT 'Medium',
    AssignmentDate DATETIME2 NULL,
    ResolutionDate DATETIME2 NULL,
    CreatedBy INT NOT NULL,
    CreationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ModifiedBy INT NULL,
    ModificationDate DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
    FOREIGN KEY (CitizenID) REFERENCES [User](UserID)
);

CREATE TABLE EmergencyAssignment (
    EmergencyAssignmentID INT PRIMARY KEY IDENTITY(1,1),
    EmergencyID INT NOT NULL,
    UnitID INT NOT NULL,
    AssignedBy INT NOT NULL,
    AssignmentDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    StartDate DATETIME2 NULL,
    CompletionDate DATETIME2 NULL,
    CalculatedDistance DECIMAL(10, 2),
    AssignmentStatus NVARCHAR(50) NOT NULL DEFAULT 'Assigned',
    Notes NVARCHAR(MAX),
    CreatedBy INT NOT NULL,
    CreationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ModifiedBy INT NULL,
    ModificationDate DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
    FOREIGN KEY (EmergencyID) REFERENCES Emergency(EmergencyID),
    FOREIGN KEY (UnitID) REFERENCES Unit(UnitID)
);

CREATE TABLE Notification (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NULL,
    RecipientEmail NVARCHAR(255) NULL,
    RecipientPhone NVARCHAR(20) NULL,
    EmergencyID INT NULL,
    EmergencyAssignmentID INT NULL,
    NotificationType NVARCHAR(50) NOT NULL,
    SendChannel NVARCHAR(50) NOT NULL,
    Subject NVARCHAR(255),
    Message NVARCHAR(MAX),
    SendDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    SendStatus NVARCHAR(50) NOT NULL DEFAULT 'Sent',
    ErrorMessage NVARCHAR(MAX) NULL,
    ReadDate DATETIME2 NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedBy INT NOT NULL,
    CreationDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ModifiedBy INT NULL,
    ModificationDate DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
    FOREIGN KEY (UserID) REFERENCES [User](UserID),
    FOREIGN KEY (EmergencyID) REFERENCES Emergency(EmergencyID),
    FOREIGN KEY (EmergencyAssignmentID) REFERENCES EmergencyAssignment(EmergencyAssignmentID)
);

CREATE TABLE EmergencyStatusHistory (
    HistoryID INT PRIMARY KEY IDENTITY(1,1),
    EmergencyID INT NOT NULL,
    PreviousStatus NVARCHAR(50) NULL,
    NewStatus NVARCHAR(50) NOT NULL,
    ChangeDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ChangedBy INT NOT NULL,
    Comment NVARCHAR(MAX) NULL,
    FOREIGN KEY (EmergencyID) REFERENCES Emergency(EmergencyID)
);
