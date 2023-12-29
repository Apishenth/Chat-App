using ChatServerInterface;
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
using System.Windows.Threading;

namespace ChatClient
{
    public partial class PrivateMessaging : Window
    {
        private IChatServer chatServerProxy;
        private string username;
        private DispatcherTimer updateTimer;

        public PrivateMessaging(string pUsername, IChatServer chatServer)
        {
            this.chatServerProxy = chatServer;
            this.username = pUsername;

            InitializeComponent();

            inboxLabel.Content = $"{username}'s inbox";  // Sets the label content


            LoadAllUsers();  // Load the list of all users

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(1);  // Set the interval to 1 second
            updateTimer.Tick += UpdateTimer_Tick;  // Assign the event handler
            updateTimer.Start();
        }


        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            LoadPrivateMessages();
            UpdateUserListDropdown();  // Method to handle updating the dropdown
        }

        private void UpdateUserListDropdown()
        {
            // Store the currently selected user
            var selectedUser = (string)userListDropdown.SelectedItem;

            // Reload the list of users
            var users = chatServerProxy.GetAllUsers();
            userListDropdown.ItemsSource = users;

            // If the previously selected user is still in the list, reselect them
            if (users.Contains(selectedUser))
            {
                userListDropdown.SelectedItem = selectedUser;
            }
            else if (users.Any())
            {
                userListDropdown.SelectedIndex = 0;  // Select the first user if the previous selection no longer exists
            }
        }

        private void LoadPrivateMessages()
        {
            var messages = chatServerProxy.GetPrivateMessagesForUser(username);
            privateMessageInbox.ItemsSource = messages;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            updateTimer.Stop();
        }

        // This method fetches and displays all users in the ComboBox
        private void LoadAllUsers()
        {
            var users = chatServerProxy.GetAllUsers();
            userListDropdown.ItemsSource = users;
            userListDropdown.SelectedIndex = 0;  // Select the first user by default
        }

        // Event handler for the send button
        private void BtnSendPrivateMessage_Click1(object sender, RoutedEventArgs e)
        {
            string selectedUser = (string)userListDropdown.SelectedItem;
            if (!string.IsNullOrEmpty(selectedUser) && !string.IsNullOrEmpty(privateMessageInput.Text))
            {
                chatServerProxy.SendPrivateMessage( username, selectedUser, privateMessageInput.Text);
                privateMessageInput.Clear();
                LoadPrivateMessages();  // Refresh the message inbox after sending a message
            }
        }
    }
}
