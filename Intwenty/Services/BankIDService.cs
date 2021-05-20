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

        public BankIDClientService(HttpClient http_client, IOptions<IntwentySettings> options)
        {
            this.client = http_client;
            this.settings = options.Value;
            this.client.BaseAddress = new Uri(this.settings.BankIdBaseAddress);
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<BankIDAuthResponse> InitQRAuthentication(BankIDAuthRequest request)
        {


            try
            {
                // Set serializer options
                var json_options = new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    WriteIndented = false,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(request, json_options);
                var content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType.MediaType = "application/json";
                //content.Headers.ContentType.MediaType.cc = null;

                //content.Headers.ContentType.CharSet = "utf-8";

                var response = await client.PostAsync("https://appapi2.test.bankid.com/rp/v5.1/auth", content);
                if (response.IsSuccessStatusCode == true)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var status_response = JsonSerializer.Deserialize<BankIDAuthResponse>(data);
                    return status_response;

                }
            }
            catch (Exception ex)
            {
                var x = "";
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
                // Set serializer options
                var json_options = new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    WriteIndented = false,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(request, json_options);
                var content = new StringContent(json);
                content.Headers.ContentType.MediaType = "application/json";
                content.Headers.ContentType.CharSet = "utf-8";

                var timeout = 15000;

                while (true)
                {
                    // Check for a timeout
                    if (timeout <= 0)
                    {
                        var cancelrequest = new BankIDCancelRequest() { OrderRef = request.OrderRef };
                        string canceljson = JsonSerializer.Serialize(request, json_options);
                        var cancelcontent = new StringContent(json);
                        content.Headers.ContentType.MediaType = "application/json";
                        content.Headers.ContentType.CharSet = "utf-8";
                        await client.PostAsync("/cancel", cancelcontent);
                    }

                    var response = await client.PostAsync("/collect", content);
                    if (response.IsSuccessStatusCode == true)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var status_response = JsonSerializer.Deserialize<BankIDCollectResponse>(data);
                        return status_response;

                    }

                    await Task.Delay(2000);
                    timeout -= 2000;

                }
            }
            catch (Exception ex)
            {
                var x = "";
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
                // Set serializer options
                var json_options = new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    WriteIndented = false,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(request, json_options);
                var content = new StringContent(json);
                content.Headers.ContentType.MediaType = "application/json";
                content.Headers.ContentType.CharSet = "utf-8";

                var response = await client.PostAsync("/sign", content);
                if (response.IsSuccessStatusCode == true)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var status_response = JsonSerializer.Deserialize<BankIDAuthResponse>(data);
                    return status_response;

                }
            }
            catch (Exception ex)
            {

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
                // Set serializer options
                var json_options = new JsonSerializerOptions
                {
                    IgnoreNullValues = true,
                    WriteIndented = false,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(request, json_options);
                var content = new StringContent(json);
                content.Headers.ContentType.MediaType = "application/json";
                content.Headers.ContentType.CharSet = "utf-8";

                var response = await client.PostAsync("/cancel", content);
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
                    return qrCode.GetGraphic(320);
                }
            }
        }

    }
}