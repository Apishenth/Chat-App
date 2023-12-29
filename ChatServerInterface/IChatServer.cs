using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Reflection.Emit;

namespace ChatServerInterface
{
    [ServiceContract]
    public interface IChatServer
    {
        [OperationContract]
        bool Login(string username);

        [OperationContract]
        bool Logout(string username);

        [OperationContract]
        void JoinChatRoom(string roomName, string username);

        [OperationContract]
        void SendGroupMessage(string roomName, string message, string senderUsername);

        [OperationContract]
        void SendPrivateMessage(string message, string senderUsername, string recipientUsername);

        [OperationContract]
        string GetLatestMessage(string roomName, string username);

        [OperationContract]
        bool CreateChatRoom(string roomName);

        [OperationContract]
        List<string> GetAvailableChatRooms();

        [OperationContract]
        void AddPrivateMessage(string recipientUsername, DataModels.Message message);

        [OperationContract]
        List<DataModels.Message> GetPrivateMessagesForUser(string username);

        [OperationContract]
        List<string> GetAllUsers();


    }
}
