using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;


namespace Moley.Data.Entity
{
    public class ValueDomain
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

    public class ValueDomainMap
    {
        public ValueDomainMap(EntityTypeBuilder<ValueDomain> entityBuilder)
        {

        }
    }
}
