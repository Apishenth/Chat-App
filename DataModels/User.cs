using System;
using System.Runtime.Serialization;

namespace DataModels
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Name { get; private set; }

        public User(string name)
        {
            Name = name;
        }
    }
}
