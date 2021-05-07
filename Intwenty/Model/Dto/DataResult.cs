using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Dto
{
    public class DataResult : IntwentyJSONStringResult
    {
        public int Version { get; set; }

        public int Id { get; set; }

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

      

    }
    public class DataListResult : IntwentyJSONStringResult
    {
        public ClientOperation CurrentOperation { get; set; }

        public DataListResult()
        {
            CurrentOperation = new ClientOperation();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            Data = "[]";
        }

        public DataListResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "")
        {
            CurrentOperation = new ClientOperation();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            AddMessage(messagecode, message);
            Data = "[]";
        }

    }

    public class DataResult<T> : IntwentyResult
    {
        public int Version { get; set; }

        public int Id { get; set; }

        public T Data { get; set; }

        public DataResult()
        {
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            Data = default(T);
        }

        public DataResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "", int id = 0, int version = 0)
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

    public class DataListResult<T> : IntwentyResult
    {


        public DataListResult()
        {
            CurrentOperation = new ClientOperation();
            Data = new List<T>();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
        }

        public DataListResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "")
        {
            CurrentOperation = new ClientOperation();
            Data = new List<T>();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            AddMessage(messagecode, message);
        }

        public ClientOperation CurrentOperation { get; set; }

        public List<T> Data { get; set; }
    }

    public class DataListResult<TData, TOperation> : IntwentyResult where TOperation : ClientOperation, new()
    {

        public DataListResult()
        {
            CurrentOperation = new TOperation();
            Data = new List<TData>();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
        }

        public DataListResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "")
        {
            CurrentOperation = new TOperation();
            Data = new List<TData>();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            AddMessage(messagecode, message);
        }

        public TOperation CurrentOperation { get; set; }

        public List<TData> Data { get; set; }
    }



}


