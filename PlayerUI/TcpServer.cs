﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proximity
{
    class TcpServer
    {
        private TcpListener _server;
        private Boolean _isRunning;
        public static List<string> recievedData = new List<string>();
        public static List<TcpClient> connectedClinets = new List<TcpClient>();
        public TcpServer(int port)
        {
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();

            _isRunning = true;

            LoopClients();
        }

        public void LoopClients()
        {
            while (_isRunning)
            {
                // wait for client connection
                TcpClient newClient = _server.AcceptTcpClient();

                // client found.
                // create a thread to handle communication
                connectedClinets.Add(newClient);
                //clientsIPs.Add(((IPEndPoint)newClient.Client.RemoteEndPoint).Address.ToString());
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Start(newClient);
            }
        }
        string dataReceived;
        private string getClientData(TcpClient client)
        {
            NetworkStream nwStream3 = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream3.Read(buffer, 0, client.ReceiveBufferSize);
            return dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

        }

        public void HandleClient(object obj)
        {
            // retrieve client from parameter passed to thread
            System.Net.Sockets.TcpClient client = (TcpClient)obj;

            // sets two streams
            //StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            //StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            // you could use the NetworkStream to read and write, 
            // but there is no forcing flush, even when requested

            Boolean bClientConnected = true;
            String sData = null;

            while (bClientConnected)
            {
                // reads from stream
                sData = getClientData(client);

                // shows content on the console.
                recievedData.Add(sData);

                // to write something back.
                // sWriter.WriteLine("Meaningfull things here");
                // sWriter.Flush();
            }
        }
    }
}
