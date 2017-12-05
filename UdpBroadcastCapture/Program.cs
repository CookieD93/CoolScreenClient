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
        // IMPORTANT Windows firewall must be open on UDP port 7000
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
                    Console.WriteLine("Receives {0} bytes from {1} port {2} message {3}", datagramReceived.Length,
                        remoteEndPoint.Address, remoteEndPoint.Port, message);
                    string temptosend = message;
                    Console.WriteLine(temptosend);
                    //string[] SplittedMessage = message.Split();

                    //Console.WriteLine("####");
                    //foreach (string s in SplittedMessage)
                    //{
                    //    Console.WriteLine(s);
                    //}
                    //Console.WriteLine("####");
                    //string temptosend = SplittedMessage[1];
                    
                    InsertTemp(new TemperaturClass(double.Parse(temptosend)));

                    Thread.Sleep(15000);
                        
                    
                    

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
//                await client.PostAsync("http://coolscreenwebservice.azurewebsites.net/Service1.svc/Temperatur", byteContent);
                await client.PostAsync("http://localhost:36917/Service1.svc/Temperatur", byteContent);
                
            }
        }
    
            
        

        // To parse data from the IoT devices in the teachers room, Elisagårdsvej
        private static void Parse(string response)
        {
            string[] parts = response.Split(' ');
            foreach (string part in parts)
            {
                Console.WriteLine(part);
            }
            string temperatureLine = parts[6];
            string temperatureStr = temperatureLine.Substring(temperatureLine.IndexOf(": ") + 2);
            Console.WriteLine(temperatureStr);
        }
    }
}
