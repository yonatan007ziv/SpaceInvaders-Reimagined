using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvadersServer
{
    internal class ClientLoginRegistValidator
    {
        public delegate bool WhenRead(string username, string password);
        WhenRead del;
        Byte[] buffer;
        TcpClient client;
        public ClientLoginRegistValidator(TcpClient client, WhenRead del)
        {
            this.del += del;

            buffer = new Byte[4096];
            this.client = client;

            client.GetStream().BeginRead(buffer, 0, buffer.Length, read, null);
        }
        void read(IAsyncResult aR)
        {
            string msg = Encoding.UTF8.GetString(buffer);

            string username = msg.Split('/')[0];
            string password = msg.Split('/')[1];
            Console.WriteLine(username);
            Console.WriteLine(password);
            bool okCredentials = del(username,password);

            if(okCredentials)
            {
                write("OK LOGIN");
                Console.WriteLine("nice");
            }
            else
                Console.WriteLine("wrong");
        }
        void write(string msg)
        {
            Byte[] buffer = Encoding.UTF8.GetBytes(msg);
            client.GetStream().Write(buffer, 0, buffer.Length);
        }
    }
}
