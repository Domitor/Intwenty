using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moley.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;


namespace Moley.Data.Dto
{
    public class NoSerieDto
    {

        public NoSerieDto(NoSerie entity)
        {
            Id = entity.Id;
            AppMetaCode = entity.AppMetaCode;
            MetaCode = entity.MetaCode;
            DataMetaCode = entity.DataMetaCode;
            Description = entity.Description;
            Counter = entity.Counter;
            StartValue = entity.StartValue;
            Prefix = entity.Prefix;
            Properties = entity.Properties;
        }

        public int Id { get; set; }

        public string AppMetaCode { get; set; }

        public string MetaCode { get; set; }

        public string DataMetaCode { get; set; }

        public int Counter { get; set; }

        public int StartValue { get; set; }

        public string Prefix { get; set; }

        public string Description { get; set; }

        public string Properties { get; set; }

        public string NewValue { get; set; }


    }

   
}
