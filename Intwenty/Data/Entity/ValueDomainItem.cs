using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;


namespace Intwenty.Data.Entity
{
    public class ValueDomainItem
    {
        [Key]
        public int Id { get; set; }

        public string DomainName { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(300)]
        public string Value { get; set; }

        public string Properties { get; set; }

    }

    public class ValueDomainItemMap
    {
        public ValueDomainItemMap(EntityTypeBuilder<ValueDomainItem> entityBuilder)
        {

        }
    }
}
