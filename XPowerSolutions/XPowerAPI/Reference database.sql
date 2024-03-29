USE [XPower_Test]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 29-06-2021 10:10:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Mail] [varchar](250) NULL,
	[Username] [varchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserAuthentication]    Script Date: 29-06-2021 10:10:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAuthentication](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[User_FK_Users_Id] [int] NOT NULL,
	[Password] [varchar](250) NOT NULL,
	[Salt] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[AuthUsersView]    Script Date: 29-06-2021 10:10:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AuthUsersView]
AS
	SELECT
		u.Id AS ID,
		u.Mail as Mail,
		u.Username as Username,
		Auth.[Password] as [Password],
		Auth.Salt as Salt
	FROM Users as u
	INNER JOIN UserAuthentication as Auth
	on u.Id = Auth.User_FK_Users_Id
GO


/****** Object:  Table [dbo].[RefreshToken]    Script Date: 29-06-2021 10:10:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RefreshToken](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Token] [nvarchar](max) NULL,
	[Expires] [datetime2](7) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[CreatedByIp] [nvarchar](max) NULL,
	[Revoked] [datetime2](7) NULL,
	[RevokedByIp] [nvarchar](max) NULL,
	[ReplacedByToken] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_RefreshToken] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


/****** Object:  Index [UQ__UserAuth__73B51CB945797E73]    Script Date: 29-06-2021 10:10:57 ******/
ALTER TABLE [dbo].[UserAuthentication] ADD UNIQUE NONCLUSTERED 
(
	[User_FK_Users_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[RefreshToken]  WITH CHECK ADD  CONSTRAINT [FK_RefreshToken_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RefreshToken] CHECK CONSTRAINT [FK_RefreshToken_Users_UserId]
GO
ALTER TABLE [dbo].[UserAuthentication]  WITH CHECK ADD FOREIGN KEY([User_FK_Users_Id])
REFERENCES [dbo].[Users] ([Id])
GO
/****** Object:  StoredProcedure [dbo].[SPCreateNewUser]    Script Date: 29-06-2021 10:10:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPCreateNewUser] 
	@Username nvarchar(250),
	@Email nvarchar(250),
	@Password nvarchar(250),
	@Salt nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @InsertedUserID INT;

	INSERT INTO [Users] ([Username], [Mail]) 
	VALUES (@Username, @Email)
	SET @InsertedUserID = SCOPE_IDENTITY()
	
	INSERT INTO [UserAuthentication] ([User_FK_Users_Id],[Password],[Salt]) 
	VALUES (@InsertedUserID,@Password, @Salt);

	SELECT @InsertedUserID;
END
GO

/****** Object:  StoredProcedure [dbo].[SPGetUserByLoginName]    Script Date: 29-06-2021 10:10:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetUserByLoginName]
(
	@LoginName VARCHAR(150)
)  
AS  
BEGIN  
    -- SET NOCOUNT ON added to prevent extra result sets from  
    -- interfering with SELECT statements.  
    SET NOCOUNT ON;  
  
    -- Select statements for procedure here  
    Select * from Users where Mail = @LoginName;  
END  
GO
/****** Object:  StoredProcedure [dbo].[SPGetUserByToken]    Script Date: 29-06-2021 10:10:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPGetUserByToken]
(
	@Token VARCHAR(150)
)
AS  
BEGIN  
    SET NOCOUNT ON;  

	DECLARE @userID INT;

	SET @userID = (
		select UserId 
		from RefreshToken
		where Token = @Token 
	);

	SELECT * 
	FROM Users
	WHERE Id = @userID
END  
GO

/****** Object:  StoredProcedure [dbo].[SPUserEmailIsUnique]    Script Date: 29-06-2021 10:10:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SPUserEmailIsUnique]
(
	@Email AS varchar(150)
)
AS
BEGIN
	  SET NOCOUNT ON;
 
      IF EXISTS(SELECT 1 FROM Users WHERE Mail = @Email)
      BEGIN
            select 1
      END
      ELSE
      BEGIN
            select 0
      END

END;
GO