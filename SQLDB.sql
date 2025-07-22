-- Tạo cơ sở dữ liệu
CREATE DATABASE NewsDB;
GO

USE NewsDB;
GO

-- 1. Tạo bảng SystemAccount
CREATE TABLE SystemAccount (
    AccountID INT PRIMARY KEY IDENTITY(1,1),
    AccountName NVARCHAR(50) NOT NULL,
    AccountEmail NVARCHAR(100) NOT NULL UNIQUE,
    AccountRole INT NOT NULL CHECK (AccountRole IN (1, 2, 3,4)), -- 1: Staff, 2: Lecturer, 3: Admin, 4: Reader
    AccountPassword NVARCHAR(100) NOT NULL
);
GO

-- 2. Tạo bảng Category
CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL,
    CategoryDescription NVARCHAR(500),
    ParentCategoryID INT,
    IsActive BIT NOT NULL DEFAULT 1 CHECK (IsActive IN (0, 1)), -- 1: active, 0: inactive
    FOREIGN KEY (ParentCategoryID) REFERENCES Category(CategoryID)
);
GO

-- 3. Tạo bảng NewsArticle
CREATE TABLE NewsArticle (
    NewsArticleID INT PRIMARY KEY IDENTITY(1,1),
    NewsTitle NVARCHAR(200) NOT NULL,
    Headline NVARCHAR(500),
    ImagePath NVARCHAR(255),
    CreatedDate DATETIME NOT NULL DEFAULT '2025-05-28 16:37:00 +07:00',
    NewsContent NVARCHAR(MAX),
    NewsSource NVARCHAR(100),
    CategoryID INT NOT NULL, -- Một NewsArticle chỉ thuộc một Category
    NewsStatus INT NOT NULL, 
    CreatedByID INT,
    UpdatedByID INT,
    ModifiedDate DATETIME,
    ViewCount INT DEFAULT 0,
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID),
    FOREIGN KEY (CreatedByID) REFERENCES SystemAccount(AccountID),
    FOREIGN KEY (UpdatedByID) REFERENCES SystemAccount(AccountID)
);
GO

-- 4. Tạo bảng Tag
CREATE TABLE Tag (
    TagID INT PRIMARY KEY IDENTITY(1,1),
    TagName NVARCHAR(50) NOT NULL,
    Note NVARCHAR(200)
);
GO

-- 5. Tạo bảng trung gian NewsTag (mối quan hệ nhiều-nhiều giữa NewsArticle và Tag)
CREATE TABLE NewsTag (
    NewsArticleID INT NOT NULL,
    TagID INT NOT NULL,
    PRIMARY KEY (NewsArticleID, TagID),
    FOREIGN KEY (NewsArticleID) REFERENCES NewsArticle(NewsArticleID),
    FOREIGN KEY (TagID) REFERENCES Tag(TagID)
);
GO

-- 6. Tạo bảng Comment
CREATE TABLE Comment (
    CommentID INT PRIMARY KEY IDENTITY(1,1),
    NewsArticleID INT NOT NULL,
    CreatedByID INT NOT NULL,
    CommentText NVARCHAR(1000) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT '2025-05-28 16:37:00 +07:00',
    IsActive BIT NOT NULL DEFAULT 1 CHECK (IsActive IN (0, 1)),
    ParentCommentID INT NULL,
    CONSTRAINT FK_Comment_Parent FOREIGN KEY (ParentCommentID) REFERENCES Comment(CommentID);
    FOREIGN KEY (NewsArticleID) REFERENCES NewsArticle(NewsArticleID),
    FOREIGN KEY (CreatedByID) REFERENCES SystemAccount(AccountID)
);
GO

CREATE TABLE NewsLike (
    LikeID INT PRIMARY KEY IDENTITY,
    NewsArticleID INT NOT NULL,
    LikedByID INT NOT NULL,
    LikedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Like_News FOREIGN KEY (NewsArticleID) REFERENCES NewsArticle(NewsArticleID),
    CONSTRAINT FK_Like_User FOREIGN KEY (LikedByID) REFERENCES SystemAccount(AccountID),
    CONSTRAINT UQ_News_Like UNIQUE (NewsArticleID, LikedByID) -- Mỗi người chỉ like 1 lần
);
GO
-- chen du lieu vao 
USE NewsDB;
GO

-- 1. Chèn dữ liệu vào bảng SystemAccount
INSERT INTO SystemAccount (AccountName, AccountEmail, AccountRole, AccountPassword) VALUES
('John Doe', 'john.doe@example.com', 1, 'password123'), -- Staff
('Jane Smith', 'jane.smith@example.com', 2, 'password456'), -- Lecturer
('Admin User', 'admin@example.com', 3, 'admin789'); -- Admin
GO

-- 2. Chèn dữ liệu vào bảng Category
INSERT INTO Category (CategoryName, CategoryDescription, ParentCategoryID, IsActive) VALUES
('Technology', 'News about technology and gadgets', NULL, 1),
('Science', 'Latest updates in the world of science', NULL, 1),
('AI', 'Artificial Intelligence news', 1, 1); -- AI là danh mục con của Technology
GO

-- 3. Chèn dữ liệu vào bảng NewsArticle
INSERT INTO NewsArticle (NewsTitle, Headline, CreatedDate, NewsContent, NewsSource, CategoryID, NewsStatus, CreatedByID, UpdatedByID, ModifiedDate) VALUES
('AI Breakthrough in 2025', 'New AI model achieves human-like reasoning', GETDATE(), 'A new AI model developed by xAI has shown remarkable progress...', 'xAI Blog', 3, 1, 1, NULL, NULL),
('Quantum Computing Advances', 'Scientists make strides in quantum computing', GETDATE(), 'Researchers at MIT have developed a new quantum chip...', 'MIT News', 1, 1, 1, NULL, NULL),
('Space Exploration Update', 'New discoveries on Mars', GETDATE(), 'NASA reports signs of ancient water on Mars...', 'NASA', 2, 0, 1, 3, GETDATE());
GO

-- 4. Chèn dữ liệu vào bảng Tag
INSERT INTO Tag (TagName, Note) VALUES
('AI', 'Artificial Intelligence related'),
('Quantum', 'Quantum computing topics'),
('Space', 'Space exploration news'),
('Mars', 'Mars-specific news');
GO

-- 5. Chèn dữ liệu vào bảng NewsTag (mối quan hệ nhiều-nhiều)
INSERT INTO NewsTag (NewsArticleID, TagID) VALUES
(1, 1), -- AI Breakthrough -> Tag: AI
(2, 2), -- Quantum Computing -> Tag: Quantum
(3, 3), -- Space Exploration -> Tag: Space
(3, 4); -- Space Exploration -> Tag: Mars
GO

-- 6. Chèn dữ liệu vào bảng Comment
INSERT INTO Comment (NewsArticleID, CreatedByID, CommentText, CreatedDate, IsActive) VALUES
(7, 2, 'This AI breakthrough is incredible!', '2025-05-28 16:50:00 ', 1),
(7, 3, 'Looking forward to more updates on this.', '2025-05-28 16:50:00 ', 1),
(9, 2, 'Mars discoveries are always exciting!', '2025-05-28 16:50:00 ', 0);
GO