using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Dto
{
    public class DataResult : IntwentyResult
    {
        public int Version { get; set; }

        public int Id { get; set; }

        public string Data { get; set; }

        public DataResult()
        {
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            Data = "{}";
        }

        public DataResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "", int id = 0, int version = 0)
        {
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            Id = id;
            Version = version;
            Data = "{}";
            AddMessage(messagecode, message);
        }

        public ApplicationData GetAsApplicationData()
        {
            if (string.IsNullOrEmpty(Data))
                return new ApplicationData();

            if (Data.Length < 5)
                return new ApplicationData();

            var model = System.Text.Json.JsonDocument.Parse(Data).RootElement;
            var result = ApplicationData.CreateFromJSON(model);


            return result;

        }

        public ClientStateInfo CreateClientState()
        {
            var model = System.Text.Json.JsonDocument.Parse(Data).RootElement;
            var result = ClientStateInfo.CreateFromJSON(model);
            return result;
        }

        public void AddApplicationJSON(string jsonname, object jsonvalue, bool isnumeric = false)
        {
            var check = Data.IndexOf("{", 2);
            if (check < 2)
                return;

            check = Data.IndexOf("}", check);
            if (check < 2)
                return;


            string value = string.Empty;
            if (!isnumeric)
                value = ",\"" + jsonname + "\":" + "\"" + System.Text.Json.JsonEncodedText.Encode(Convert.ToString(jsonvalue)).ToString() + "\"";
            else
                value = ",\"" + jsonname + "\":" + System.Text.Json.JsonEncodedText.Encode(Convert.ToString(jsonvalue)).ToString();

            Data = Data.Insert(check, value);

        }

        public void RemoveJSON(string jsonname)
        {
            var cnt = 0;

            if (string.IsNullOrEmpty(jsonname))
                return;

            if (jsonname.Length < 2)
                return;

            while (Data.IndexOf(jsonname) > -1 && cnt < 1000)
            {
                cnt += 1;

                var nameindex = Data.IndexOf(jsonname);
                if (nameindex < 0)
                    continue;

                var test = Data.IndexOf(":", nameindex + jsonname.Length);
                if ((nameindex + jsonname.Length + 3) < test)
                    continue;

                var startindex = Data.LastIndexOf(",", nameindex);
                test = Data.LastIndexOf("{", nameindex);
                if (test > startindex)
                    startindex = test;

                var endindex = Data.IndexOf(",", startindex + 1);
                test = Data.IndexOf("}", startindex + 1);
                if (endindex == -1)
                {
                    endindex = test;
                }
                else
                {
                    if (test < endindex && test > -1)
                        endindex = test;
                }

                var count = (endindex - startindex);
                if (count < 3)
                    continue;

                Data = Data.Remove(startindex, count);

            }
        }

        public class DataListResult : IntwentyResult
        {
            public DataListResult()
            {
                StartTime = DateTime.Now;
                Messages = new List<OperationMessage>();
                Data = "[]";
            }

            public DataListResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "")
            {
                StartTime = DateTime.Now;
                Messages = new List<OperationMessage>();
                IsSuccess = success;
                AddMessage(messagecode, message);
                Data = "[]";
            }

            public string Data { get; set; }
        }

        public class TypedDataResult<T> : IntwentyResult
        {
            public int Version { get; set; }

            public int Id { get; set; }

            public T Data { get; set; }

            public TypedDataResult()
            {
                StartTime = DateTime.Now;
                Messages = new List<OperationMessage>();
                Data = default(T);
            }

            public TypedDataResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "", int id = 0, int version = 0)
            {
                Data = default(T);
                StartTime = DateTime.Now;
                Messages = new List<OperationMessage>();
                IsSuccess = success;
                Id = id;
                Version = version;
                AddMessage(messagecode, message);
            }
        }

        public class TypedDataListResult<T> : IntwentyResult
        {
            public TypedDataListResult()
            {
                Data = new List<T>();
                StartTime = DateTime.Now;
                Messages = new List<OperationMessage>();
            }

            public TypedDataListResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "")
            {
                new List<T>();
                StartTime = DateTime.Now;
                Messages = new List<OperationMessage>();
                IsSuccess = success;
                AddMessage(messagecode, message);
            }

            public List<T> Data { get; set; }
        }



    }

}
