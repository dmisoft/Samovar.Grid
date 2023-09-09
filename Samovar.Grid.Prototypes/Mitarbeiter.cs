using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.Grid.Prototypes
{
    public class Mitarbeiter
    {
        public string Name { get; set; }
        public string Vorname { get; set; }
        public int Abteilung { get; set; }
        public string Team { get; set; }

        public override string ToString()
        {
            return $"Abt.: {Abteilung.ToString()} Team: {Team} MA: {Vorname} {Name}";
        }
    }

    public class Mitarbeiter2<T> where T:class
    {
        public string Name { get; set; }
        public string Vorname { get; set; }
        public int Abteilung { get; set; }
        public string Team { get; set; }
    }
}
