using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace X4_WarePriceCalculator
{

    public static class XmlWareExtractor
    {
        public static List<Ware> ReadAllMaterials(string filepath)
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                List<Ware> wares = new List<Ware>();
                var doc = XDocument.Load(filepath);

                var wareXmlList = doc.Descendants("ware").Where(x => x.Attribute("tags") != null && x.Attribute("tags").Value.Contains("economy")).ToList();

                foreach (var item in wareXmlList)
                {
                    Ware ware = new Ware();
                    ware.Price = Convert.ToInt16(item.Element("price").Attribute("average").Value);
                    ware.Name = item.Attribute("id").Value;
                    wares.Add(ware);
                }
                return wares;
            }
        }
        public static List<Ware> ReadAllWares(string filepath)
        {
            using (StreamReader sr = new StreamReader(filepath))
            {
                List<Ware> wares = new List<Ware>();
                var doc = XDocument.Load(filepath);

                var wareXmlList = doc.Descendants("ware").Where(x => x.Attribute("tags") != null).ToList();

                foreach (var item in wareXmlList)
                {
                    Ware ware = new Ware();
                    ware.Price = Convert.ToInt32(item.Element("price").Attribute("average").Value);
                    ware.Name = item.Attribute("id").Value;
                    wares.Add(ware);
                }
                return wares;
            }
        }
    }
}