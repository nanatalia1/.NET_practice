using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Lab3
{

    [XmlRoot(ElementName = "engine")]
    public class Engine
    {

        public double displacement;
        public double horsePower;
        [XmlAttribute]
        public string model;

        public Engine() { }
        public Engine(double displacement, double horsePower, string model)
        {
            this.displacement = displacement;
            this.horsePower = horsePower;
            this.model = model;
        }

    }

    [XmlType("car")]
    public class Car
    {
        public string model;
        public int year;
        [XmlElement(ElementName = "engine")]
        public Engine motor;

        public Car() { }
        public Car(string model, Engine motor, int year)
        {
            this.model = model;
            
            this.motor = motor;
            this.year = year;


        }

    }
    public class Program
    {
        public static void serialization(List<Car> myCars, string path)
        {
            using (TextWriter file = new StreamWriter(path))
            {
                XmlSerializer x = new(myCars.GetType(), new XmlRootAttribute("cars"));
                x.Serialize(file, myCars);
                file.Close();
            }
        }

        public static List<Car>? Deserialization(string path)
        {
            using (Stream file = new FileStream(path, FileMode.Open))
            {
                XmlSerializer x = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));
                var output = x.Deserialize(file);
                return output as List<Car>;
            }
        }



        private static void XPathStatements(string path)
        {
            XElement rootNode = XElement.Load(path);
            double avgHP = (double)rootNode.XPathEvaluate("sum(/car/engine[@model!='TDI']/horsePower) div count(/car/engine[@model!='TDI'])");
            // var sum = avgPH.Evaluate
            // avgHP= sum(//car/engine[@model!=\"TDI\"]/horsePower) div count(//car/engine[@model!=\"TDI\"]/horsePower)
            
            Console.WriteLine("Średnia: {0}", avgHP);

            var removeDuplicatesXPath = "//car[following-sibling::car/model = model]";
            IEnumerable<XElement> models = rootNode.XPathSelectElements(removeDuplicatesXPath);

            var fileName = "CarsCollectionNoRepeats.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentDirectory, fileName);
            using (var writer = new StreamWriter(filePath))
            {
                foreach (var model in models)
                {
                    writer.WriteLine(model);
                }
            }
        }

        static private void xmlLinq(List<Car> myCars)
        {
            IEnumerable<XElement> nodes = myCars
                .Select(n =>
                new XElement("car",
                    new XElement("model", n.model),
                    new XElement("engine",
                        new XAttribute("model", n.motor.model),
                        new XElement("displacement", n.motor.displacement),
                        new XElement("horsePower", n.motor.horsePower)),
                    new XElement("year", n.year)));

            XElement rootNode = new XElement("cars", nodes); //create a root node to contain the query results
            rootNode.Save("CarsFromLinq.xml");

           
        }

        private static void XHTMLTable(List<Car> myCars)
        {
            IEnumerable<XElement> rows = myCars
                .Select(car =>
                new XElement("tr", new XAttribute("style", "border: 2px solid black"),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.model),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.motor.model),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.motor.displacement),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.motor.horsePower),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.year)));
            XElement table = new XElement("table", new XAttribute("style", "border: 2px double black"), rows);
            



            var filename = "template.html";
            var currentDirectory = Directory.GetCurrentDirectory();
            string htmlFileLocation = Path.Combine(currentDirectory, filename);
            table.Save(htmlFileLocation);

        }



        private static void modifyCarsCollection(string path)
        {
            XElement template = XElement.Load(path);
            foreach (var car in template.Elements())
            {
                foreach (var field in car.Elements())
                {
                    if (field.Name == "engine")
                    {
                        foreach (var engineElement in field.Elements())
                        {
                            if (engineElement.Name == "horsePower")
                            {
                                engineElement.Name = "hp";
                            }
                        }
                    }
                    else if (field.Name == "model")
                    {
                        var yearField = car.Element("year");
                        XAttribute attribute = new XAttribute("year", yearField.Value);
                        field.Add(attribute);
                        yearField.Remove();
                    }
                }
            }


            var filename = "CarsCollectionModified.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            string modFileLocation = Path.Combine(currentDirectory, filename);
            template.Save(modFileLocation);
           
        }
        static void Main()
        {


            List<Car> myCars = new List<Car>(){
                new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
                new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
                new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
                new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
                new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
                new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
                new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
                new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
                new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
            };
            var query1 = from c in myCars
                         where c.model == "A6"
                         select new
                         {
                             engineType = String.Compare(c.motor.model, "TDI") == 0
                                      ? "diesel"
                                      : "petrol",
                             hppl = c.motor.horsePower / c.motor.displacement
                         };
            foreach (var c in query1)
            {
                // Console.WriteLine("engine: {0} hppl: {1}", c.engineType, c.hppl);
            }
            Console.WriteLine();
            IEnumerable<IGrouping<string, double>> query2 =
                from c in query1 group c.hppl by c.engineType;

            foreach (IGrouping<string, double> group in query2)
            {
                double sum = 0;
                int num = 0;


                foreach (double value in group)
                {
                    num++;
                    sum += value;
                }
                double mean = sum / num;
                Console.WriteLine("Group key: {0}, mean:{1}", group.Key, mean);
            }

            //string path= "C:/Users/48516//Desktop/platformy technologiczne/CarsCollection.xml";

            var filename = "CarsCollection.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            string xmlFileLocation = Path.Combine(currentDirectory, filename);
            serialization(myCars, xmlFileLocation);
            var deserializedList = Deserialization(xmlFileLocation);
            //Console.Write("Models of cars from deserialized list: {0}", deserializedList);
           // foreach (var c in deserializedList){
             //   Console.Write(c.motor);
            //}
            XPathStatements(xmlFileLocation);
            xmlLinq(myCars);
            XHTMLTable(myCars);
            modifyCarsCollection(xmlFileLocation);


        }


    }
}