using System;
using System.ServiceModel;
using System.Windows;
using ChatServerInterface;

namespace ChatClient
{
    public partial class MainWindow : Window
    {
        private IChatServer chatServerProxy;

        public MainWindow()
        {
            InitializeComponent();

            // Set up the connection to the chat server
            ChannelFactory<IChatServer> chatServerFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string chatServerURL = "net.tcp://localhost:8201/ChatServer";
            chatServerFactory = new ChannelFactory<IChatServer>(tcp, chatServerURL);
            chatServerProxy = chatServerFactory.CreateChannel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please enter a valid username.");
                return;
            }

            // Attempt to log in using the WCF service
            var result = chatServerProxy.Login(txtUsername.Text);

            if (result)
            {
                // Successful login
                MessageBox.Show($"Welcome, {username}!");

                // Transition to the dashBoard window
                var dashboardWindow = new dashBoard(username, chatServerProxy);
                dashboardWindow.Show();
                ///this.Close();  // Optionally close the login window
            }
            else
            {
                // Failed login
                MessageBox.Show($"Username {username} is already in use. Please choose another.");
            }


        }
    }
}
