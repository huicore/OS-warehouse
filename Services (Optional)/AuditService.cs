using AssetManagementSystem.Data;

public class AuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogActionAsync(int userId, string action, string details)
    {
        try
        {
            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                Details = details
            });
            await _context.SaveChangesAsync();
        }
        catch { /* Логирование в файл, если БД недоступна */ }
    }
}