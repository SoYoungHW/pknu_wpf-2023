
CREATE TABLE [dbo].[hospital]
	[instit_nm] [nvarchar](50) NOT NULL, PRIMARY KEY
	[instit_kind] [nvarchar](20) NOT NULL,
	[medical_instit_kind] [nvarchar](50) NOT NULL,
	[zip_code] [int] NOT NULL,
	[street_nm_addr] [nvarchar](200) NOT NULL,
	[tel] [char](20) NOT NULL,
	[organ_loc] [nvarchar](200) NOT NULL,
	[Monday] [char](20) NULL,
	[Tuesday] [char](20) NULL,
	[Wednesday] [char](20) NULL,
	[Thursday] [char](20) NULL,
	[Friday] [char](20) NULL,
	[Saturday] [char](20) NULL,
	[Sunday] [char](20) NULL,
	[holiday] [char](20) NULL,
	[sunday_oper_week] [char](10) NULL,
	[exam_part] [nvarchar](200) NULL,
	[regist_dt] [date] NULL,
	[update_dt] [date] NULL,
	[lng] [float] NOT NULL,
	[lat] [float] NOT NULL



