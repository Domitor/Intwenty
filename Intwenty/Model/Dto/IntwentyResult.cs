using System;
using System.Collections.Generic;
using System.Text;

namespace Intwenty.Model.Dto
{
    public enum MessageCode
    {
       RESULT = 0
      ,USERERROR = 1
      ,SYSTEMERROR = 2
      ,WARNING = 3
      ,INFO = 4
    }

    public enum LifecycleStatus
    {
          NONE = 0
         ,NEW_NOT_SAVED = 1
         ,NEW_SAVED = 2
         ,EXISTING_NOT_SAVED = 3
         ,EXISTING_SAVED = 4
         ,DELETED_NOT_SAVED = 5
         ,DELETED_SAVED = 6
    }


    public class IntwentyResult
    {
        public List<OperationMessage> Messages { get; set; }

        public bool IsSuccess { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool HasMessage
        {
            get { return Messages.Count > 0; }
        }

        public double Duration
        {
            get { return EndTime.Subtract(StartTime).TotalMilliseconds; }
        }

        public void Finish()
        {
            EndTime = DateTime.Now;
        }

        public void Finish(MessageCode code, string message)
        {
            EndTime = DateTime.Now;
            Messages.Add(new OperationMessage(code, message));
        }

        public void AddMessage(MessageCode code, string message)
        {
            Messages.Add(new OperationMessage(code, message));
        }

        public void SetError(string systemmsg, string usermsg)
        {
            IsSuccess = false;
            Messages.Add(new OperationMessage(MessageCode.SYSTEMERROR, systemmsg));
            Messages.Add(new OperationMessage(MessageCode.USERERROR, usermsg));
        }

        public void SetSuccess(string msg)
        {
            IsSuccess = true;
            Messages.Add(new OperationMessage(MessageCode.RESULT, msg));
        }

        public string UserError
        {
            get
            {
                var msg = Messages.Find(p => p.Code == MessageCode.USERERROR);
                if (msg != null)
                    return msg.Message;

                return string.Empty;
            }
        }

        public string SystemError
        {
            get
            {
                var msg = Messages.Find(p => p.Code == MessageCode.SYSTEMERROR);
                if (msg != null)
                    return msg.Message;

                return string.Empty;
            }
        }

        public string Result
        {
            get
            {
                var msg = Messages.Find(p => p.Code == MessageCode.RESULT);
                if (msg != null)
                    return msg.Message;

                return string.Empty;
            }
        }

    }
}
