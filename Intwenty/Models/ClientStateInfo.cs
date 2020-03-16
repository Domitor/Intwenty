using Intwenty.Data.Dto;
using Intwenty.MetaDataService.Model;
using System.Runtime.Serialization;

namespace Intwenty.Models
{

    public class ClientStateInfo 
    {
        public ApplicationModelItem  Application { get; set; }

        public int Version { get; set; }

        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Properties { get; set; }


        public ClientStateInfo()
        {
            Properties = "";
        }


    }

}
