namespace Group4RecycleApp.Models
{
    public class RecyclingGuide
    {
        // 1. 新增 ID (資料庫的主鍵)
        public int Id { get; set; }

        public string Title { get; set; }

        // 2. Icon 改名為 ImageUrl (為了對應後端欄位)
        public string ImageUrl { get; set; }

        // 3. VideoSource 改為 string VideoUrl (存 YouTube 網址)
        public string VideoUrl { get; set; }

        // 4. JournalUrl 改為 ArticleUrl (為了對應後端欄位)
        public string ArticleUrl { get; set; }

        // 5. 這些是前端為了美觀保留的欄位，給個預設值以免報錯
        public string Color { get; set; } = "#6F85FD";
        public string CreditName { get; set; } = "RecycleRight";
    }
}