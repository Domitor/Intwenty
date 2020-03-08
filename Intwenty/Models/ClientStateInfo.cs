using Moley.Data.Dto;
using Moley.MetaDataService.Model;
using System.Runtime.Serialization;

namespace Moley.Models
{
    public enum TimeScopeDefinition
    {
        [EnumMember]
        NotSpecified = 0,
        [EnumMember]
        LatestPriorToSpecifiedContextEnd = 1,
        [EnumMember]
        DuringSpecifiedContextSpan = 2,
        [EnumMember]
        CarePeriodEndToCareVisitEnd = 3,
        [EnumMember]
        LatestInSpecifiedContextSpan = 4,
        [EnumMember]
        LatestInSpecifiedOrg = 5,
        [EnumMember]
        LatestInSystem = 6,
        [EnumMember]
        PriorToSpecifiedContextStart = 7,
        [EnumMember]
        LatestInCarePeriodEndToCareVisitEnd = 8

    }


   


    public class ClientStateInfo 
    {
        public ApplicationModelItem  Application { get; set; }

        public int Version { get; set; }

        public int Id { get; set; }

        public int OwnerId { get; set; }



        public ClientStateInfo()
        {
           
        }


    }

}
