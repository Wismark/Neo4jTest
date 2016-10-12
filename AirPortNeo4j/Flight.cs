using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirPortNeo4j
{
    class Flight
    {
        public int flight_num { get; set; }
        public string flight_type { get; set; }
        public string aircraft_type { get; set; }
        public string destination { get; set; }
    }
}
