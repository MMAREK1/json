using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scryfall.API;
using Newtonsoft.Json;
using System.IO;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var scryfall = new ScryfallClient();
            string[] riadky = File.ReadAllLines(@"cards.csv");
            var subor = File.ReadAllText(@"zoznam.json");
            var karty = JsonConvert.DeserializeObject<Scryfall.API.Models.Card[]>(subor, new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            });
            List<Scryfall.API.Models.Card> zoznam = karty.ToList<Scryfall.API.Models.Card>();
            List<string> chybaju = new List<string>();
            var aaa = zoznam.GroupBy(p => new { p.Name, p.Effects })
                       .Select(grp => grp.First())
                       .ToList();


            List<Scryfall.API.Models.Card> vyber = karty.ToList<Scryfall.API.Models.Card>();
            var zoznamset = vyber.GroupBy(p => new { p.Set })
                .Select(grp => grp.First())
                           .ToList();
            List<Scryfall.API.Models.Lista> listass = new List<Scryfall.API.Models.Lista>();
            foreach(var abd in zoznamset)
            {
                listass.Add(new Scryfall.API.Models.Lista(abd.Set.ToString(), vyber.Where(c => c.Set.ToString() == abd.Set.ToString()).Select(c => c.SetName).First().ToString()));
            }
            listass.Add(new Scryfall.API.Models.Lista("00",""));
            listass=listass.OrderBy(x => x.Code).ToList();
            // Get a random card

            //var card = scryfall.Cards.GetNamed("Hengegate Pathway", null, null,null,null, null,true);
            //
            //Console.WriteLine((card.Layout == Scryfall.API.Models.Layouts.Transform || card.Layout == Scryfall.API.Models.Layouts.ModalDfc) ? new Uri(card.CardFaces[0].ImageUris.Normal) : new Uri(card.ImageUris.Normal));


            int i = 0;
            foreach (string zaznam in riadky)
            {
                i++;
                Console.Write(i + " - " + riadky.Length);
                string[] riadok = zaznam.Split(';');
                Console.Write("  " + riadok[0] + " - " + riadok[1] + "  ");
                var card = scryfall.Cards.GetNamed((riadok[0].IndexOf("/") > 0) ? riadok[0].Substring(0, riadok[0].IndexOf("/") - 1) : riadok[0], 
                                                    null,
                                                    riadok[1].Remove(riadok[1].IndexOf(' '), 1)
                                                    );
                if (card is null)
                {
                    chybaju.Add(zaznam);
                    Console.WriteLine("Chyba");
                }
                else
                {
                    if(card.CollectorNumber!=riadok[4].Remove(riadok[4].IndexOf(' '), 1))
                    {
                        if(!string.IsNullOrEmpty(riadok[3].Remove(riadok[3].IndexOf(' '), 1)))
                        {
                                card.Effects = "FS";
                        }
                        else
                        {
                                card.Effects = "S";
                            }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(riadok[3].Remove(riadok[3].IndexOf(' '), 1)))
                        {
                            card.Effects = "F";
                        }
                    }
                    for(int p=0;p< Int32.Parse(riadok[2].Remove(riadok[2].IndexOf(' '), 1));p++)
                        zoznam.Add(card);
                    Console.WriteLine("OK");

                }
            }
            zoznam = zoznam.OrderBy(x => x.Name).ToList();
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
            serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
            serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
            using (StreamWriter sw = new StreamWriter(@"zoznam.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, zoznam.ToArray());
            }
            using (StreamWriter outputFile = new StreamWriter("WriteLines.txt"))
            {
                foreach (string line in chybaju)
                    outputFile.WriteLine(line);
            }
            Console.ReadLine();
        }
    }
}
