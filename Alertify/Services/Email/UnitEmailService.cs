using Alertify.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Alertify.Services.Email
{
    public class UnitEmailService
    {
        private readonly EmailOptions _settings;

        public UnitEmailService(IOptions<EmailOptions> options)
        {
            _settings = options.Value;
        }

        public async Task SendUnitAssignedEmail(string toEmail, Emergency emergency, Unit unit)
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = _settings.EnableSsl
            };

            string citizenName = $"{emergency.Citizen?.FirstName} {emergency.Citizen?.FirstLastName} {emergency.Citizen?.SecondLastName}".Trim();
            if (string.IsNullOrWhiteSpace(citizenName))
                citizenName = "No especificado";

            string citizenPhone = emergency.Citizen?.Phone ?? "No disponible";

            string reference = emergency.LocationReference ?? "Sin referencia adicional";


            string subject = $"Nueva emergencia asignada - {emergency.EmergencyCategory}";


            string body = $@"
                <h2>🚨 Se le ha asignado una emergencia</h2>

                <p><strong>Unidad asignada:</strong> {unit.Name}</p>
                <p><strong>Tipo de emergencia:</strong> {emergency.EmergencyCategory}</p>
                <p><strong>Descripción:</strong> {emergency.Description}</p>
                <p><strong>Dirección:</strong> {emergency.Address}</p>
                <p><strong>Referencia adicional:</strong> {reference}</p>

                <hr/>

                <p><strong>Datos del ciudadano que reportó:</strong></p>
                <p><strong>Nombre:</strong> {citizenName}</p>
                <p><strong>Teléfono:</strong> {citizenPhone}</p>

                <hr/>

                <p><strong>Fecha de asignación:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>

                <br/>
                <p>Por favor dirigirse inmediatamente al lugar asignado y contactar al ciudadano si es necesario.</p>

                <hr/>
                <p>🔰 <strong>Alertify - Sistema de Emergencias</strong></p>
            ";

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }
    }
}
