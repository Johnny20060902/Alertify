namespace Alertify.Services.Email
{
    public class EmailTemplateService
    {
        public string GetForgotPasswordTemplate(string userName, string temporaryPassword)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #d32f2f;'>Alertify - Recuperación de Contraseña</h2>
        <p>Hola {userName},</p>
        <p>Has solicitado restablecer tu contraseña. Tu contraseña temporal es:</p>
        <div style='background-color: #f5f5f5; padding: 15px; margin: 20px 0; border-left: 4px solid #d32f2f;'>
            <strong style='font-size: 18px; letter-spacing: 2px;'>{temporaryPassword}</strong>
        </div>
        <p>Por favor, inicia sesión con esta contraseña y cámbiala inmediatamente por una nueva.</p>
        <p style='color: #666; font-size: 12px; margin-top: 30px;'>
            Este es un correo automático, por favor no respondas a este mensaje.
        </p>
    </div>
</body>
</html>";
        }

        //// Puedes agregar más templates conforme los necesites
        //public string GetWelcomeTemplate(string userName)
        //{
        //    // Template de bienvenida
        //}

        //public string GetEmailVerificationTemplate(string userName, string verificationLink)
        //{
        //    // Template de verificación de email
        //}
    }
}