public class AuditLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string Action { get; set; } // "Удаление ОС", "Изменение пароля"
    public string Details { get; set; } // JSON с изменениями
    public DateTime Timestamp { get; set; } = DateTime.Now;
}