using Moley.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moley.Models.MetaDesigner
{
    public static class NoSeriesVmCreator
    {
        public static List<NoSeriesVm> GetNoSeriesVm(List<NoSerieDto> list)
        {
            var res = new List<NoSeriesVm>();
            foreach (var t in list)
            {
                var s = new NoSeriesVm() { Id = t.Id, Description = t.Description, Prefix = t.Prefix, StartValue = t.StartValue };
                if (t.IsDataConnected)
                    s.ColumnName = t.DataInfo.DbName;
                if (t.Application != null)
                {
                    s.ApplicationId = t.Application.Id;
                    s.TableName = t.Application.DbName;
                }
                res.Add(s);
            }
            return res;

        }

    }

    public static class NoSeriesDtoCreator
    {
        public static List<NoSerieDto> GetNoSeriesDto(List<NoSeriesVm> list)
        {
            var res = new List<NoSerieDto>();
            foreach (var t in list)
            {
                var s = new NoSerieDto() { Id = t.Id, Description = t.Description, Prefix = t.Prefix, StartValue = t.StartValue };

                throw new NotImplementedException();

                res.Add(s);
            }
            return res;

        }

    }

    public class NoSeriesVm
    {
        public int Id = 0;
        public int ApplicationId = 0;
        public int StartValue = 1000;
        public string TableName = "";
        public string ColumnName = "";
        public string Prefix = "";
        public string Description = "";
      

    }
}
