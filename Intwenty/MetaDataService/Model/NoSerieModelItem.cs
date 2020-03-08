using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moley.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;


namespace Moley.MetaDataService.Model
{
    public class NoSerieModelItem
    {

        public NoSerieModelItem()
        {
            SetEmptyStrings();
        }

        public NoSerieModelItem(Data.Entity.NoSerie entity)
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
            SetEmptyStrings();
        }


        private void SetEmptyStrings()
        {
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
            if (string.IsNullOrEmpty(MetaCode)) MetaCode = string.Empty;
            if (string.IsNullOrEmpty(DataMetaCode)) DataMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Properties)) Properties = string.Empty;
            if (string.IsNullOrEmpty(Description)) Description = string.Empty;
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

        public bool IsDataConnected
        {
            get { return DataInfo != null && !string.IsNullOrEmpty(DataMetaCode); }
        }

        public DatabaseModelItem DataInfo { get; set; }

        public ApplicationModelItem Application { get; set; }


    }

   
}
