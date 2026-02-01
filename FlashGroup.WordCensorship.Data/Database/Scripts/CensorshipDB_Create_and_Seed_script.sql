USE [master]
GO


--------------------------------------------------------------------------------------------------------------------------------
-- Check if Database Censorship already exists and drop it, and recreate again
--------------------------------------------------------------------------------------------------------------------------------
DROP DATABASE IF EXISTS [Censorship]
GO


CREATE DATABASE [Censorship]
    CONTAINMENT = NONE
    ON  PRIMARY 
( NAME = N'Censorship', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER01\MSSQL\DATA\Censorship.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
    LOG ON 
( NAME = N'Censorship_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER01\MSSQL\DATA\Censorship_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
    WITH CATALOG_COLLATION = DATABASE_DEFAULT

GO    

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Censorship].[dbo].[sp_fulltext_database] @action = 'enable'
end

GO

ALTER DATABASE [Censorship] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [Censorship] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [Censorship] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [Censorship] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [Censorship] SET ARITHABORT OFF 
GO

ALTER DATABASE [Censorship] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [Censorship] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [Censorship] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [Censorship] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [Censorship] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [Censorship] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [Censorship] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [Censorship] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [Censorship] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [Censorship] SET  DISABLE_BROKER 
GO

ALTER DATABASE [Censorship] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [Censorship] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [Censorship] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [Censorship] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [Censorship] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [Censorship] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [Censorship] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [Censorship] SET RECOVERY FULL 
GO

ALTER DATABASE [Censorship] SET  MULTI_USER 
GO

ALTER DATABASE [Censorship] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [Censorship] SET DB_CHAINING OFF 
GO

ALTER DATABASE [Censorship] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [Censorship] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [Censorship] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [Censorship] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [Censorship] SET QUERY_STORE = OFF
GO

ALTER DATABASE [Censorship] SET  READ_WRITE 
GO

--------------------------------------------------------------------------------------------------------------------------------
-- Check if SensitiveWords Table already exists and drop it, and recreate again
--------------------------------------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.SensitiveWords', 'U') IS NOT NULL
    DROP TABLE dbo.SensitiveWords;

CREATE TABLE [dbo].[SensitiveWords](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Word] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_SensitiveWords] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

--------------------------------------------------------------------------------------------------------------------------------
-- Populate seed data
--------------------------------------------------------------------------------------------------------------------------------

SET NOCOUNT ON;

