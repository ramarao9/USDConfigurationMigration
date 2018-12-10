using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.WPF.Models
{
    public class MessageNotification : NotificationBase
    {


        private int _messageType;
        public int MessageType
        {
            get { return _messageType; }
            set
            {
                if (_messageType != value)
                {
                    _messageType = value;
                    RaisePropertyChanged(() => MessageType);
                    RaisePropertyChanged(() => IconSrc);
                }
            }
        }


        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    RaisePropertyChanged(() => Message);
                }
            }
        }

        public string IconSrc
        {

            get
            {

                switch (MessageType)
                {
                    case (int)NotificationType.Info: return "/Assets/Info_16.png";


                    case (int)NotificationType.Warning:
                        return "/Assets/warning_31.png";


                    case (int)NotificationType.Error:
                        return "/Assets/error_n.png";

                    default: return "/Assets/Info_16.png";
                }

            }
        }
    }
}
