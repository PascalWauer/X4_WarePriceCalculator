using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace X4_WarePriceCalculator
{
    class Program
    {
        public static List<Ware> Materials { get; set; }
        public static List<Ware> Wares { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting extraction...");

            string terranWares = @"Z:\Spiele\Steam\steamapps\common\X4 Foundations\unpacked_terrans\libraries\wares.xml";
            string modWares = @"Z:\Spiele\Steam\steamapps\common\X4 Foundations\extensions\x4_economyoverhaul\libraries\wares.xml";
            string VanillaWares = @"Z:\Spiele\Steam\steamapps\common\X4 Foundations\unpacked\libraries\wares.xml";
            string outputWares = "wares.xml";

            //Wares = new List<Ware>();

            Materials = XmlWareExtractor.ReadAllMaterials(modWares);
            Wares = XmlWareExtractor.ReadAllWares(VanillaWares);
            CalculateWares(terranWares, outputWares);

        }

        private static void CalculateWares(string pathInput, string pathOutput)
        {
            using (StreamWriter sw = new StreamWriter(pathOutput))
            {
                using (StreamReader sr = new StreamReader(pathInput))
                {
                    sr.ReadLine();
                    string line = "";
                    string warename = "";
                    string type = "";
                    int price = 0;
                    bool isVanillaItem = false;
                    double factor = 0.45;

                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf - 8\"?>");
                    sw.WriteLine("<diff>");

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("</ware>"))
                        {
                            type = "";
                            warename = "";
                        }
                        if (line.Contains("<ware id="))
                        {
                            type = "";
                            warename = line.Split('"')[1];
                            isVanillaItem = false;
                        }
                        if (line.Contains("<add sel=\"/wares/ware[@id="))
                        {
                            type = "";
                            warename = line.Split('\'')[1];
                            isVanillaItem = true;
                        }

                        if (line.Contains("tags=\"") && line.Contains("engine") && line.Contains("equipment"))
                        {
                            type = "engine";
                        }
                        if ((line.Contains("tags=\"") && line.Contains("module"))  || (line.Contains("'module_")))
                        {
                            type = "module";
                            if (line.Contains("_hab_") || line.Contains("_build_"))
                                factor = 0.03;
                            if (line.Contains("_equip_"))
                                factor = 0.05;
                        }
                        if (line.Contains("tags=\"") && line.Contains("shield") && line.Contains("equipment"))
                        {
                            type = "shield";
                        }
                        if (line.Contains("tags=\"") && line.Contains("transport=\"ship\""))
                        {
                            type = "ship";
                        }
                        if (line.Contains("tags=\"") && line.Contains("drone") && line.Contains("equipment"))
                        {
                            type = "drone";
                        }
                        if (line.Contains("tags=\"") && line.Contains("turret") && line.Contains("equipment"))
                        {
                            type = "turret";
                        }
                        if ((line.Contains("tags=\"") && line.Contains("weapon") && line.Contains("equipment")) || (line.Contains("'weapon_")))
                        {
                            type = "weapon";
                        }                        
                        if (line.Contains("'missile_"))
                        {
                            type = "missile";
                        }
                        if (line.Contains("'thruster_"))
                        {
                            type = "thruster";
                        }


                        if (line.Contains("price") && line.Contains("average"))
                        {
                            price = Convert.ToInt32(line.Split('"')[3]);
                        }

                        

                        if (line.Contains("<primary>") && type != "")
                        {
                            if (isVanillaItem)
                            {
                                price = Wares.FirstOrDefault(x => x.Name == warename).Price;
                                sw.WriteLine("<replace sel=\"/wares/ware[@id='" + warename + "']/production[@method='terran']/primary\">");
                            }
                            else
                            {
                                sw.WriteLine("<replace sel=\"/wares/ware[@id='" + warename + "']/production[@method='default']/primary\">");
                            }
                            sw.WriteLine("<primary>");
                            if (type == "shield")
                            {
                                sw.WriteLine(CalculateComponent(price, "energycells", 10));
                                sw.WriteLine(CalculateComponent(price, "forcefieldunits", 70));
                                sw.WriteLine(CalculateComponent(price, "computronicsubstrate", 20));
                            }
                            if (type == "module")
                            {
                                if (factor == 0.45)
                                { 
                                    //if (price < 1000000)
                                    //    factor = 3;
                                    //if (price < 400000)
                                    //    factor = 4;
                                }
                                sw.WriteLine(CalculateComponent(price, "energycells", 10, factor));
                                sw.WriteLine(CalculateComponent(price, "siliconcarbide", 20, factor));
                                sw.WriteLine(CalculateComponent(price, "computronicsubstrate", 70, factor));
                            }
                            if (type == "ship")
                            {
                                sw.WriteLine(CalculateComponent(price, "energycells", 20));
                                sw.WriteLine(CalculateComponent(price, "metallicmicrolattice", 80));
                            }
                            if (type == "engine")
                            {
                                sw.WriteLine(CalculateComponent(price, "energycells", 20));
                                sw.WriteLine(CalculateComponent(price, "metallicmicrolattice", 30));
                                sw.WriteLine(CalculateComponent(price, "fusionbatteries", 50));
                            }
                            if (type == "thruster")
                            {
                                sw.WriteLine(CalculateComponent(price, "energycells", 20));
                                sw.WriteLine(CalculateComponent(price, "metallicmicrolattice", 40));
                                sw.WriteLine(CalculateComponent(price, "fusionbatteries", 40));
                            }
                            if (type == "weapon")
                            {
                                sw.WriteLine(CalculateComponent(price, "energycells", 20));
                                sw.WriteLine(CalculateComponent(price, "siliconcarbide", 50));
                                sw.WriteLine(CalculateComponent(price, "fusionbatteries", 30));
                            }
                            if (type == "turret")
                            {
                                sw.WriteLine(CalculateComponent(price, "energycells", 20));
                                sw.WriteLine(CalculateComponent(price, "siliconcarbide", 40));
                                sw.WriteLine(CalculateComponent(price, "fusionbatteries", 30));
                                sw.WriteLine(CalculateComponent(price, "aicores", 10));
                            }
                            if (type == "missile")
                            {
                                sw.WriteLine(CalculateComponent(price, "energycells", 10));
                                sw.WriteLine(CalculateComponent(price, "missilecomponents", 80));
                                sw.WriteLine(CalculateComponent(price, "aicores", 10));
                            }

                            sw.WriteLine("</primary>");
                            sw.WriteLine("</replace>");
                            
                        }
                    }

                    sw.WriteLine("</diff>");
                }
            }
        }

        private static int GetPriceFromWare(string name)
        {
            return Materials.FirstOrDefault(x => x.Name == name).Price;
        }
        private static string CalculateComponent(int price, string ware, int percentage, double marginFactor = 0.2)
        {
            int warePrice = GetPriceFromWare(ware);
            double factor = percentage * 0.01 * marginFactor;
            double targetPrice = price * factor; // 0.2 = 400% margin

            int amount;
            amount = Convert.ToInt32(Math.Round(targetPrice / warePrice));

            //amount = Convert.ToInt32(Math.Round((price * percentage * 0.01),0));
            if (amount == 0)
                amount = 1;
            return "<ware ware=\"" + ware + "\" amount=\"" + amount + "\" />";
            //return "falsch";
        }

    }
}
