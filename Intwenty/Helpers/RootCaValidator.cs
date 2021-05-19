using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Intwenty.Helpers
{
    public class RootCaValidator
    {
        private readonly X509Certificate2 _certificateAuthority;

        public RootCaValidator(X509Certificate2 certificateAuthority)
        {
            _certificateAuthority = certificateAuthority;
        }

        public bool Validate(HttpRequestMessage httpRequestMessage, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            chain = new X509Chain
            {
                ChainPolicy =
                {
                    RevocationMode = X509RevocationMode.NoCheck,
                    VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority
                }
            };
            chain.ChainPolicy.ExtraStore.Add(_certificateAuthority);

            var isChainValid = chain.Build(certificate);
            var isChainRootCA = chain.ChainElements[chain.ChainElements.Count - 1].Certificate.RawData.SequenceEqual(_certificateAuthority.RawData);

            return isChainValid && isChainRootCA;
        }
    }
}
