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
                var newHouses = new List<homeClass>();
                try
                {
                    newHouses = yad2.main();
                }
                catch(Exception ex)
                {

                }
                //Uniqify the newHouses array
                newHouses = newHouses.Distinct(new EqualityParity()).ToList();
               
                foreach (var house in newHouses)
                {
                    mailwrapper mailer = new mailwrapper();
                    mailer.sendMail(house);

                }
                newHouses = null;
                Thread.Sleep(120000);
            }

        }
    }
    class ParameterClass
    {
        public string parameterString;
        public string parameterOwnerEmail;
        public int maxMeter = 9999;
        public int minMeter = 0;
        public void FilterHousesByCustomParams(ref List<homeClass> houses)
        {
            houses = houses.Where(house => house.Meter <= this.maxMeter
            && house.Meter >= this.minMeter).ToList();
        }
    }
    class yad2Layer
    {
        private const string yad2Api = "http://www.yad2.co.il/ajax/Nadlan/searchMap/results.php";
        private bool firstRun = true;
        
        private List<ParameterClass> parametersCollection=null;
        private List<List<homeClass>> cache = null;
        public yad2Layer()
        {
            buildParams();
        }
        public string getHouseUrl(homeClass home)
        {
            if (!home.isTivuh)
            {
                return "http://www.yad2.co.il/Nadlan/rent_info.php?NadlanID=" + home.PostNo;
            }
            else
            {
                return "http://www.yad2.co.il/Nadlan/tivrent_info.php?NadlanID=" + home.PostNo;
            }
        }
        private void buildParams()
        {
            parametersCollection = new List<ParameterClass>();
            //parametersCollection.Add("?SubCatID=2&AreaID=&City=&HomeTypeID=&fromRooms=2.5&untilRooms=3.5&fromPrice=&untilPrice=7500&PriceType=1&FromFloor=1&ToFloor=&fromSquareMeter=65&untilSquareMeter=&EnterDate=&Info=&coords%5Btop%5D%5Blat%5D=32.0812407485499&coords%5Btop%5D%5Blng%5D=34.76860038936138&coords%5Bbottom%5D%5Blat%5D=32.063020925819046&coords%5Bbottom%5D%5Blng%5D=34.76860038936138&coords%5Bright%5D%5Blat%5D=32.07213038336747&coords%5Bright%5D%5Blng%5D=34.77935106571829&coords%5Bleft%5D%5Blat%5D=32.07213038336747&coords%5Bleft%5D%5Blng%5D=34.757849713004475&radius=1012.9759260172019&centerCoords%5Blat%5D=32.072130837184474&centerCoords%5Blng%5D=34.76860038936138&searchMode=radius&_="+ (int)getUnixTimeNow());
            //var parameter2 = new ParameterClass() { parameterString = "?SubCatID=2&AreaID=1&City=&HomeTypeID=&fromRooms=2.5&untilRooms=&fromPrice=&untilPrice=8500&PriceType=1&FromFloor=&ToFloor=&EnterDate=&Info=&coords%5Btop%5D%5Blat%5D=32.09144237371608&coords%5Btop%5D%5Blng%5D=34.76313625025796&coords%5Bbottom%5D%5Blat%5D=32.06837841287437&coords%5Bbottom%5D%5Blng%5D=34.76313625025796&coords%5Bright%5D%5Blat%5D=32.07990966586507&coords%5Bright%5D%5Blng%5D=34.7767463869809&coords%5Bleft%5D%5Blat%5D=32.07990966586507&coords%5Bleft%5D%5Blng%5D=34.74952611353501&radius=1282.2977169630556&centerCoords%5Blat%5D=32.07991039329522&centerCoords%5Blng%5D=34.76313625025796&searchMode=radius&_=" + (int)getUnixTimeNow(), parameterOwnerEmail = "sabih.erdemanar@gmail.com" };
            parametersCollection.Add(new ParameterClass() { minMeter = 73, parameterString = "?SubCatID=2&AreaID=1&City=&HomeTypeID=&fromRooms=2.5&untilRooms=5&fromPrice=&untilPrice=8200&PriceType=1&FromFloor=&ToFloor=&EnterDate=&Info=&coords%5Btop%5D%5Blat%5D=32.10217712228644&coords%5Btop%5D%5Blng%5D=34.77134697139263&coords%5Bbottom%5D%5Blat%5D=32.07873741994686&coords%5Bbottom%5D%5Blng%5D=34.77134697139263&coords%5Bright%5D%5Blat%5D=32.09045651948452&coords%5Bright%5D%5Blng%5D=34.78518043105737&coords%5Bleft%5D%5Blat%5D=32.09045651948452&coords%5Bleft%5D%5Blng%5D=34.75751351172789&radius=1303.1879911096337&centerCoords%5Blat%5D=32.090457271116655&centerCoords%5Blng%5D=34.77134697139263&searchMode=radius&_=" + (int)getUnixTimeNow(), parameterOwnerEmail = "ketty.slonimsky@gmail.com,sabih.erdemanar@gmail.com" });
            parametersCollection.Add(new ParameterClass() { minMeter = 73, parameterString = "?SubCatID=2&AreaID=1&City=&HomeTypeID=&fromRooms=2.5&untilRooms=&fromPrice=&untilPrice=8200&PriceType=1&FromFloor=&ToFloor=&EnterDate=&Info=&coords%5Btop%5D%5Blat%5D=32.09836336922085&coords%5Btop%5D%5Blng%5D=34.78850276760102&coords%5Bbottom%5D%5Blat%5D=32.08717432640638&coords%5Bbottom%5D%5Blng%5D=34.78850276760102&coords%5Bright%5D%5Blat%5D=32.092768676526&coords%5Bright%5D%5Blng%5D=34.7951063962189&coords%5Bleft%5D%5Blat%5D=32.092768676526&coords%5Bleft%5D%5Blng%5D=34.781899138983135&radius=622.0823974889346&centerCoords%5Blat%5D=32.09276884781361&centerCoords%5Blng%5D=34.78850276760102&searchMode=radius&_=" + (int)getUnixTimeNow(), parameterOwnerEmail = "ketty.slonimsky@gmail.com,sabih.erdemanar@gmail.com" });
            parametersCollection.Add(new ParameterClass() { minMeter = 73, parameterString = "?SubCatID=2&AreaID=1&City=&HomeTypeID=&fromRooms=2.5&untilRooms=&fromPrice=&untilPrice=8200&PriceType=1&FromFloor=&ToFloor=&EnterDate=&Info=&coords%5Btop%5D%5Blat%5D=32.09938851458584&coords%5Btop%5D%5Blng%5D=34.77976681283917&coords%5Bbottom%5D%5Blat%5D=32.07739732808628&coords%5Bbottom%5D%5Blng%5D=34.77976681283917&coords%5Bright%5D%5Blat%5D=32.08839225978448&coords%5Bright%5D%5Blng%5D=34.79274510558571&coords%5Bleft%5D%5Blat%5D=32.08839225978448&coords%5Bleft%5D%5Blng%5D=34.76678852009263&radius=1222.6541848221752&centerCoords%5Blat%5D=32.08839292133607&centerCoords%5Blng%5D=34.77976681283917&searchMode=radius&_=" + (int)getUnixTimeNow(), parameterOwnerEmail = "ketty.slonimsky@gmail.com,sabih.erdemanar@gmail.com" });
            parametersCollection.Add(new ParameterClass() { minMeter = 73, parameterString = "?SubCatID=2&AreaID=1&City=&HomeTypeID=&fromRooms=2.5&untilRooms=&fromPrice=&untilPrice=8200&PriceType=1&FromFloor=&ToFloor=&EnterDate=&Info=&coords%5Btop%5D%5Blat%5D=32.100488733295784&coords%5Btop%5D%5Blng%5D=34.77999077729169&coords%5Bbottom%5D%5Blat%5D=32.077244487396186&coords%5Bbottom%5D%5Blng%5D=34.77999077729169&coords%5Bright%5D%5Blat%5D=32.08886587124244&coords%5Bright%5D%5Blng%5D=34.793708645202855&coords%5Bleft%5D%5Blat%5D=32.08886587124244&coords%5Bleft%5D%5Blng%5D=34.76627290938052&radius=1292.3211088564356&centerCoords%5Blat%5D=32.08886661034598&centerCoords%5Blng%5D=34.77999077729169&searchMode=radius&_=" + (int)getUnixTimeNow(), parameterOwnerEmail = "ketty.slonimsky@gmail.com,sabih.erdemanar@gmail.com" });
            parametersCollection.Add(new ParameterClass() { minMeter = 73, parameterString = "?SubCatID=2&AreaID=&City=&HomeTypeID=&fromRooms=2.5&untilRooms=3.5&fromPrice=6000&untilPrice=8200&PriceType=1&FromFloor=&ToFloor=&EnterDate=&Info=&coords%5Btop%5D%5Blat%5D=32.09870724446947&coords%5Btop%5D%5Blng%5D=34.78892829263236&coords%5Bbottom%5D%5Blat%5D=32.08800745216178&coords%5Bbottom%5D%5Blng%5D=34.78892829263236&coords%5Bright%5D%5Blat%5D=32.09335719167635&coords%5Bright%5D%5Blng%5D=34.795243212595096&coords%5Bleft%5D%5Blat%5D=32.09335719167635&coords%5Bleft%5D%5Blng%5D=34.78261337266963&radius=594.8813103834988&centerCoords%5Blat%5D=32.09335734831563&centerCoords%5Blng%5D=34.78892829263236&searchMode=radius&_=" + (int)getUnixTimeNow(), parameterOwnerEmail = "ketty.slonimsky@gmail.com,sabih.erdemanar@gmail.com" });

            //parametersCollection.Add(parameter2);
            cache = new List<List<homeClass>>(parametersCollection.Count);
            parametersCollection.ForEach(param => cache.Add(new List<homeClass>()));
        }
        private double getUnixTimeNow() {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (DateTime.UtcNow - epoch).TotalSeconds*1000;
        }
        private homeClass fillHouseValuesFromHousePage(homeClass home)
        {
            var web = new WebClient();
            web.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8,tr;q=0.6,he;q=0.4");
            web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
            web.Headers.Add(HttpRequestHeader.Accept, "*");
            string  homePageString = web.DownloadString(this.getHouseUrl(home));
            var homePage = new HtmlAgilityPack.HtmlDocument();
            homePage.LoadHtml(homePageString);
            var homeDataTable = homePage.DocumentNode.DescendantNodes()
                .Where(table => 
                table.HasClass("innerDetailsDataGrid")).FirstOrDefault();
            // Fill the details for home from table

            string homeMeter = homeDataTable.Descendants("tr").Last().Descendants("td").Last().Descendants("b").First().InnerHtml;
            home.Meter = int.Parse(homeMeter.Trim());
            // Add more advanced filters here
            return home;

        }
        
        public List<homeClass> main()
        {
            var result = new List<homeClass>();
            foreach (var param in parametersCollection)
            {

                var web = (HttpWebRequest)WebRequest.Create(yad2Api + param.parameterString);
                //web.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                web.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8,tr;q=0.6,he;q=0.4");
                web.Accept = "application/json; q=0.01";
                web.UserAgent= "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                web.Timeout = 100000;
                //web.Referer= @"http://www.yad2.co.il/Nadlan/rentMap.php?AreaID=&City=&HomeTypeID=&fromRooms=2.5&untilRooms=3.5&fromPrice=&untilPrice=7500&PriceType=1&FromFloor=1&ToFloor=&fromSquareMeter=65&untilSquareMeter=&EnterDate=&Info=";

                HttpWebResponse response = (HttpWebResponse)web.GetResponse();
                Console.WriteLine("request sent. Area : "+parametersCollection.IndexOf(param)+" Time: "+DateTime.Now.ToString());
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                string responseFromServer = reader.ReadToEnd();
                var houses = parseResponse(responseFromServer);
                Console.WriteLine(houses.Count + " houses found");
                var newhouses = returnNewHouses(houses, parametersCollection.IndexOf(param));
                for(int i = 0; i < newhouses.Count; i++)
                {
                    newhouses[i] = fillHouseValuesFromHousePage(newhouses[i]);
                }
                param.FilterHousesByCustomParams(ref newhouses);
                if (newhouses.Count > 0)
                {
                    Console.WriteLine(newhouses.Count + " new houses found");
                }
                if (!firstRun)
                {
                    result.AddRange(newhouses);
                }
                reader.Close();
                dataStream.Close();
                response.Close();
                web = null;
            }
            firstRun = false;
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
            result = null;
            return resultList;
        }
        private List<homeClass> returnNewHouses(List<homeClass> houses,int paramIndex)
        {
            List<homeClass> result = new List<homeClass>();
            if(cache[paramIndex].Count == 0)
            {

                cache[paramIndex] = houses;
            }
            else
            {
                result = houses.Where(x => !cache[paramIndex].Any(y => x.PostNo == y.PostNo)).ToList<homeClass>();
                if (result.Count > 0)
                {
                    cache[paramIndex].AddRange(result);
                }
                var updatedHouses = houses.Where(x => replaceRecordInCacheIfNeed(x,paramIndex)).ToList();
                if (updatedHouses.Count > 0) {
                    result.AddRange(updatedHouses);
                }
                var dissappearedHouses = cache[paramIndex].Where(x => !houses.Any(y => y.PostNo == x.PostNo));
                dissappearedHouses.ToList().ForEach(dissappearedHouse => cache[paramIndex].Remove(dissappearedHouse));
            }
            return result;
        }
        private bool replaceRecordInCacheIfNeed(homeClass newHouse,int paramIndex)
        {
            var oldHouses = cache[paramIndex].Where(y => (y.PostNo == newHouse.PostNo) && (y.Street != newHouse.Street)).ToList();
            
            int oldHouseCount = oldHouses.Count;
            if (oldHouseCount > 0)
            {
                oldHouses.ForEach(x => cache[paramIndex].Remove(x));
                cache[paramIndex].Add(newHouse);
            }
            return oldHouseCount > 0;
        }
    }
    public class homeClass
    {
        public string City;
        public string HomeNo;
        public string PostNo;
        public string Price;
        public string Street;
        public string Neighborhood;
        public int Meter;
        public DateTime PostDate;
        public bool isTivuh;
    }
    public class mailwrapper
    {
        private SmtpClient _mailer;

        public string subject = "";
        public string to = "ketty.slonimsky@gmail.com,sabih.erdemanar@gmail.com";
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
                return @"new home in " + home.Neighborhood + ".Address:" + home.Street + ".Price:" + home.Price + ". Link:" + new yad2Layer().getHouseUrl(home);
        }
    }
    class EqualityParity : IEqualityComparer<homeClass>
    {
        public bool Equals(homeClass x, homeClass y)
        {
            return this.GetHashCode(x) == this.GetHashCode(y);
        }
        public int GetHashCode(homeClass x)
        {
            return int.Parse(x.PostNo);
        }

    }
}

