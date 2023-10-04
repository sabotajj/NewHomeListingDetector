using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homefinderYad2
{
    public class Home
    {
        public string City;
        public string HomeNo;
        public string PostNo;
        public string Price;
        public string Street;
        public string Neighborhood;
        public int Meter;
        public DateTime PostDate;
        public bool IsAgency;
    }
    class EqualityParity : IEqualityComparer<Home>
    {
        public bool Equals(Home x, Home y)
        {
            return this.GetHashCode(x) == this.GetHashCode(y);
        }
        public int GetHashCode(Home x)
        {
            return int.Parse(x.PostNo);
        }

    }
}
