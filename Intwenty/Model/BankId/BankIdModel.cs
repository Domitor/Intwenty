using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.BankId
{
    public class BankIDAuthRequest
    {

        /// <summary>
        /// The user IP address as seen by RP. String. IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address. It must be the IP address
        /// representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be
        /// taken to get the correct address. In some use cases the IP address is not available,
        /// for instance for voice based services. In this case, the internal representation
        /// of those systems IP address is ok to use. 
        /// </summary>
        public string EndUserIp { get; set; }

        /// <summary>
        /// The personal number of the user. String. 12 digits. Century must be included.
        /// If the personal number is excluded, the client must be started with the autoStartToken returned in the response 
        /// </summary>
        public string PersonalNumber { get; set; }

    }


    public class BankIDSignRequest : BankIDAuthRequest
    {


        private string _userVisibleData;
        /// <summary>
        /// The text to be displayed and signed. String. The text can be formatted using CR, LF and CRLF for new lines.
        /// </summary>
        public string UserVisibleData
        {
            // The text must be encoded as UTF-8 and then base 64 encoded. 1--40 000 characters after base 64 encoding
            get => Convert.ToBase64String(Encoding.UTF8.GetBytes(_userVisibleData));
            set => _userVisibleData = value;
        }

        private string _userNonVisibleData;
        /// <summary>
        /// Data not displayed to the user.
        /// </summary>
        public string UserNonVisibleData
        {
            // The value must be base 64-encoded. 1-200 000 characters after base 64-encoding
            get => string.IsNullOrEmpty(_userNonVisibleData) ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(_userNonVisibleData));
            set => _userNonVisibleData = value;
        }


    }


    public class BankIDAuthResponse
    {

        /// <summary>
        /// Used as reference to this order when the client is started automatically
        /// </summary>
        public string AutoStartToken { get; set; }

        /// <summary>
        /// Used to collect the status of the order
        /// </summary>
        public string OrderRef { get; set; }


    }

    public class BankIDCollectRequest
    {
        public string OrderRef { get; set; }

    }

    public class BankIDCollectResponse
    {

        /// <summary>
        /// The orderRef in question
        /// </summary>
        public string OrderRef { get; set; }

        /// <summary>
        /// pending: The order is being processed. hintCode describes the status of the order.
        /// failed: Something went wrong with the order. hintCode describes the error.
        /// complete: The order is complete. completionData holds user information
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Only present for pending and failed orders
        /// </summary>
        public string HintCode { get; set; }

        /// <summary>
        /// Only present for complete orders
        /// </summary>
        public BankIDCompletionData CompletionData { get; set; }


    }

    public class BankIDCompletionData
    {
       
        /// <summary>
        /// The OCSP response. String. Base64-encoded. The OCSP response is signed by a
        /// certificate that has the same issuer as the certificate being verified.
        /// The OSCP response has an extension for Nonce. The nonce is calculated as:
        ///     SHA-1 hash over the base 64 XML signature encoded as UTF-8.
        ///     12 random bytes is added after the hash
        ///     The nonce is 32 bytes (20 + 12) 
        /// </summary>
        public string OcspResponse { get; set; }

        /// <summary>
        /// The content of the signature is described in BankID Signature Profile specification. Base64-encoded. XML signature.
        /// </summary>
        public string Signature { get; set; }

        /*
        /// <summary>
        /// Information related to the users certificate (BankID)
        /// </summary>
        public Cert Cert { get; set; }

        /// <summary>
        /// Information related to the device
        /// </summary>
        public Device Device { get; set; }
             */

        /// <summary>
        /// Information related to the user
        /// </summary>
        public BankIdUser User { get; set; }
   
       
    }

    public class BankIDCancelRequest
    {
        public string OrderRef { get; set; }
    }

    public class BankIdUser 
    {
     
        /// <summary>
        /// The given name of the user
        /// </summary>
        public string GivenName { get; set; }

        /// <summary>
        ///  The given name and surname of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The personal number (SSN)
        /// </summary>
        public string PersonalNumber { get; set; }

        /// <summary>
        /// The surname of the user
        /// </summary>
        public string Surname { get; set; }

      
    }

}
