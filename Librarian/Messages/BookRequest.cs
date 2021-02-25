namespace Librarian.Messages
{
    public class BookRequest
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public int Direction { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Flat { get; set; }
    }
}
