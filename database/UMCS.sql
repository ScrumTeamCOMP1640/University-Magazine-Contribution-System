-- Create the Database
CREATE DATABASE UMCS;

-- Set the Database as the Current Database
USE UMCS;

-- Create Role Table
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY,
    RoleName VARCHAR(50) UNIQUE NOT NULL
);

-- Create User Table
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    RoleID INT,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- Create Faculty Table
CREATE TABLE Faculties (
    FacultyID INT PRIMARY KEY IDENTITY,
    FacultyName VARCHAR(100) UNIQUE NOT NULL
);

-- Create Article Table
CREATE TABLE Articles (
    ArticleID INT PRIMARY KEY IDENTITY,
    Title VARCHAR(255) NOT NULL,
    Content TEXT,
    ImagePath VARCHAR(255),
    SubmissionDate DATETIME,
    FacultyID INT,
    UserID INT,
    Status VARCHAR(50),
    FOREIGN KEY (FacultyID) REFERENCES Faculties(FacultyID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Create Comment Table
CREATE TABLE Comments (
    CommentID INT PRIMARY KEY IDENTITY,
    ArticleID INT,
    CommentContent TEXT,
    CommentDate DATETIME,
    UserID INT,
    FOREIGN KEY (ArticleID) REFERENCES Articles(ArticleID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Create ClosureDate Table
CREATE TABLE ClosureDates (
    ClosureDateID INT PRIMARY KEY IDENTITY,
    AcademicYear INT,
    ClosureDate DATETIME,
    FacultyID INT,
    FOREIGN KEY (FacultyID) REFERENCES Faculties(FacultyID)
);

-- Create TermsAndConditions Table
CREATE TABLE TermsAndConditions (
    TermsAndConditionsID INT PRIMARY KEY IDENTITY,
    Content TEXT,
    Version VARCHAR(50),
    ReleaseDate DATE
);

-- Create SelectedArticle Table
CREATE TABLE SelectedArticles (
    SelectedArticleID INT PRIMARY KEY IDENTITY,
    ArticleID INT,
    PublicationDate DATE,
    IsPublished BIT,
    FOREIGN KEY (ArticleID) REFERENCES Articles(ArticleID)
);

-- Create GuestAccount Table
CREATE TABLE GuestAccounts (
    GuestID INT PRIMARY KEY IDENTITY,
    FacultyID INT,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    FOREIGN KEY (FacultyID) REFERENCES Faculties(FacultyID)
);

