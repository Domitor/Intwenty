using Intwenty.Interface;
using Intwenty.Model;
using Intwenty.Model.BankId;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Intwenty.Services
{
    public class BankIDService : IBankIDService
    {
        

        private readonly HttpClient client;
        private readonly IntwentySettings settings;

        public BankIDService(HttpClient http_client, IOptions<IntwentySettings> options)
        {
            this.client = http_client;
            this.settings = options.Value;
            this.client.BaseAddress = new Uri(this.settings.BankIdBaseAddress);
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<AuthResponse> Auth(AuthRequest request)
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

                var response = await client.PostAsync("/auth", content);
                if (response.IsSuccessStatusCode == true)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var status_response = JsonSerializer.Deserialize<AuthResponse>(data);
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
        public async Task<AuthResponse> Sign(SignRequest request)
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
                    var status_response = JsonSerializer.Deserialize<AuthResponse>(data);
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
        public async Task<CollectResponse> Collect(CollectRequest request)
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

                var response = await client.PostAsync("/collect", content);
                if (response.IsSuccessStatusCode == true)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var status_response = JsonSerializer.Deserialize<CollectResponse>(data);
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
        public async Task<bool> Cancel(CancelRequest request)
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

    }
}