using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Interface
{
    public interface IIntwentySeeder
    {
        void SeedLocalization(IntwentySettings settings, IServiceProvider services);
        void SeedProductAndOrganization(IntwentySettings settings, IServiceProvider services);
        void SeedProductAuthorizationItems(IntwentySettings settings, IServiceProvider services);
        void SeedUsersAndRoles(IntwentySettings settings, IServiceProvider services);
        void SeedModel(IntwentySettings settings, IServiceProvider services);
        void SeedData(IntwentySettings settings, IServiceProvider services);
        void ConfigureDataBase(IntwentySettings settings, IServiceProvider services);
    }

}
