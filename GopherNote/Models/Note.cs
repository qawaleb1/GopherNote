namespace GopherNote.Models;

public class Note
{
    public int Id { get; set; }  // Убедитесь, что Id - int, не Guid
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Color { get; set; } = "";
    public string TextColor { get; set; } = "#00000000";
    public bool IsPinned { get; set; } = false;
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public string? TemplateClass { get; set; } = "note-template-2";  
}