using System;
using System.Runtime.Serialization;

namespace DataModels
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string Text { get; private set; }

        [DataMember]
        public User Sender { get; private set; }

        [DataMember]
        public User Recipient { get; private set; }

        [DataMember]
        public DateTime TimeStamp { get; private set; }

        public Message(string text, User sender, User recipient, DateTime timeStamp)
        {
            Text = text;
            Sender = sender;
            Recipient = recipient;
            TimeStamp = timeStamp;
        }
    }
}
