using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class ChatRoom
    {
        public string Name { get; }
        public List<User> Users { get; }
        public List<Message> Messages { get; }

        public ChatRoom(string name)
        {
            Name = name;
            Users = new List<User>();
            Messages = new List<Message>();
        }

        public void addUser(User user)
        {
            if (Users == null) return;
            Users.Add(user);
        }

        public void recordMessage(Message message)
        {
            if (message == null) return;
            Messages.Add(message);
        }

        public bool HasUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            // Check if a user with the specified username is in the Users list
            return Users.Any(user => user.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        }
    }
}
