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

using System.ServiceModel;
using System.Windows;
using ChatServerInterface;
using System.Threading;
using System.Windows.Threading;

namespace ChatClient
{

    public partial class dashBoard : Window
    {
        private IChatServer chatServerProxy;
        private string username;
        bool isButtonCLicked;
        private DispatcherTimer timer;

        public dashBoard(string loggedInUsername, IChatServer chatServer)
        {
            InitializeComponent();
            this.username = loggedInUsername;
            this.chatServerProxy = chatServer;
            msg1.Content = $"Howdy, {username}!";

            // Initialize and start the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Stackpanel refreshed every 1 second
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Fetch chat rooms in a separate thread
            Task.Run(() => fetchChatRooms());
        }

        // Fetch and display available chat rooms
        private void fetchChatRooms()
        {
            var chatRooms = chatServerProxy.GetAvailableChatRooms();
            if (chatRooms != null && chatRooms.Count > 0)
            {
                // Update the UI on the main thread
                this.Dispatcher.Invoke(() =>
                {
                    chatRoomPanel.Children.Clear(); // Clear existing chat rooms
                    foreach (var room in chatRooms)
                    {
                        Button chatRoomButton = new Button();
                        chatRoomButton.Content = room;
                        chatRoomButton.Width = 400;
                        chatRoomButton.Margin = new Thickness(5);
                        chatRoomButton.Click += ChatRoomButton_Click;
                        chatRoomPanel.Children.Add(chatRoomButton);
                    }
                });
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newRoomName = newChatRoomTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(newRoomName))
                {
                    var isCreated = chatServerProxy.CreateChatRoom(newRoomName);
                    if (isCreated)
                    {
                       
                        chatRoomPanel.Children.Clear(); // Clear before refreshing stackPanel
                        fetchChatRooms(); // Refresh stack panel

                        newChatRoomTextBox.Text = string.Empty; // Clear data entry field for new data

                        MessageBox.Show($"Chat Room '{newRoomName}' Created Succesfully", "Ok", MessageBoxButton.OK);


                    }
                    else
                    {
                        MessageBox.Show($"Chat room with the name '{newRoomName}' already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        newChatRoomTextBox.Text = string.Empty;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChatRoomButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string selectedRoom = clickedButton.Content.ToString().Trim();
                if (!string.IsNullOrEmpty(selectedRoom))
                {
                    chatServerProxy.JoinChatRoom(selectedRoom, username);
                    chatRoom chatRoomWindow = new chatRoom(selectedRoom,username, chatServerProxy);

                    //hiding or collapsing the dashboard to make the application easier to navigate.
                    this.Visibility = Visibility.Collapsed;
                    chatRoomWindow.Show();

                }
            }
        }

        private void btnPrivateMessages_Click(object sender, RoutedEventArgs e)
        {
            PrivateMessaging privateMessagingWindow = new PrivateMessaging(username, chatServerProxy);

            //hiding or collapsing the dashboard to make the application easier to navigate.
            this.Visibility= Visibility.Collapsed;
            privateMessagingWindow.Show();
        }
    }
}

