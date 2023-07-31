namespace MyLifeApp.Infrastructure.Shared.Services.Email
{
    public interface IEmailService
    {
        public Task SendMailAsync(SendMailRequest request); 
    }
}