;WITH SeedWords AS
(
    SELECT Word
    FROM (VALUES
        ('ACTION'),
        ('ADD'),
        ('ALL'),
        ('ALLOCATE'),
        ('ALTER'),
        ('ANY'),
        ('APPLICATION'),
        ('ARE'),
        ('AREA'),
        ('ASC'),
        ('ASSERTION'),
        ('ATOMIC'),
        ('AUTHORIZATION'),
        ('AVG'),
        ('BEGIN'),
        ('BY'),
        ('CALL'),
        ('CASCADE'),
        ('CASCADED'),
        ('CATALOG'),
        ('CHECK'),
        ('CLOSE'),
        ('COLUMN'),
        ('COMMIT'),
        ('COMPRESS'),
        ('CONNECT'),
        ('CONNECTION'),
        ('CONSTRAINT'),
        ('CONSTRAINTS'),
        ('CONTINUE'),
        ('CONVERT'),
        ('CORRESPONDING'),
        ('CREATE'),
        ('CROSS'),
        ('CURRENT'),
        ('CURRENT_PATH'),
        ('CURRENT_SCHEMA'),
        ('CURRENT_SCHEMAID'),
        ('CURRENT_USER'),
        ('CURRENT_USERID'),
        ('CURSOR'),
        ('DATA'),
        ('DEALLOCATE'),
        ('DECLARE'),
        ('DEFAULT'),
        ('DEFERRABLE'),
        ('DEFERRED'),
        ('DELETE'),
        ('DESC'),
        ('DESCRIBE'),
        ('DESCRIPTOR'),
        ('DETERMINISTIC'),
        ('DIAGNOSTICS'),
        ('DIRECTORY'),
        ('DISCONNECT'),
        ('DISTINCT'),
        ('DO'),
        ('DOMAIN'),
        ('DOUBLEATTRIBUTE'),
        ('DROP'),
        ('EACH'),
        ('EXCEPT'),
        ('EXCEPTION'),
        ('EXEC'),
        ('EXECUTE'),
        ('EXTERNAL'),
        ('FETCH'),
        ('FLOAT'),
        ('FOREIGN'),
        ('FOUND'),
        ('FULL'),
        ('FUNCTION'),
        ('GET'),
        ('GLOBAL'),
        ('GO'),
        ('GOTO'),
        ('GRANT'),
        ('GROUP'),
        ('HANDLER'),
        ('HAVING'),
        ('IDENTITY'),
        ('IMMEDIATE'),
        ('INDEX'),
        ('INDEXED'),
        ('INDICATOR'),
        ('INITIALLY'),
        ('INNER'),
        ('INOUT'),
        ('INPUT'),
        ('INSENSITIVE'),
        ('INSERT'),
        ('INTERSECT'),
        ('INTO'),
        ('ISOLATION'),
        ('JOIN'),
        ('KEY'),
        ('LANGUAGE'),
        ('LAST'),
        ('LEAVE'),
        ('LEVEL'),
        ('LOCAL'),
        ('LONGATTRIBUTE'),
        ('LOOP'),
        ('MODIFIES'),
        ('MODULE'),
        ('NAMES'),
        ('NATIONAL'),
        ('NATURAL'),
        ('NEXT'),
        ('NULLIF'),
        ('ON'),
        ('ONLY'),
        ('OPEN'),
        ('OPTION'),
        ('ORDER'),
        ('OUT'),
        ('OUTER'),
        ('OUTPUT'),
        ('OVERLAPS'),
        ('OWNER'),
        ('PARTIAL'),
        ('PATH'),
        ('PRECISION'),
        ('PREPARE'),
        ('PRESERVE'),
        ('PRIMARY'),
        ('PRIOR'),
        ('PRIVILEGES'),
        ('PROCEDURE'),
        ('PUBLIC'),
        ('READ'),
        ('READS'),
        ('REFERENCES'),
        ('RELATIVE'),
        ('REPEAT'),
        ('RESIGNAL'),
        ('RESTRICT'),
        ('RETURN'),
        ('RETURNS'),
        ('REVOKE'),
        ('ROLLBACK'),
        ('ROUTINE'),
        ('ROW'),
        ('ROWS'),
        ('SCHEMA'),
        ('SCROLL'),
        ('SECTION'),
        ('SELECT'),
        ('SEQ'),
        ('SEQUENCE'),
        ('SESSION'),
        ('SESSION_USER'),
        ('SESSION_USERID'),
        ('SET'),
        ('SIGNAL'),
        ('SOME'),
        ('SPACE'),
        ('SPECIFIC'),
        ('SQL'),
        ('SQLCODE'),
        ('SQLERROR'),
        ('SQLEXCEPTION'),
        ('SQLSTATE'),
        ('SQLWARNING'),
        ('STATEMENT'),
        ('STRINGATTRIBUTE'),
        ('SUM'),
        ('SYSACC'),
        ('SYSHGH'),
        ('SYSLNK'),
        ('SYSNIX'),
        ('SYSTBLDEF'),
        ('SYSTBLDSC'),
        ('SYSTBT'),
        ('SYSTBTATT'),
        ('SYSTBTDEF'),
        ('SYSUSR'),
        ('SYSTEM_USER'),
        ('SYSVIW'),
        ('SYSVIWCOL'),
        ('TABLE'),
        ('TABLETYPE'),
        ('TEMPORARY'),
        ('TRANSACTION'),
        ('TRANSLATE'),
        ('TRANSLATION'),
        ('TRIGGER'),
        ('UNDO'),
        ('UNION'),
        ('UNIQUE'),
        ('UNTIL'),
        ('UPDATE'),
        ('USAGE'),
        ('USER'),
        ('USING'),
        ('VALUE'),
        ('VALUES'),
        ('VIEW'),
        ('WHERE'),
        ('WHILE'),
        ('WITH'),
        ('WORK'),
        ('WRITE'),
        ('ALLSCHEMAS'),
        ('ALLTABLES'),
        ('ALLVIEWS'),
        ('ALLVIEWTEXTS'),
        ('ALLCOLUMNS'),
        ('ALLINDEXES'),
        ('ALLINDEXCOLS'),
        ('ALLUSERS'),
        ('ALLTBTS'),
        ('TABLEPRIVILEGES'),
        ('TBTPRIVILEGES'),
        ('MYSCHEMAS'),
        ('MYTABLES'),
        ('MYTBTS'),
        ('MYVIEWS'),
        ('SCHEMAVIEWS'),
        ('DUAL'),
        ('SCHEMAPRIVILEGES'),
        ('SCHEMATABLES'),
        ('STATISTICS'),
        ('USRTBL'),
        ('STRINGTABLE'),
        ('LONGTABLE'),
        ('DOUBLETABLE'),
        ('SELECT * FROM')
    ) AS V(Word)
)

INSERT INTO dbo.SensitiveWords (Word)
SELECT SW.Word
FROM SeedWords SW
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.SensitiveWords T
    WHERE T.Word = SW.Word
);

--------------------------------------------------------------------------------------------------------------------------------