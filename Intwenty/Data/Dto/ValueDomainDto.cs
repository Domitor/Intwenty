using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moley.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Moley.Data.Dto
{
    public class ValueDomainDto : MetaModelDto
    {

        public ValueDomainDto(ValueDomain entity)
        {
            Id = entity.Id;
            DomainName = entity.DomainName;
            Code = entity.Code;
            Value = entity.Value;
            Properties = entity.Properties;
            MetaType = "VALUEDOMAIN";
            MetaCode = DomainName;
            ParentMetaCode = "ROOT";
        }

        public int Id { get; set; }

        public string DomainName { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }

        public override bool HasValidMetaType
        {
            get { return true;  }
        }

        protected override List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();
                return t;
            }
        }

        protected override List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add("VALUEDOMAIN");
                return t;
            }
        }
    }

   
}
