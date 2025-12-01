namespace Group4RecycleApp.Models
{
    public class AdminRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string RequestType { get; set; }
        public string Icon { get; set; }
        public bool IsPending { get; set; } = true;
    }
}