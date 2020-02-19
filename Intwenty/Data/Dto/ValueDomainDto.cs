using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;


namespace Moley.Data.Entity
{
    public class ValueDomainDto
    {

        public ValueDomainDto(ValueDomain entity)
        {
            Id = entity.Id;
            DomainName = entity.DomainName;
            Code = entity.Code;
            Value = entity.Value;
            Properties = entity.Properties;
        }

        public int Id { get; set; }

        public string DomainName { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }

        public string Properties { get; set; }

    }

   
}
