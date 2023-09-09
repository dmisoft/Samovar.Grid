using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samovar.Grid.ZLabor
{
    public class Mitarbeiter
    {
        public Mitarbeiter()
        {

        }
        public string Name { get; set; }
        public string Vorname { get; set; }
        public int Abteilung { get; set; }
        public string Team { get; set; }
    }

    public class Mitarbeiter2<T> where T:class
    {
        public string Name { get; set; }
        public string Vorname { get; set; }
        public int Abteilung { get; set; }
        public string Team { get; set; }
    }
}
