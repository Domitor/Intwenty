using Intwenty.Entity;
using Intwenty.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Intwenty.Model
{


    public class ViewUserInterfaceModelItem 
    {
       

        public ViewUserInterfaceModelItem()
        {
            SetEmptyStrings();
            UIModel = new List<UserInterfaceModelItem>();
        }

        public ViewUserInterfaceModelItem(string systemmetacode, string appmetacode, string viewmetacode, string userinterfacemetacode)
        {
            SetEmptyStrings();
            UIModel = new List<UserInterfaceModelItem>();
        }

        public ViewUserInterfaceModelItem(ViewUserInterfaceItem entity)
        {
            Id = entity.Id;
            SystemMetaCode = entity.SystemMetaCode;
            AppMetaCode = entity.AppMetaCode;
            SystemMetaCode = entity.SystemMetaCode;
            ViewMetaCode = entity.ViewMetaCode;
            UserInterfaceMetaCode = entity.UserInterfaceMetaCode;
            SetEmptyStrings();
            UIModel = new List<UserInterfaceModelItem>();
        }

        private void SetEmptyStrings()
        {

            if (string.IsNullOrEmpty(UserInterfaceMetaCode)) UserInterfaceMetaCode = string.Empty;
            if (string.IsNullOrEmpty(Title)) Title = string.Empty;
            if (string.IsNullOrEmpty(ViewMetaCode)) ViewMetaCode = string.Empty;
            if (string.IsNullOrEmpty(SystemMetaCode)) SystemMetaCode = string.Empty;
            if (string.IsNullOrEmpty(AppMetaCode)) AppMetaCode = string.Empty;
        }

        public int Id { get; set; }
        public SystemModelItem SystemInfo { get; set; }
        public string SystemMetaCode { get; set; }
        public string AppMetaCode { get; set; }
        public string ViewMetaCode { get; set; }
        public string UserInterfaceMetaCode { get; set; }
        public string Title { get; set; }
        public List<UserInterfaceModelItem> UIModel { get; set; }


        public bool HasSystemInfo
        {
            get
            {
                return this.SystemInfo != null;
            }

        }

    }

}
