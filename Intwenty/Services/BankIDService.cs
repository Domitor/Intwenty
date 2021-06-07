using Intwenty.Interface;
using Intwenty.Model;
using Intwenty.Model.BankId;
using Microsoft.Extensions.Options;
using QRCoder;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Intwenty.Services
{
    public class BankIDClientService : IBankIDClientService
    {
        

        private readonly HttpClient client;
        private readonly IntwentySettings settings;
        private readonly IIntwentyDbLoggerService logservice;

        public BankIDClientService(HttpClient http_client, IOptions<IntwentySettings> options, IIntwentyDbLoggerService logger)
        {
            this.logservice = logger;
            this.client = http_client;
            this.settings = options.Value;
            this.client.BaseAddress = new Uri(this.settings.BankIdBaseAddress);
        }

        private JsonSerializerOptions GetJsonOptions() 
        {
            var json_options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return json_options;
        }

        private StringContent BuildContent<TRequest>(TRequest request)
        {
           
            var json = JsonSerializer.Serialize(request, GetJsonOptions());
            var result = new StringContent(json, Encoding.UTF8);
            result.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return result;
        }

        public async Task<BankIDAuthResponse> InitAuthentication(BankIDAuthRequest request)
        {


            try
            {
                var content = BuildContent(request);
                var response = await client.PostAsync(settings.BankIdAuthEndPoint, content);
                var data = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode == true)
                {                  
                    var status_response = JsonSerializer.Deserialize<BankIDAuthResponse>(data, GetJsonOptions());
                    return status_response;

                }
                else
                { 
                    throw new InvalidOperationException(string.Format("BankIDClientService.InitAuthentication: {0}", data));
                }
            }
            catch (Exception ex)
            {
                await logservice.LogErrorAsync(ex.Message);
            }

            return null;
            
       }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BankIDCollectResponse> Authenticate(BankIDCollectRequest request)
        {
            try
            {
                var content = BuildContent(request);
                var timeout = settings.BankIdTimeoutInMilliseconds;

                while (true)
                {
                    // Check for a timeout
                    if (timeout <= 0)
                    {
                        await logservice.LogWarningAsync("BankIDClientService.Authenticate: Time Out");
                        var cancelrequest = new BankIDCancelRequest() { OrderRef = request.OrderRef };
                        var cancelcontent = BuildContent(cancelrequest);
                        await client.PostAsync(settings.BankIdCancelEndPoint, cancelcontent);
                        return new BankIDCollectResponse() { HintCode = "timeOut" };
                    }

                    var response = await client.PostAsync(settings.BankIdCollectEndPoint, content);
                    var data = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        var status_response = JsonSerializer.Deserialize<BankIDCollectResponse>(data, GetJsonOptions());
                        if (status_response.IsAuthOk || status_response.IsAuthTimeOut || status_response.IsAuthCanceled || status_response.IsAuthFailure || status_response.IsAuthUserCanceled)
                            return status_response;

                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("BankIDClientService.Authenticate: {0}", data));
                    }
                  

                    await Task.Delay(2000);
                    timeout -= 2000;

                }
            }
            catch (Exception ex)
            {
                await logservice.LogErrorAsync(ex.Message);
            }

            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BankIDAuthResponse> Sign(BankIDSignRequest request)
        {
            try
            {
                var content = BuildContent(request);
                var response = await client.PostAsync(settings.BankIdSignEndPoint, content);
                var data = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode == true)
                {                 
                    var status_response = JsonSerializer.Deserialize<BankIDAuthResponse>(data);
                    return status_response;

                }
                else
                {
                    throw new InvalidOperationException(string.Format("BankIDClientService.Sign: {0}", data));
                }
            }
            catch (Exception ex)
            {
                await logservice.LogErrorAsync(ex.Message);
            }

            return null;

           
        }

      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> Cancel(BankIDCancelRequest request)
        {
            try
            {
                var content = BuildContent(request);
                var response = await client.PostAsync(settings.BankIdCancelEndPoint, content);
                if (response.IsSuccessStatusCode == true)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

            }

            return false;

        }

        public string GetQRCode(string autoStartToken)
        {
            var bidUrl = $"bankid:///?autostarttoken={autoStartToken.Trim()}";

            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(bidUrl, QRCodeGenerator.ECCLevel.Q);

                using (var qrCode = new Base64QRCode(qrCodeData))
                {
                    var qr = qrCode.GetGraphic(settings.BankIdQrSize);
                    return $"data:image/png;base64, {qr}";
                }
            }
        }

      

    }
}