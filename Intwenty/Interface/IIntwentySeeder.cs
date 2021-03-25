﻿using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Interface
{
    public interface IIntwentySeeder
    {
        void SeedLocalization();
        void SeedProductAndOrganization();
        void SeedProductAuthorizationItems();
        void SeedUsersAndRoles();
        void SeedModel();
        void SeedData();
        void ConfigureDataBase();
    }

}
