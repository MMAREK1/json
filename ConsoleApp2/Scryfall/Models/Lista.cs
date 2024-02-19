namespace Scryfall.API.Models
{
    public class Lista
    {
        public Lista(string code, string name)
        {
            Code = code;
            Name = name;
        }
        public string Code { get; set; }
        public string Name { get; set; }

    }
}