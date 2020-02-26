using Moley.Data.Entity;
using System.Collections.Generic;

namespace Moley.Data.Dto
{
    public class MetaMenuItemDto : MetaModelDto
    {

        public MetaMenuItemDto(MetaMenuItem entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            MetaType = entity.MetaType;
            MetaCode = entity.MetaCode;
            ParentMetaCode = entity.ParentMetaCode;
            Controller = entity.Controller;
            Action = entity.Action;
            Properties = entity.Properties;
        }


        public int Id { get; set; }

        public ApplicationDescriptionDto Application { get; set; }

        public string Title { get; set; }

        public int Order { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string MetaTypeMainMenu
        {
            get { return "MAINMENU"; }
        }

        public bool IsMetaTypeMainMenu
        {
            get { return MetaType == MetaTypeMainMenu; }
        }

        public string MetaTypeMenuItem
        {
            get { return "MENUITEM"; }
        }

        public bool IsMetaTypeMenuItem
        {
            get { return MetaType == MetaTypeMenuItem; }
        }

        public override bool HasValidMetaType
        {
            get
            {
                if (string.IsNullOrEmpty(MetaType))
                    return false;

                if (ValidMetaTypes.Contains(MetaType))
                    return true;

                return false;

            }
        }

      

        protected override List<string> ValidProperties
        {
            get
            {
                var t = new List<string>();
                return t;
            }
        }

        public override List<string> ValidMetaTypes
        {
            get
            {
                var t = new List<string>();
                t.Add("MAINMENU");
                t.Add("MENUITEM");
                return t;
            }
        }

    }

 
}
