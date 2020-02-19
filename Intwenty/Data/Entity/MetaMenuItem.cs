using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;


namespace Moley.Data.Entity
{
    public class MetaMenuItem
    {
        [Key]
        public int Id { get; set; }

        public string AppMetaCode { get; set; }

        public string Title { get; set; }

        public string MetaType { get; set; }

        public string MetaCode { get; set; }

        public string ParentMetaCode { get; set; }

        public int Order { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Properties { get; set; }

    }

   

    public class MetaMenuItemMap
    {
        public MetaMenuItemMap(EntityTypeBuilder<MetaMenuItem> entityBuilder)
        {

        }
    }
}
