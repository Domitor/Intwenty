using Intwenty.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Intwenty.Interface
{
    public interface IIntwentySeeder
    {
        Task SeedLocalization(IntwentySettings settings, IServiceProvider services);
        Task SeedProductAndOrganization(IntwentySettings settings, IServiceProvider services);
        Task SeedProductAuthorizationItems(IntwentySettings settings, IServiceProvider services);
        Task SeedUsersAndRoles(IntwentySettings settings, IServiceProvider services);
        Task SeedModel(IntwentySettings settings, IServiceProvider services);
        Task SeedData(IntwentySettings settings, IServiceProvider services);
        Task ConfigureDataBase(IntwentySettings settings, IServiceProvider services);
    }

}
