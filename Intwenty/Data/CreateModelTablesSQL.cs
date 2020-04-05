using Intwenty.Data.DBAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Data
{
    public static class CreateModelTablesSQL
    {

        public static StringBuilder GetApplicationTable(DBMS db)
        {
			var res = new StringBuilder();
			if (db == DBMS.MSSqlServer)
			{
				res.Append("CREATE TABLE [dbo].[sysmodel_ApplicationItem]( ");
				res.Append("[Id] [int] NOT NULL ");
				res.Append(",[Title] [nvarchar](max) NULL ");
				res.Append(",[Description] [nvarchar](max) NULL ");
				res.Append(",[MetaCode] [nvarchar](max) NULL ");
				res.Append(",[DbName] [nvarchar](max) NULL ");
				res.Append(",[IsHierarchicalApplication] [bit] NOT NULL ");
				res.Append(",[UseVersioning] [bit] NOT NULL ");
				res.Append("CONSTRAINT [PK_sysmodel_ApplicationItem] PRIMARY KEY CLUSTERED ([Id] ASC)");
				
			}

			if (db == DBMS.Postgres)
			{
				res.Append("CREATE TABLE [dbo].[sysmodel_ApplicationItem]( ");
				res.Append("[Id] [int] NOT NULL ");
				res.Append(",[Title] [nvarchar](max) NULL ");
				res.Append(",[Description] [nvarchar](max) NULL ");
				res.Append(",[MetaCode] [nvarchar](max) NULL ");
				res.Append(",[DbName] [nvarchar](max) NULL ");
				res.Append(",[IsHierarchicalApplication] [bit] NOT NULL ");
				res.Append(",[UseVersioning] [bit] NOT NULL ");
				res.Append("CONSTRAINT [PK_sysmodel_ApplicationItem] PRIMARY KEY CLUSTERED ([Id] ASC)");

			}

			if (db == DBMS.MariaDB)
			{
				res.Append("CREATE TABLE [dbo].[sysmodel_ApplicationItem]( ");
				res.Append("[Id] [int] NOT NULL ");
				res.Append(",[Title] [nvarchar](max) NULL ");
				res.Append(",[Description] [nvarchar](max) NULL ");
				res.Append(",[MetaCode] [nvarchar](max) NULL ");
				res.Append(",[DbName] [nvarchar](max) NULL ");
				res.Append(",[IsHierarchicalApplication] [bit] NOT NULL ");
				res.Append(",[UseVersioning] [bit] NOT NULL ");
				res.Append("CONSTRAINT [PK_sysmodel_ApplicationItem] PRIMARY KEY CLUSTERED ([Id] ASC)");

			}



			return res;
		}

    }
}
