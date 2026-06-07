using System.Text.Json;
using GopherNote.Models;
using Microsoft.JSInterop;

namespace GopherNote.Services;

public class NoteService(IJSRuntime js)
{
    private const string StorageKey = "gopher_notes";

    public async Task<List<Note>> GetNotesAsync()
    {
        try
        {
            var json = await js.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (string.IsNullOrWhiteSpace(json)) return new List<Note>();

            var notes = JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
        
            // Убеждаемся, что у всех заметок есть TemplateClass
            foreach (var note in notes)
            {
                if (string.IsNullOrEmpty(note.TemplateClass))
                {
                    note.TemplateClass = "note-template-2";  // Меняем с 1 на 2
                }
                if (note.Tags == null) note.Tags = new List<string>();
            }
        
            return notes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JS Interop Error: {ex.Message}");
            return new List<Note>();
        }
    }

    public async Task SaveNoteAsync(Note note)
    {
        var notes = await GetNotesAsync();
    
        // Убеждаемся, что TemplateClass установлен
        if (string.IsNullOrEmpty(note.TemplateClass))
        {
            note.TemplateClass = "note-template-2";  
        }
        
        // Убеждаемся, что Tags не null
        if (note.Tags == null) note.Tags = new List<string>();
        
        // Обновляем дату
        note.UpdatedAt = DateTime.Now;
        
        // Для новых заметок устанавливаем Id и CreatedAt
        if (note.Id == 0)
        {
            note.Id = notes.Any() ? notes.Max(n => n.Id) + 1 : 1;
            note.CreatedAt = DateTime.Now;
        }
        
        var existingNote = notes.FirstOrDefault(n => n.Id == note.Id);
        if (existingNote != null)
        {
            notes.Remove(existingNote);
        }
        
        notes.Add(note);
        
        // Сортируем по закрепленным и дате
        notes = notes.OrderByDescending(n => n.IsPinned)
                     .ThenByDescending(n => n.UpdatedAt)
                     .ToList();
        
        var json = JsonSerializer.Serialize(notes);
        await js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        
        Console.WriteLine($"Сохранена заметка: ID={note.Id}, Title={note.Title}, TemplateClass={note.TemplateClass}");
    }

    public async Task DeleteNoteAsync(int id)  // Изменено с Guid на int
    {
        var notes = await GetNotesAsync();
        var note = notes.FirstOrDefault(n => n.Id == id);
        
        if (note != null)
        {
            notes.Remove(note);
            var json = JsonSerializer.Serialize(notes);
            await js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
            Console.WriteLine($"Удалена заметка: ID={id}");
        }
    }
    
    // Перегрузка для совместимости, если где-то используется Guid
    public async Task DeleteNoteAsync(Guid id)
    {
        var notes = await GetNotesAsync();
        var note = notes.FirstOrDefault(n => n.Id.ToString() == id.ToString());
        
        if (note != null)
        {
            await DeleteNoteAsync(note.Id);
        }
    }
}