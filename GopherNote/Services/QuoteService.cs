using GopherNote.Models;

namespace GopherNote.Services;

public class QuoteService
{
    public string GetContextualQuote(List<Note> notes)
    {
        // Пустой список
        if (notes == null || !notes.Any())
            return "Чистый лист — начало великих идей. Создайте свою первую заметку.";

        var now = DateTime.Now;
        
        
        var lastTouchNote = notes.MaxBy(n => n.CreatedAt); 
        var lastActivityTime = lastTouchNote?.CreatedAt ?? now; 
        var minutesSinceLastActivity = (now - lastActivityTime).TotalMinutes;
        var daysSinceLastActivity = (now - lastActivityTime).TotalDays;

        // ==========================================

        // Пользователь только что что-то сохранил/изменил после долгого перерыва
        if (minutesSinceLastActivity < 1 && daysSinceLastActivity > 3)
        {
            return "С возвращением! Главное в продуктивности — это последовательность, а не скорость.";
        }

        // Пользователь застрял в цикле бесконечного создания заметок за последние 15 минут
        var updatesInLastHour = notes.Count(n => (now - n.CreatedAt).TotalMinutes < 60); 
        if (updatesInLastHour > 5)
        {
            return "Прогресс идет полным ходом. Но не забывайте фиксировать мысли, а не бесконечно их полировать.";
        }

        // ==========================================

        // Проверка на выгорание / перегрузку в течение дня
        var notesCreatedToday = notes.Count(n => (now - n.CreatedAt).TotalDays < 1);
        if (notesCreatedToday > 10)
        {
            return "Вы создали очень много заметок сегодня. Не забывайте об отдыхе — выгорание не дремлет.";
        }

        // Проверка на застой (прокрастинацию)
        if (daysSinceLastActivity > 7)
        {
            return "Прокрастинация — вор времени. Давно мы не видели новых записей или правок!";
        }

        // Анализ фокуса (слишком много закрепов)
        var pinnedCount = notes.Count(n => n.IsPinned);
        if (pinnedCount > 5)
        {
            return "Слишком много приоритетов означает отсутствие приоритетов. Почистите закрепленные заметки.";
        }

        // НОВАЯ ИДЕЯ: Анализ хаоса в тегах
        var totalTags = notes.Where(n => n.Tags != null).SelectMany(n => n.Tags).Distinct().Count();
        if (totalTags > 15 && notes.Count < 10)
        {
            return "У вас тегов больше, чем самих заметок. Возможно, пора упростить структуру?";
        }

        // Замечания о многословности/графомании
        var avgContentLength = notes.Average(n => n.Content?.Length ?? 0);
        if (avgContentLength > 2000)
        {
            return "Ваши заметки похожи на главы из книг. Попробуйте выделить из них ключевые тезисы.";
        }
        
        
        // Если другие триггеры не сработали
        return GetTimeOfDayQuote(now.Hour);
    }

    private string GetTimeOfDayQuote(int hour)
    {
        return hour switch
        {
            >= 0 and < 5 => "Ночь — время для сна, а не для кода и заметок. Утро вечера мудренее, отдохните.",
            >= 5 and < 11 => "Доброе утро! Свежий взгляд и чистый разум — лучшие инструменты для планирования дня.",
            >= 11 and < 17 => "День в самом разгаре. Сфокусируйтесь на главной задаче, отключив уведомления.",
            >= 17 and < 22 => "Вечер — отличное время, чтобы подвести итоги и разгрузить голову перед завтрашним днем.",
            _ => "День близится к концу. Запишите оставшиеся мысли, чтобы не думать о них ночью."
        };
    }
}