using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Intwenty.Model.FrejaEId
{
   

    public class ResponseData
    {

        public bool success { get; set; }
        public string id { get; set; }
        public string message { get; set; }
        public string url { get; set; }

        public ResponseData()
        {

            this.success = false;
            this.id = "";
            this.message = "";
            this.url = "";
        }

        public ResponseData(bool success, string id, string message, string url = "")
        {
            // Set values for instance variables
            this.success = success;
            this.id = id;
            this.message = message;
            this.url = url;
        }

    }

    public class Signature
    {

        public string validation_type { get; set; }
        public string algorithm { get; set; }
        public string padding { get; set; }
        public string data { get; set; }
        public string value { get; set; }
        public string certificate { get; set; }



        public Signature()
        {
            // Set values for instance variables
            this.validation_type = null;
            this.algorithm = null;
            this.padding = null;
            this.data = null;
            this.value = null;
            this.certificate = null;
        }

    }

    public class SignatureValidationResult
    {

        public bool valid { get; set; }
        public string signature_data { get; set; }
        public string signatory { get; set; }
        public X509Certificate2 certificate { get; set; }

        public SignatureValidationResult()
        {
            // Set values for instance variables
            this.valid = false;
            this.signature_data = null;
            this.signatory = null;
            this.certificate = null;
        }

    }

    public class DataToSign
    {
        public string text { get; set; }
        public string binaryData { get; set; }
    } 

    public class PushNotification
    {
        public string title { get; set; }
        public string text { get; set; }
    } 

    public class AttributesToReturnItem
    {
        public string attribute { get; set; }
    } 
    public class FrejaRequest
    {
        public string userInfoType { get; set; }
        public string userInfo { get; set; }
        public string minRegistrationLevel { get; set; }
        public string title { get; set; }
        public PushNotification pushNotification { get; set; }
        public Int64? expiry { get; set; }
        public string dataToSignType { get; set; }
        public DataToSign dataToSign { get; set; }
        public string signatureType { get; set; }
        public IList<AttributesToReturnItem> attributesToReturn { get; set; }
    } 

    public class BasicUserInfo
    {
        public string name { get; set; }
        public string surname { get; set; }
    } 

    public class AddressesItem
    {
        public string country { get; set; }
        public string city { get; set; }
        public string postCode { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string validFrom { get; set; }
        public string type { get; set; }
        public string sourceType { get; set; }
    } 

    public class Ssn
    {
        public string ssn { get; set; }
        public string country { get; set; }
    } 
    public class RequestedAttributes
    {
        public BasicUserInfo basicUserInfo { get; set; }
        public string emailAddress { get; set; }
        public string dateOfBirth { get; set; }
        public List<AddressesItem> addresses { get; set; }
        public Ssn ssn { get; set; }
        public string relyingPartyUserId { get; set; }
        public string integratorSpecificUserId { get; set; }
        public string customIdentifier { get; set; }
    } 

    public class FrejaStatusResponse
    {
        public string authRef { get; set; }
        public string signRef { get; set; }
        public string status { get; set; }
        public string details { get; set; }
        public RequestedAttributes requestedAttributes { get; set; }

        public bool HasWaitForUserStatus
        {

            get
            {
                if (string.IsNullOrEmpty(status))
                    return false;

                if (status == "STARTED" || status == "DELIVERED_TO_MOBILE" || status == "OPENED" || status == "OPENED")
                    return true;

                return false;
            }
        }

        public bool HasApprovedStatus
        {

            get
            {
                if (string.IsNullOrEmpty(status))
                    return false;

                if (status == "APPROVED")
                    return true;

                return false;
            }
        }

       
        

    } 

    public class FrejaResponseHeader
    {
        public string x5t { get; set; }
        public string alg { get; set; }
    } 

    public class FrejaPayload
    {
        public string authRef { get; set; }
        public string signRef { get; set; }
        public string status { get; set; }
        public string userInfoType { get; set; }
        public string userInfo { get; set; }
        public string minRegistrationLevel { get; set; }
        public RequestedAttributes requestedAttributes { get; set; }
        public string signatureType { get; set; }
        public SignatureData signatureData { get; set; }
        public Int64? timestamp { get; set; }
    } 

    public class SignatureData
    {
        public string userSignature { get; set; }
        public string certificateStatus { get; set; }
    } 

}