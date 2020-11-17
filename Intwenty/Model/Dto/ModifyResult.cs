using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Dto
{
    public class ModifyResult : IntwentyResult
    {
        public LifecycleStatus Status { get; set; }

        public int Version { get; set; }

        public int Id { get; set; }


        public ModifyResult()
        {
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
        }

        public ModifyResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "", int id = 0, int version = 0)
        {
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            Id = id;
            Version = version;
            AddMessage(messagecode, message);
        }

    }

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
