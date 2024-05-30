using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        private static TcpClient client;
        private static NetworkStream stream;
        private static string clientName;

        static void Main(string[] args)
        {
            ConnectToServer();
        }

        static void ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 12345);
                stream = client.GetStream();

                Console.Write("Enter your name: ");
                clientName = Console.ReadLine();
                SendNameToServer(clientName); // Send the client's name to the server

                Console.WriteLine($"Connected to server as '{clientName}'. Start typing messages. Type 'exit' to quit.");

                Thread receiveThread = new Thread(ReceiveMessages);
                receiveThread.Start();

                while (true)
                {
                    string message = Console.ReadLine();
                    if (message.ToLower() == "exit")
                    {
                        DisconnectFromServer();
                        break;
                    }
                    SendMessageToServer(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                DisconnectFromServer();
            }
        }

        static void ReceiveMessages()
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Disconnected from server.");
                        DisconnectFromServer();
                        break;
                    }
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(receivedMessage); // Display received message directly
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while receiving messages: {ex.Message}");
                DisconnectFromServer();
            }
        }

        static void SendMessageToServer(string message)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending message: {ex.Message}");
                DisconnectFromServer();
            }
        }

        static void SendNameToServer(string name)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(name);
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while sending name: {ex.Message}");
                DisconnectFromServer();
            }
        }

        static void DisconnectFromServer()
        {
            try
            {
                stream?.Close();
                client?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while disconnecting from server: {ex.Message}");
            }
            finally
            {
                Environment.Exit(0);
            }
        }
    }
}
