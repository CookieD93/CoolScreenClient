using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UdpBroadcastCapture
{
    class Program
    {
        // https://msdn.microsoft.com/en-us/library/tst0kwb1(v=vs.110).aspx
        // IMPORTANT Windows firewall must be open on UDP port 11235
        // Use the network EGV5-DMU2 to capture from the local IoT devices
        private const int Port = 11235;
        //private static readonly IPAddress IpAddress = IPAddress.Parse("192.168.5.137"); 
        private static readonly IPAddress IpAddress = IPAddress.Any;
        // Listen for activity on all network interfaces
        // https://msdn.microsoft.com/en-us/library/system.net.ipaddress.ipv6any.aspx
        static void Main()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IpAddress, 11235);

            using (UdpClient socket = new UdpClient(Port))
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast {0}", socket.Client.LocalEndPoint);
                    byte[] datagramReceived = socket.Receive(ref remoteEndPoint);
                    
                    string message = Encoding.ASCII.GetString(datagramReceived, 0, datagramReceived.Length);
                    //Console.WriteLine(message);
                    message = message.Replace('.', ',');
                    double messageDouble = double.Parse(message);
                    InsertTemp(new TemperaturClass(messageDouble));
                }
            }
        }
        private static async Task InsertTemp(TemperaturClass t)
        {
            string data = JsonConvert.SerializeObject(t);
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            using (HttpClient client = new HttpClient())
            {
                await client.PostAsync("http://coolscreenwebservice.azurewebsites.net/Service1.svc/Temperatur", byteContent);
                    //await client.PostAsync("http://localhost:36917/Service1.svc/Temperatur", byteContent);
            }
        }
    }
}
