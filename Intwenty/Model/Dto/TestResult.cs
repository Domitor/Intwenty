using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Dto
{
    public class TestResult : IntwentyResult
    {
        public TestResult()
        {
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
        }

        public TestResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "")
        {
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            AddMessage(messagecode, message);
        }
    }
}
