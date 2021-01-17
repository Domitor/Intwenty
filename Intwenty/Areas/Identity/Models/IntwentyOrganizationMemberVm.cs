using Intwenty.Areas.Identity.Entity;
using Intwenty.DataClient.Reflection;
using Intwenty.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{

    public class IntwentyOrganizationMemberVm
    {

        public IntwentyOrganizationMemberVm()
        {

        }
        public IntwentyOrganizationMemberVm(IntwentyOrganizationMember entity)
        {
            Id = entity.Id;
            OrganizationId = entity.OrganizationId;
            UserId = entity.UserId;
            UserName = entity.UserName;
        }

        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }



    }
}
