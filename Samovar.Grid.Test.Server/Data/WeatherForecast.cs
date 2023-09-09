using System;
using System.ComponentModel.DataAnnotations;

namespace Samovar.Grid.Test.Server.Data
{
    public class WeatherForecastMinViewModel
    {
        public int Position { get; set; }
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }

        public double SummaryDbl { get; set; }
        public double? SummaryDblNullable { get; set; }
        public bool YesNo { get; set; }
        public sbyte SByteValue { get; set; }
        public byte ByteValue { get; set; }
        public char CharValue { get; set; }
        
        public override string ToString()
        {
            return $"{Position.ToString()} {TemperatureC.ToString()} {Summary}";
        }
    }


    public class WeatherForecast
    {
        public Employe WeatherReportCreater { get; set; } = new Employe { FirstName = "Dimi", LastName = "Michi" };
        public int? NullInt { get; set; } = null;
        public string NullString { get; set; } = null;
        public int Position { get; set; }
        public DateTime Date { get; set; }
        public DateTime? DateNullable { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        private string summary;
        public string Summary {
            get { return summary; }
            set {
                summary = value;
            } 
        }
        public string Summary2 { get; set; }

        public double SummaryDbl { get; set; }
        public double? SummaryDblNullable { get; set; }
        public bool YesNo { get; set; }
        public sbyte SByteValue { get; set; }
        public byte ByteValue { get; set; }
        public char CharValue { get; set; }

        
        public string Summary01 { get { return summary; } set { summary = value; } }
        public string Summary02 { get { return summary; } set { summary = value; } }
        public string Summary03 { get { return summary; } set { summary = value; } }
        public string Summary04 { get { return summary; } set { summary = value; } }
        public string Summary05 { get { return summary; } set { summary = value; } }
        public string Summary06 { get { return summary; } set { summary = value; } }
        public string Summary07 { get { return summary; } set { summary = value; } }
        public string Summary08 { get { return summary; } set { summary = value; } }
        public string Summary09 { get { return summary; } set { summary = value; } }
        public string Summary10 { get { return summary; } set { summary = value; } }
        public string Summary11 { get { return summary; } set { summary = value; } }
        public string Summary12 { get { return summary; } set { summary = value; } }
        public string Summary13 { get { return summary; } set { summary = value; } }
        public string Summary14 { get { return summary; } set { summary = value; } }
        public string Summary15 { get { return summary; } set { summary = value; } }
        public string Summary16 { get { return summary; } set { summary = value; } }
        public string Summary17 { get { return summary; } set { summary = value; } }
        public string Summary18 { get { return summary; } set { summary = value; } }
        public string Summary19 { get { return summary; } set { summary = value; } }
        public string Summary20 { get { return summary; } set { summary = value; } }
        public string Summary21 { get { return summary; } set { summary = value; } }
        public string Summary22 { get { return summary; } set { summary = value; } }
        public string Summary23 { get { return summary; } set { summary = value; } }
        public string Summary24 { get { return summary; } set { summary = value; } }
        public string Summary25 { get { return summary; } set { summary = value; } }
        public string Summary26 { get { return summary; } set { summary = value; } }
        public string Summary27 { get { return summary; } set { summary = value; } }
        public string Summary28 { get { return summary; } set { summary = value; } }
        public string Summary29 { get { return summary; } set { summary = value; } }
        public string Summary30 { get { return summary; } set { summary = value; } }
        public string Summary31 { get { return summary; } set { summary = value; } }
        public string Summary32 { get { return summary; } set { summary = value; } }
        public string Summary33 { get { return summary; } set { summary = value; } }
        public string Summary34 { get { return summary; } set { summary = value; } }
        public string Summary35 { get { return summary; } set { summary = value; } }
        public string Summary36 { get { return summary; } set { summary = value; } }
        public string Summary37 { get { return summary; } set { summary = value; } }
        public string Summary38 { get { return summary; } set { summary = value; } }
        public string Summary39 { get { return summary; } set { summary = value; } }
        public string Summary40 { get { return summary; } set { summary = value; } }
        public string Summary41 { get { return summary; } set { summary = value; } }
        public string Summary42 { get { return summary; } set { summary = value; } }
        public string Summary43 { get { return summary; } set { summary = value; } }
        public string Summary44 { get { return summary; } set { summary = value; } }
        public string Summary45 { get { return summary; } set { summary = value; } }
        public string Summary46 { get { return summary; } set { summary = value; } }
        public string Summary47 { get { return summary; } set { summary = value; } }
        public string Summary48 { get { return summary; } set { summary = value; } }
        public string Summary49 { get { return summary; } set { summary = value; } }
        public string Summary50 { get { return summary; } set { summary = value; } }
        public string Summary51 { get { return summary; } set { summary = value; } }
        public string Summary52 { get { return summary; } set { summary = value; } }
        public string Summary53 { get { return summary; } set { summary = value; } }
        public string Summary54 { get { return summary; } set { summary = value; } }
        public string Summary55 { get { return summary; } set { summary = value; } }
        public string Summary56 { get { return summary; } set { summary = value; } }
        public string Summary57 { get { return summary; } set { summary = value; } }
        public string Summary58 { get { return summary; } set { summary = value; } }
        public string Summary59 { get { return summary; } set { summary = value; } }
        public string Summary60 { get { return summary; } set { summary = value; } }
        public string Summary61 { get { return summary; } set { summary = value; } }
        public string Summary62 { get { return summary; } set { summary = value; } }
        public string Summary63 { get { return summary; } set { summary = value; } }
        public string Summary64 { get { return summary; } set { summary = value; } }
        public string Summary65 { get { return summary; } set { summary = value; } }
        public string Summary66 { get { return summary; } set { summary = value; } }
        public string Summary67 { get { return summary; } set { summary = value; } }
        public string Summary68 { get { return summary; } set { summary = value; } }
        public string Summary69 { get { return summary; } set { summary = value; } }
        public string Summary70 { get { return summary; } set { summary = value; } }
        public string Summary71 { get { return summary; } set { summary = value; } }
        public string Summary72 { get { return summary; } set { summary = value; } }
        public override string ToString()
        {
            return Position.ToString() + " " + Summary;
        }
    }

    public class Employe
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return $"{LastName}, {FirstName}"; 
        }
    }
}
