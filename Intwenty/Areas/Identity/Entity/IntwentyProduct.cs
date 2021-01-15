﻿using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Entity
{
    [DbTableName("security_Product")]
    [DbTablePrimaryKey("Id")]
    public class IntwentyProduct
    {
        public string Id { get; set; }

        public string ProductName { get; set; }

        public string ProductURI { get; set; }

        public string ApplicationPath { get; set; }

        public string ApplicationAPIPath { get; set; }

        public string WebAPIPath { get; set; }




    }
}