using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Dto
{
    public class OperationResult : IntwentyResult
    {
        public OperationResult()
        {
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
        }

        public OperationResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "")
        {
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            AddMessage(messagecode, message);
        }
    }
}
