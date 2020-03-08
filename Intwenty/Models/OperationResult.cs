using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Intwenty.Models
{

    public class OperationMessage
    {

        public DateTime Date { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public OperationMessage(string code, string message)
        {
            Date = DateTime.Now;
            Code = code;
            Message = message;
        }

    }

    public class ListRetrivalArgs
    {
        public int ApplicationId { get; set; }

        public string ListViewMetaCode { get; set; }

        public string DataViewMetaCode { get; set; }

        public int MaxCount { get; set; }

        public int CurrentRowNum{ get; set; }

        public int BatchSize { get; set; }

        public string FilterField { get; set; }

        public string FilterValue { get; set; }

        public ListRetrivalArgs()
        {
            BatchSize = 50;
        }

    }

    public class OperationResult
    {
        public ListRetrivalArgs RetriveListArgs { get; set; }

        public List<OperationMessage> Messages { get; set; }

        public bool IsSuccess { get; set; }

        public int Version { get; set; }

        public int ID { get; set; }

        public string Data { get; set; }

        public OperationResult()
        {
            Messages = new List<OperationMessage>();
        }

        public OperationResult(bool success, string resultmessage, int id, int version)
        {
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            ID = id;
            Version = version;
            AddMessage("RESULT", resultmessage);
        }

       
        public bool HasMessage
        {
            get { return Messages.Count > 0; }
        }

        public void AddMessage(string code, string message)
        {
            Messages.Add(new OperationMessage(code, message));
        }

        public void SetError(string systemmsg, string usermsg)
        {
            IsSuccess = false;
            Messages.Add(new OperationMessage("SYSTEMERROR", systemmsg));
            Messages.Add(new OperationMessage("USERERROR", usermsg));
        }

        public string UserError
        {
            get {
                var msg = Messages.Find(p => p.Code == "USERERROR");
                if (msg != null)
                    return msg.Message;

                return string.Empty;
            }
        }
    }

   

    
}
