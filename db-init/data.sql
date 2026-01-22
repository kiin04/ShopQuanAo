CREATE DATABASE ShopQuanAo;
GO
USE ShopQuanAo;
GO
CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    Phone NVARCHAR(15),
    Address NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE()
);
ALTER TABLE Customers  
ADD PasswordHash NVARCHAR(255) NOT NULL,  -- Lưu mật khẩu đã hash  
    Avatar NVARCHAR(255) NULL;  -- Lưu đường dẫn ảnh đại diện  

CREATE TABLE Categories (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);
CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(255) NOT NULL,
    CategoryID INT FOREIGN KEY REFERENCES Categories(CategoryID),
    Price DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL DEFAULT 0,
    Description NVARCHAR(MAX),
    ImageURL NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE()
);
CREATE TABLE Suppliers (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(100) NOT NULL,
    ContactName NVARCHAR(100),
    Phone NVARCHAR(15),
    Address NVARCHAR(255)
);
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
    Status NVARCHAR(50) CHECK (Status IN (N'Chờ Xác Nhận', N'Đã Giao', N'Đã Hủy'))
);
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL CHECK (Quantity > 0),
    Price DECIMAL(10,2) NOT NULL
);
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) CHECK (Role IN ('Admin', N'Nhân Viên'))
);
CREATE TABLE Comments (
    CommentID INT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    CustomerID INT NOT NULL,
    ParentCommentID INT NULL,
    CommentText NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Comments_Product FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE,
    CONSTRAINT FK_Comments_Customer FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE CASCADE,
    CONSTRAINT FK_Comments_Parent FOREIGN KEY (ParentCommentID) REFERENCES Comments(CommentID)
);
CREATE TABLE Cart (
    CartID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    AddedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Cart_Customer FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE CASCADE,
    CONSTRAINT FK_Cart_Product FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE
);

ALTER TABLE Users  
ADD Email NVARCHAR(100) UNIQUE  -- Lưu mật khẩu đã hash 
INSERT INTO Users (Username, PasswordHash, Role, Email)  
VALUES ('admin', '123456', 'Admin', 'admin@gmail.com');
