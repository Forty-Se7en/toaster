using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Listener
{
    public class UDPListener
    {
        public event Action<byte[]> Received = (d) => { };

        public int ListenPort { get; }

        private readonly UdpClient _listener;
        private IPEndPoint _groupEP;

        public UDPListener(int startPort, int endPort)
        {
            for (int i = startPort; i <= endPort; i++) { 
                try
                {
                    _listener = new UdpClient(i); 
                    _groupEP = new IPEndPoint(IPAddress.Loopback, i);
                    ListenPort = i;
                    break;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("UDP LISTENER SocketException: " + ex);
                }
                catch  (Exception ex)
                {
                    Console.WriteLine("UDP LISTENER exception: " + ex);
                }
            }
        }

        public void StartListen()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("StartListen: Waiting for broadcast");
                    byte[] bytes = _listener.Receive(ref _groupEP);

                    new Thread(() =>
                    {
                        Thread.CurrentThread.IsBackground = true;
                        Received(bytes);
                    }).Start();                    

                    Console.WriteLine($"StartListen: Received broadcast from {_groupEP} : {bytes.Length} bytes");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("StartListen SocketException: " + ex);
            }
            finally
            {
                _listener.Close();
            }
        }

        ~UDPListener() {
            try
            {
                _listener.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("~UDPListener exception: " + e);
            }
        }
    }
}
