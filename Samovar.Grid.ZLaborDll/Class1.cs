using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Samovar.Grid.ZLaborDll
{
    public class ClassTest
    {
        public ClassTest()
        {

        }
        public void Test() {
            List<Mitarbeiter> mitarbeiterList = new List<Mitarbeiter>();
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 1", Vorname = "Vorname 1", Abteilung = 1, Team = "Team 11" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 2", Vorname = "Vorname 2", Abteilung = 1, Team = "Team 11" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 3", Vorname = "Vorname 3", Abteilung = 1, Team = "Team 12" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 4", Vorname = "Vorname 4", Abteilung = 2, Team = "Team 21" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 5", Vorname = "Vorname 5", Abteilung = 2, Team = "Team 21" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 6", Vorname = "Vorname 6", Abteilung = 3, Team = "Team 31" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 7", Vorname = "Vorname 7", Abteilung = 4, Team = "Team 41" });
            mitarbeiterList.Add(new Mitarbeiter { Name = "Name 8", Vorname = "Vorname 8", Abteilung = 4, Team = "Team 41" });

            var result = mitarbeiterList.AsQueryable().GroupBy("new (Abteilung, Team)").ToDynamicArray();

            for (int i = 0; i < result.Length; i++)
            {
                var expectedRow = result[i];

                // The DynamicBinder doesn't allow us to access values of the Group object, so we have to cast first
                var testRow = (IGrouping<DynamicClass, Mitarbeiter>)result[i];

                Console.WriteLine("Key.Abteilung   = {0}", ((dynamic)testRow.Key).Abteilung);
                Console.WriteLine("Key.Team = {0}", ((dynamic)testRow.Key).TEam);
                Console.WriteLine("Mitarbeiter   = {0}", string.Join(", ", testRow.Select(x => $"'{x.ToString()}'")));
                Console.WriteLine();
            }
        }
    }
}
