using MEDIPLAN.Data;
using MEDIPLAN.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public class TerminReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public TerminReminderService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var sada = DateTime.Now;
                var granica = sada.AddHours(24);
                var granicaDonja = sada.AddHours(23.5); // tolerancija da se ne salje vise puta

                var termini = await context.Termini
                    .Include(t => t.Pacijent)
                    .Where(t => t.DatumVrijemePocetak >= granicaDonja && t.DatumVrijemePocetak <= granica)
                    .ToListAsync(stoppingToken);

                foreach (var termin in termini)
                {
                    var email = termin.Pacijent.Email;
                    var ime = termin.Pacijent.Ime;
                    var datum = termin.DatumVrijemePocetak.ToString("dd.MM.yyyy. HH:mm");

                    var body = $@"
                        <p>Poštovani/a {ime},</p>
                        <p>Podsjećamo vas da imate zakazan termin u MEDIPLAN klinici za <strong>{datum}</strong>.</p>
                        <p>Molimo vas da dođete 10 minuta ranije.</p>
                        <br/>
                        <p>S poštovanjem,<br/>MEDIPLAN</p>";

                    await emailService.SendEmailAsync(email, "Podsjetnik: termin u MEDIPLAN klinici", body);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken); // provjera svakih sat vremena
        }
    }
}
