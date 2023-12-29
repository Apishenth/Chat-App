using ChatServerInterface;
using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]

    internal class ServerImplementation : IChatServer
    {
        private Dictionary<string, List<Message>> privateMessages = new Dictionary<string, List<Message>>();

        private List<ChatRoom> ActiveChatRooms;
        private List<User> LoggedInUsers;
        internal ServerImplementation()
        {
            ActiveChatRooms = new List<ChatRoom>();
            LoggedInUsers = new List<User>();
        }

        /* Adds a new user object to the list of users who are currently logged in to the system.
         * Returns a boolean as the status message.
         * true = success
         * false = failed
        */
        public bool Login(string username)
        {
            if (username == null || username == "") throw new ArgumentNullException("username");
            bool isLoggedInSucess = true;
            foreach (var user in LoggedInUsers)
            {
                if (user.Name.Equals(username))
                {
                    isLoggedInSucess = false;
                    break;
                }
            }
            User newUser = new User(username);
            LoggedInUsers.Add(newUser);
            return isLoggedInSucess;
        }

        /* Removes a user from the list of logged in user
         * Returns a boolean value as the status message.
         * true = logged out successfully
         * false = logout failed
         */
        public bool Logout(string username)
        {

            if (username == null) throw new ArgumentNullException("username");
            bool isLogoutSuccess = false;
            for (int i = LoggedInUsers.Count - 1; i >= 0; i--)
            {
                if (LoggedInUsers[i].Name.Equals(username))
                {
                    LoggedInUsers.RemoveAt(i);
                    isLogoutSuccess = true;
                    break;
                }
            }
            return isLogoutSuccess;
        }

        /* Checks if a user is currently logged into the system */
        private bool IsLoggedIn(string username)
        {
            foreach (var user in LoggedInUsers)
            {
                if (user.Name.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    return true; // user is logged in
                }
            }
            return false; // user is not logged in
        }

        /* Gets a user a object from the list of currently logged in users */
        private User GetUser(string username)
        {
            User user = null;
            foreach (var loggedUser in LoggedInUsers)
            {
                if (loggedUser.Name.Equals(username))
                {
                    user = loggedUser;
                    break;
                }
            }
            return user;
        }

        /* Joins a chat room or creates a new one if it doesn't exist, adding the user to the room. */
        public void JoinChatRoom(string roomName, string username)
        {

            if (roomName == null || username == null) throw new ArgumentNullException();
            if (!IsLoggedIn(username)) return; // User is not logged in

            ChatRoom chatRoom = ActiveChatRooms.FirstOrDefault(room => room.Name.Equals(roomName));

            if (chatRoom == null)
            {
                // Create a new chat room if it doesn't exist
                chatRoom = new ChatRoom(roomName);
                ActiveChatRooms.Add(chatRoom);
            }
            else if (chatRoom.HasUser(username))
            {
                // User is already a member of the chat room; no action needed
                return;
            }

            // Add the user to the chat 
            User user = GetUser(username);
            chatRoom.addUser(user);
        }

        public bool CreateChatRoom(string roomName)
        {
            if (ActiveChatRooms.Any(room => room.Name.Equals(roomName, StringComparison.OrdinalIgnoreCase)))
                return false;  // Chat room with the same name already exists

            var newChatRoom = new ChatRoom(roomName);
            ActiveChatRooms.Add(newChatRoom);
            return true;  // Chat room created successfully
        }

        public List<string> GetAvailableChatRooms()
        {
            return ActiveChatRooms.Select(room => room.Name).ToList();
        }



        public string GetLatestMessage(string roomName, string username)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(roomName)) throw new ArgumentNullException("roomName");

            ChatRoom chatRoom = ActiveChatRooms.FirstOrDefault(room => room.Name.Equals(roomName));
            if (chatRoom != null && chatRoom.HasUser(username) && chatRoom.Messages.Count > 0)
            {
                var latestMessage = chatRoom.Messages.Last();
                var recipientName = latestMessage.Recipient != null ? latestMessage.Recipient.Name : "Group";
                return $"{latestMessage.TimeStamp}: {latestMessage.Sender.Name} to {recipientName} - {latestMessage.Text}";
            }

            return "No latest message found.";
        }



        public void SendGroupMessage(string roomName, string message, string senderUsername)
        {
            if (string.IsNullOrWhiteSpace(senderUsername)) throw new ArgumentNullException("senderUsername");
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException("message");
            if (string.IsNullOrWhiteSpace(roomName)) throw new ArgumentNullException("roomName");

            ChatRoom chatRoom = ActiveChatRooms.FirstOrDefault(room => room.Name.Equals(roomName));
            if (chatRoom != null && chatRoom.HasUser(senderUsername))
            {
                var sender = LoggedInUsers.FirstOrDefault(u => u.Name.Equals(senderUsername, StringComparison.OrdinalIgnoreCase));
                if (sender == null) throw new Exception("Sender not found.");

                var newMessage = new Message(message, sender, null, DateTime.Now);
                chatRoom.recordMessage(newMessage);
            }
        }



        public void SendPrivateMessage(string senderUsername, string recipientUsername, string messageContent)
        {
            var sender = LoggedInUsers.FirstOrDefault(u => u.Name.Equals(senderUsername, StringComparison.OrdinalIgnoreCase));
            var recipient = LoggedInUsers.FirstOrDefault(u => u.Name.Equals(recipientUsername, StringComparison.OrdinalIgnoreCase));

            if (sender == null || recipient == null) throw new Exception("Sender or Recipient not found.");

            var newMessage = new Message(messageContent, sender, recipient, DateTime.Now);

            AddPrivateMessage(recipientUsername, newMessage);
        }


        public void AddPrivateMessage(string recipientUsername, Message message)
        {
            if (!privateMessages.ContainsKey(recipientUsername))
            {
                privateMessages[recipientUsername] = new List<Message>();
            }

            privateMessages[recipientUsername].Add(message);
        }

        public List<Message> GetPrivateMessagesForUser(string username)
        {
            if (privateMessages.ContainsKey(username))
            {
                return privateMessages[username];
            }

            return new List<Message>();
        }

        public List<string> GetAllUsers()
        {
            return LoggedInUsers.Select(u => u.Name).ToList();
        }


        /* TEST FUNCTIONS DELETE BEFORE SUBMISSION */
        private void displayUsers()
        {
            foreach (var user in LoggedInUsers)
            {
                Console.WriteLine(user.Name);
            }

        }

        private void displayChats()
        {
            foreach (var chat in ActiveChatRooms)
            {
                Console.WriteLine(chat.Name);
                foreach (var user in chat.Users)
                {
                    Console.WriteLine(user.Name);
                }
            }

        }
    }
}
