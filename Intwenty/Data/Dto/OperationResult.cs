using System;
using System.Collections.Generic;

namespace Intwenty.Data.Dto
{

    public enum LifecycleStatus
    {
        NONE = 0
      , NEW_NOT_SAVED = 1
      , NEW_SAVED = 2
      , EXISTING_NOT_SAVED = 3
      , EXISTING_SAVED = 4
      , DELETED_NOT_SAVED = 5
      , DELETED_SAVED = 6
    }

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

        public string OwnerUserId { get; set; }

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

    public class RetrivalExpression
    {
      
        public List<RetrivalExpressionValue> ValueExpressions { get; set; }

        public RetrivalExpression()
        {
            ValueExpressions = new List<RetrivalExpressionValue>();
        }

        public string GetExpression()
        {
            var res = "((1=1)";
            foreach (var e in ValueExpressions)
                res += " AND (" + e.GetExpression() + ")";


            res += ")";
            return res;
        }

    }

    public class RetrivalExpressionValue
    {
        public enum RetrivalFilterOperator { LessThan, LargerThan, Equal, NotEqual, Like }
        public enum RetrivalFilterDataType { String, Numeric }

        public string DbName { get; set; }

        public RetrivalFilterOperator Operator { get; set; }

        public RetrivalFilterDataType DataType { get; set; }

        public object Value { get; set; }

        private string GetOperatorText()
        {
            if (Operator == RetrivalFilterOperator.Equal)
                return "=";
            if (Operator == RetrivalFilterOperator.NotEqual)
                return "<>";
            if (Operator == RetrivalFilterOperator.LargerThan)
                return ">";
            if (Operator == RetrivalFilterOperator.LessThan)
                return "<";
            if (Operator == RetrivalFilterOperator.LessThan)
                return "LIKE";

            return string.Empty;
        }

        public string GetExpression()
        {
            if (DataType == RetrivalFilterDataType.String)
            {
                var res = this.DbName + " " + GetOperatorText() + " '"+Convert.ToString(Value)+"'";
                if (Operator== RetrivalFilterOperator.Like)
                    res = this.DbName + " " + GetOperatorText() + " '%" + Convert.ToString(Value) + "%'";

                return res;
            }
            else
            {
                return this.DbName + " " + GetOperatorText() + " " + Convert.ToString(Value);
            }
        }

        public RetrivalExpressionValue()
        {
            Operator = RetrivalFilterOperator.Equal;
            DataType = RetrivalFilterDataType.String;
        }

        public static RetrivalExpressionValue GetEqualFilter(string dbname, int value)
        {
            return new RetrivalExpressionValue() { DataType= RetrivalFilterDataType.Numeric, DbName = dbname, Operator = RetrivalFilterOperator.Equal, Value = value };
        }

        public static RetrivalExpressionValue GetEqualFilter(string dbname, string value)
        {
            return new RetrivalExpressionValue() { DataType = RetrivalFilterDataType.String, DbName = dbname, Operator = RetrivalFilterOperator.Equal, Value = value };
        }

        public static RetrivalExpressionValue GetNotEqualFilter(string dbname, int value)
        {
            return new RetrivalExpressionValue() { DataType = RetrivalFilterDataType.Numeric, DbName = dbname, Operator = RetrivalFilterOperator.NotEqual, Value = value };
        }

        public static RetrivalExpressionValue GetNotEqualFilter(string dbname, string value)
        {
            return new RetrivalExpressionValue() { DataType = RetrivalFilterDataType.String, DbName = dbname, Operator = RetrivalFilterOperator.NotEqual, Value = value };
        }

        public static RetrivalExpressionValue GetLikeFilter(string dbname, int value)
        {
            return new RetrivalExpressionValue() { DataType = RetrivalFilterDataType.Numeric, DbName = dbname, Operator = RetrivalFilterOperator.Like, Value = value };
        }

        public static RetrivalExpressionValue GetLikeFilter(string dbname, string value)
        {
            return new RetrivalExpressionValue() { DataType = RetrivalFilterDataType.String, DbName = dbname, Operator = RetrivalFilterOperator.Like, Value = value };
        }

    }

    public class OperationResult
    {
        public LifecycleStatus Status { get; set; }

        public ListRetrivalArgs RetriveListArgs { get; set; }

        public List<OperationMessage> Messages { get; set; }

        public bool IsSuccess { get; set; }

        public int Version { get; set; }

        public int Id { get; set; }

        public string Data { get; set; }

        public OperationResult()
        {
            Messages = new List<OperationMessage>();
            Data = "{}";
        }

        public OperationResult(bool success, string resultmessage="", int id=0, int version=0)
        {
            Messages = new List<OperationMessage>();
            IsSuccess = success;
            Id = id;
            Version = version;
            Data = "{}";
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

        public void SetSuccess(string msg)
        {
            IsSuccess = true;
            Messages.Add(new OperationMessage("RESULT", msg));
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

        public string SystemError
        {
            get
            {
                var msg = Messages.Find(p => p.Code == "SYSTEMERROR");
                if (msg != null)
                    return msg.Message;

                return string.Empty;
            }
        }

        public string Result
        {
            get
            {
                var msg = Messages.Find(p => p.Code == "RESULT");
                if (msg != null)
                    return msg.Message;

                return string.Empty;
            }
        }
    }

   

    
}
