using ChatServerInterface;
using DataModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;

namespace ChatClient
{

    public partial class chatRoom : Window
    {
        private IChatServer chatServerProxy;
        private string username;
        private string currentChatRoomName;
        private Timer refreshTimer;
        private ObservableCollection<string> messageList = new ObservableCollection<string>(); 
        private static OpenFileDialog fileUploader = new OpenFileDialog();
        public static bool isfileShared = false;


        public chatRoom(string pCurrentChatRoomName, string pUsername, IChatServer chatServer)
        {
            this.chatServerProxy = chatServer;
            this.username = pUsername; // Username passed from dashboard
            this.currentChatRoomName = pCurrentChatRoomName; // Current Chat Room name passed from dashboard

            InitializeComponent();

            lblUsername.Content = $"User: {username}";
            lblChatRoomName.Content = $"Chatroom: {currentChatRoomName}";

            // Binding the ObservableCollection to the ListBox for real-time updates
            lstChatMessages.ItemsSource = messageList;

            // Initialize the timer to call RefreshMessages every 5 seconds (5000 milliseconds)
            refreshTimer = new Timer(RefreshMessages, null, 0, 1000);
            
        }

        private DateTime ExtractTimestamp(string message)
        {
            // Message format used is "2023-09-25 10:45:23: User to Recipient - Message Body"
            var splits = message.Split(':');
            var timestampString = new string[] { splits[0], splits[1], splits[2] };
            DateTime.TryParse(string.Join(":", timestampString), out DateTime timestamp);
            return timestamp;
        }

        private void RefreshMessages(object state)
        {
            // Fetch the latest message. 
            string latestMessage = chatServerProxy.GetLatestMessage(currentChatRoomName, username);

            if (latestMessage != null && !string.IsNullOrWhiteSpace(latestMessage) && !latestMessage.Equals("No latest message found.") && !messageList.Contains(latestMessage))
            {
                // Use the Dispatcher to execute the update on the UI thread.
                this.Dispatcher.Invoke(() =>
                {
                    // Add the message to the ObservableCollection
                    messageList.Add(latestMessage);

                    // Sort messages based on extracted timestamps
                    var sortedList = messageList.OrderBy(ExtractTimestamp).ToList();
                    messageList.Clear();
                    foreach (var msg in sortedList)
                    {
                        messageList.Add(msg);
                    }
                });
            }
        }


        private void btnSendMessage_Click1(object sender, RoutedEventArgs e)
        {
            string message = txtChatMessage.Text;
           
            if (!string.IsNullOrEmpty(message))
            {
                chatServerProxy.SendGroupMessage(currentChatRoomName, txtChatMessage.Text, username);
                txtChatMessage.Clear();
            }
        }

        // Stops the timer when the window is closed to avoid unnecessary operations
        protected override void OnClosed(EventArgs e)
        {
            refreshTimer.Dispose();
            base.OnClosed(e);
        }

        // File Sharing implementation
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //Limiting the number of files that can be shared at once to 1.
            fileUploader.FilterIndex = 1;

            //Show the relevant message in the chatroom
            if(fileUploader.ShowDialog()==true)
            {

                txtChatMessage.Text = "shared a file, go to C:\\DC_downloads to see!";
                isfileShared = true;

                //Getting only the fileName
                string fileName = fileUploader.SafeFileName;

                //Check whether the directory exists before creating one
                if (Directory.Exists("C:\\DC_Downloads") != true)
                {
                    //Creating a directory to store the shared files.
                    Directory.CreateDirectory("C:\\DC_Downloads");
                }

                //Setting up the path to save the file
                var newFile = System.IO.Path.Combine("C:\\DC_downloads\\" + fileName);

                if (File.Exists(newFile) == false)
                {
                    //Saving the file
                    File.Copy(fileUploader.FileName, newFile);

                }
                else
                {
                    //Renaming the file if there's already a file that exists in the same name.
                    fileName = "copyOf "+fileUploader.SafeFileName;
                    newFile = System.IO.Path.Combine("C:\\DC_downloads\\" + fileName);

                    File.Copy(fileUploader.FileName, newFile);
                }

                //Showing a simple message after sharing the file to notify the user.
                MessageBox.Show("Successfully shared the file : )");

            }
            
        }

    }
}
