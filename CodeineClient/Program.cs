﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Codeine;
using System.IO;

namespace CodeineClient
{
    class Program
    {

        private const int udpPortSrv = 4568;
        private const int udpPortCli = 4569;
        private static bool dataReceived;
        private static bool loop;
        
        static void Main(string[] args)
        {
            loop = true;
            do{
                dataReceived = false;
                Console.WriteLine("Connectig to localhost...");
                UdpClient uClient;
                try
                {
                    uClient = new UdpClient();
                    uClient.Connect("localhost", udpPortSrv);
                }
                catch (System.Net.Sockets.SocketException e) {

                    Console.WriteLine("Connection Failed: {0}", e.ToString());
                    return;
                }

                Console.WriteLine("Connection succeded!");
                Console.WriteLine("Press 's' to send SET message or 'g' for GET message...");
              
                string cmd = Console.ReadLine();
                CodeineMessage msg = null;

                if (cmd.Equals("g"))
                {
                    Console.WriteLine("Sending GET Message...");
                    msg = new CodeineMessage(_t_CDMSG._t_MSGGET, (byte)_t_MSGGET.ips);
                }
                if (cmd.Equals("s")) 
                {
                    Console.WriteLine("Sending SET Message...");
                    string ipAddr = "192.169.5.13";
                    System.Text.ASCIIEncoding  encoding=new System.Text.ASCIIEncoding();
                    byte[] encodedIP = encoding.GetBytes(ipAddr);
                    msg = new CodeineMessage(_t_CDMSG._t_MSGSET, (byte)_t_MSGSET.ip, encodedIP);
                    msg.cdByteValue = 15;
                }

                byte[] bytes = msg.ToByteArray();
                uClient.Send(bytes, bytes.Length);
                Console.WriteLine("Handshake completed!");
                
                if(cmd.Equals("g")){
                    Console.WriteLine("Waiting for data...");
                    
                    byte[] data;

                    try
                    {
                        IPEndPoint remote = new IPEndPoint(IPAddress.Any, udpPortCli);
                        data = uClient.Receive(ref remote);
                        PackedContactDescriptors packedDescs = StructConverter.fromArray(data);
                        uClient.Close();
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine("Socket Raised exception in receive {0}", e.ToString());
                    }
                }
                Console.WriteLine("Loop? Y/n");
                string ans = Console.ReadLine();

                if (ans.Equals("n")) 
                    loop = false;

            } while(loop);
 
        }

 
    }
}
