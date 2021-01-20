using Intwenty.Areas.Identity.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Areas.Identity.Models
{
    public class IntwentyUserVm
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int AccessFailedCount { get; set; }

        public bool IsBlocked { get; set; }

        public bool MfaActive { get; set; }

        public string LastLogin { get; set; }

        public string LastLoginProduct { get; set; }

        public string APIKey { get; set; }

        public bool ModelSaved { get; set; }

        public List<IntwentyUserProductVm> UserProducts { get; set; }

        public IntwentyUserVm()
        {
            UserProducts = new List<IntwentyUserProductVm>();
        }

        public IntwentyUserVm(IntwentyUser entity)
        {
            Id = entity.Id;
            UserName = entity.UserName;
            Email = entity.Email;
            EmailConfirmed = entity.EmailConfirmed;
            PhoneNumber = entity.PhoneNumber;
            PhoneNumberConfirmed = entity.PhoneNumberConfirmed;
            FirstName = entity.FirstName;
            LastName = entity.LastName;
            IsBlocked = false;
            if (entity.LockoutEnd.HasValue)
                IsBlocked = entity.LockoutEnabled && entity.LockoutEnd.HasValue && entity.LockoutEnd > DateTime.Now;
            MfaActive = entity.TwoFactorEnabled;
            LastLogin = entity.LastLogin;
            LastLoginProduct = entity.LastLoginProduct;
            AccessFailedCount = entity.AccessFailedCount;
            APIKey = entity.APIKey;
            UserProducts = new List<IntwentyUserProductVm>();
        }

    }

}
