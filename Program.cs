using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using System.Net.Mail;

namespace homefinderYad2
{
    class Program
    {
        static void Main(string[] args)
        {
            yad2Layer yad2 = new yad2Layer();
            while (true)
            {
                var newHouses = yad2.main();
                foreach (var house in newHouses)
                {
                    mailwrapper mailer = new mailwrapper();
                    mailer.sendMail(house);

                }
                Thread.Sleep(120000);
            }

        }
    }
    class yad2Layer
    {
        private const string yad2Api = "http://www.yad2.co.il/ajax/Nadlan/searchMap/results.php";
        private HttpWebRequest web;
        private List<string> parametersCollection=null;
        private List<homeClass> cache=null;
        public yad2Layer()
        {
            buildParams();
        }
        private void buildParams()
        {
            parametersCollection = new List<string>();
            parametersCollection.Add("?SubCatID=2&AreaID=&City=&HomeTypeID=&fromRooms=2.5&untilRooms=3.5&fromPrice=&untilPrice=7500&PriceType=1&FromFloor=1&ToFloor=&fromSquareMeter=65&untilSquareMeter=&EnterDate=&Info=&coords%5Btop%5D%5Blat%5D=32.0812407485499&coords%5Btop%5D%5Blng%5D=34.76860038936138&coords%5Bbottom%5D%5Blat%5D=32.063020925819046&coords%5Bbottom%5D%5Blng%5D=34.76860038936138&coords%5Bright%5D%5Blat%5D=32.07213038336747&coords%5Bright%5D%5Blng%5D=34.77935106571829&coords%5Bleft%5D%5Blat%5D=32.07213038336747&coords%5Bleft%5D%5Blng%5D=34.757849713004475&radius=1012.9759260172019&centerCoords%5Blat%5D=32.072130837184474&centerCoords%5Blng%5D=34.76860038936138&searchMode=radius&_="+ (int)getUnixTimeNow());
        }
        private double getUnixTimeNow() {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (DateTime.UtcNow - epoch).TotalSeconds*1000;
        }
        public List<homeClass> main()
        {
            var result = new List<homeClass>();
            foreach (var param in parametersCollection)
            {
                web = WebRequest.CreateHttp(yad2Api +param);
                //web.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                web.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8,tr;q=0.6,he;q=0.4");
                web.Accept = "application/json; q=0.01";
                web.UserAgent= "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                web.Referer= @"http://www.yad2.co.il/Nadlan/rentMap.php?AreaID=&City=&HomeTypeID=&fromRooms=2.5&untilRooms=3.5&fromPrice=&untilPrice=7500&PriceType=1&FromFloor=1&ToFloor=&fromSquareMeter=65&untilSquareMeter=&EnterDate=&Info=";

                HttpWebResponse response = (HttpWebResponse)web.GetResponse();
                Console.WriteLine("request sent. Time: "+DateTime.Now.ToString());
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                string responseFromServer = reader.ReadToEnd();
                var houses = parseResponse(responseFromServer);
                Console.WriteLine(houses.Count + " houses found");
                var newhouses = returnNewHouses(houses);
                if (newhouses.Count > 0)
                {
                    Console.WriteLine(newhouses.Count + " new houses found");
                }
                result.AddRange(newhouses);
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            return result;
            
        }
        private List<homeClass> parseResponse(string response)
        {
            List<homeClass> resultList = new List<homeClass>();
            dynamic result = JsonConvert.DeserializeObject(response);
            foreach (var group in result.groups)
            {
                foreach (var home in group.mador[0].results)
                {
                    homeClass newhome = new homeClass();
                    newhome.City = home.City;
                    newhome.HomeNo = home.HomeNum;
                    newhome.PostNo = home.NadlanID;
                    newhome.Neighborhood = home.Neighborhood;
                    newhome.PostDate = DateTime.Parse(home.StartDate.Value);
                    newhome.isTivuh = false;
                    newhome.Price = home.Price;
                    newhome.Street = home.Street;
                    resultList.Add(newhome);
                }
                foreach (var home in group.mador[1].results)
                {
                    homeClass newhome = new homeClass();
                    newhome.City = home.City;
                    newhome.HomeNo = home.HomeNum;
                    newhome.PostNo = home.NadlanID;
                    newhome.Neighborhood = home.Neighborhood;
                    newhome.PostDate = DateTime.Parse(home.StartDate.Value);
                    newhome.isTivuh = true;
                    newhome.Price = home.Price;
                    newhome.Street = home.Street;
                    resultList.Add(newhome);
                }
            }
            return resultList;
        }
        private List<homeClass> returnNewHouses(List<homeClass> houses)
        {
            List<homeClass> result = new List<homeClass>();
            if(cache == null)
            {
                cache = houses;
            }
            else
            {
                result = houses.Where(x => !cache.Any(y => x.PostNo == y.PostNo)).ToList<homeClass>();
                cache.AddRange(result);
            }
            return result;
        }
    }
    public struct homeClass
    {
        public string City;
        public string HomeNo;
        public string PostNo;
        public string Price;
        public string Street;
        public string Neighborhood;
        public DateTime PostDate;
        public bool isTivuh;
    }
    public class mailwrapper
    {
        private SmtpClient _mailer;

        public string subject = "";
        public string to = "sabih.erdemanar@gmail.com,ketty.slonimsky@gmail.com";
        public string host = "smtp.gmail.com";
        public string username = "berta.system";
        public string password = "BertaSystem123";
        public string from = "berta.system@gmail.com";
        public mailwrapper()
        {
            bool isGmail = host.Contains("gmail");
            _mailer = new SmtpClient(host, isGmail ? 587 : 443);
            _mailer.Credentials = new NetworkCredential(username, password);
            if (isGmail)
            {
                _mailer.EnableSsl = true;
            }
        }
        public mailwrapper(string smtpaddress)
        {
            _mailer = new SmtpClient(smtpaddress);
        }
        public mailwrapper(string smtpaddress, int port) : this(smtpaddress)
        {
            _mailer.Port = port;
        }
        public mailwrapper(string smtpaddress, int port, string username, string password) : this(smtpaddress, port)
        {
            _mailer.Credentials = new NetworkCredential(username, password);
        }
        public void sendMail(homeClass home)
        {
            string body = createBody(home);
            string subject = "new home in " + home.Neighborhood;
            MailMessage msg = new MailMessage(from, to, subject, body);
            msg.IsBodyHtml = true;
            
            _mailer.Send(msg);
        }
        private string createBody(homeClass home)
        {
            return @"new home in "+home.Neighborhood+".Address:"+home.Street+".Price:"+home.Price+". Link:"+ "http://www.yad2.co.il/Nadlan/rent_info.php?NadlanID="+home.PostNo;
        }
    }
}
