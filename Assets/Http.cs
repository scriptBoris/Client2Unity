using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Http : MonoBehaviour
{
    public Text uText;

    private string _url;
    private int _port;
    private TcpClient _socketConnection;
    private Thread _clientReceiveThread;

    // Use this for initialization 	
    void Start()
    {
        ConnectToTcpServer("localhost", 8000);
        Cmd("Connect to server...");
    }

    void Cmd(string info)
    {
        print(info);
        //uText.text += info;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessage("Message from Unity client");
        }
    }

    // Setup socket connection. 	
    private void ConnectToTcpServer(string url, int port)
    {
        try
        {
            _url = url;
            _port = port;
            //StartCoroutine(ListenForData() );
            _clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            _clientReceiveThread.IsBackground = true;
            _clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Cmd("On client connect exception " + e);
        }
    }

    // Runs in background clientReceiveThread; Listens for incomming data. 	
    private void ListenForData()
    {
        //yield return null;
        try
        {
            _socketConnection = new TcpClient(_url, _port);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading
                using (NetworkStream stream = _socketConnection.GetStream() )
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Cmd("server message received as: " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Cmd("Socket exception: " + socketException);
        }
    }

    // Send message to server using socket connection. 	
    private void SendMessage(string clientMessage)
    {
        if (_socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = _socketConnection.GetStream();
            if (stream.CanWrite)
            {
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Cmd("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Cmd("Socket exception: " + socketException);
        }
    }
}
