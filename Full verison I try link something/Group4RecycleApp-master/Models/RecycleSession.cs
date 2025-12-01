using System.Collections.ObjectModel;

namespace Group4RecycleApp.Models
{
    public class RecycleSession
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; } 
        public string Status { get; set; } 
        public string StatusColor { get; set; } 

        // Detail Data
        public int TotalItems { get; set; }
        public string Method { get; set; } 
        public string Time { get; set; } 
        public string PhotoImage { get; set; }
       
        public List<RecycledItemDetail> Items { get; set; } = new();
    }

    public class RecycledItemDetail
    {
        public string Name { get; set; } 
        public string Quantity { get; set; } 
        public string Icon { get; set; } 
    }
}