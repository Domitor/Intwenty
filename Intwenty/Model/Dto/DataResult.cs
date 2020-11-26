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
        public ListFilter ListFilter { get; set; }

        public DataListResult()
        {
            ListFilter = new ListFilter();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            Data = "[]";
        }

        public DataListResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "")
        {
            ListFilter = new ListFilter();
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
            ListFilter = new ListFilter();
            Data = new List<T>();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
        }

        public DataListResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "")
        {
            ListFilter = new ListFilter();
            Data = new List<T>();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            AddMessage(messagecode, message);
        }

        public ListFilter ListFilter { get; set; }

        public List<T> Data { get; set; }
    }

    public class DataListResult<TData,TFilter> : IntwentyResult where TFilter : ListFilter, new()
    {

        public DataListResult()
        {
            ListFilter = new TFilter();
            Data = new List<TData>();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
        }

        public DataListResult(bool success, MessageCode messagecode = MessageCode.RESULT, string message = "")
        {
            ListFilter = new TFilter();
            Data = new List<TData>();
            StartTime = DateTime.Now;
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            AddMessage(messagecode, message);
        }

        public TFilter ListFilter { get; set; }

        public List<TData> Data { get; set; }
    }



}


