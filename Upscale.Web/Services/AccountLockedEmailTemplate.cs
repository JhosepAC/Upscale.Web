namespace Upscale.Web.Services
{
    public static class AccountLockedEmailTemplate
    {
        public static string Build(
            string fullName,
            string documentNumber,
            int lockoutMinutes,
            DateTime lockoutEnd)
        {
            var lockoutEndFormatted = lockoutEnd.ToString("dd/MM/yyyy 'a las' HH:mm");

            return $"""
<!DOCTYPE html>
<html lang="es" xmlns="http://www.w3.org/1999/xhtml">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <meta http-equiv="X-UA-Compatible" content="IE=edge" />
  <title>Cuenta bloqueada — CEPLAN</title>
  <!--[if mso]>
  <noscript>
    <xml><o:OfficeDocumentSettings>
      <o:PixelsPerInch>96</o:PixelsPerInch>
    </o:OfficeDocumentSettings></xml>
  </noscript>
  <![endif]-->
</head>
<body style="margin:0;padding:0;background-color:#f4f4f4;font-family:'Roboto',Arial,sans-serif;">

  <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0"
         style="background-color:#f4f4f4;">
    <tr>
      <td align="center" style="padding:32px 16px;">

        <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0"
               style="max-width:600px;background-color:#ffffff;border-radius:10px;
                      overflow:hidden;box-shadow:0 2px 12px rgba(0,0,0,0.10);">

          <tr>
            <td style="background-color:#A32D2F;padding:28px 36px;
                       border-bottom:4px solid #8B2527;">
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0">
                <tr>
                  <td>
                    <span style="font-size:22px;font-weight:700;color:#ffffff;
                                 letter-spacing:1px;font-family:Arial,sans-serif;">
                      CEPLAN
                    </span>
                    <span style="display:block;font-size:11px;color:rgba(255,255,255,0.75);
                                 margin-top:2px;font-family:Arial,sans-serif;">
                      Centro Nacional de Planeamiento Estratégico
                    </span>
                  </td>
                  <td align="right" style="vertical-align:middle;">
                    <span style="display:inline-block;width:44px;height:44px;
                                 background-color:rgba(255,255,255,0.15);
                                 border-radius:50%;text-align:center;line-height:44px;
                                 font-size:20px;">
                      🔒
                    </span>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <tr>
            <td style="background-color:#FEF2F2;padding:20px 36px;
                       border-bottom:1px solid #FECACA;">
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0">
                <tr>
                  <td style="vertical-align:middle;padding-right:14px;width:48px;">
                    <span style="display:inline-block;width:44px;height:44px;
                                 background-color:#FEE2E2;border-radius:50%;
                                 text-align:center;line-height:44px;font-size:22px;">
                      ⚠️
                    </span>
                  </td>
                  <td>
                    <p style="margin:0;font-size:15px;font-weight:700;color:#B91C1C;
                               font-family:Arial,sans-serif;">
                      Cuenta bloqueada temporalmente
                    </p>
                    <p style="margin:4px 0 0;font-size:13px;color:#DC2626;
                               font-family:Arial,sans-serif;">
                      Se detectaron múltiples intentos fallidos de acceso
                    </p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

          <tr>
            <td style="padding:32px 36px 24px;">

              <p style="margin:0 0 16px;font-size:15px;color:#333333;
                         font-family:Arial,sans-serif;">
                Estimado/a <strong>{fullName}</strong>,
              </p>

              <p style="margin:0 0 20px;font-size:14px;color:#555555;line-height:1.65;
                         font-family:Arial,sans-serif;">
                Le informamos que el acceso a su cuenta en el sistema
                <strong>CEPLAN</strong> ha sido
                <strong style="color:#B91C1C;">bloqueado temporalmente</strong>
                debido a que se superó el límite de intentos de inicio de sesión
                fallidos permitidos (<strong>5 intentos</strong>).
              </p>

              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0"
                     style="background-color:#F8FAFC;border:1px solid #E2E8F0;
                            border-radius:8px;margin-bottom:24px;">
                <tr>
                  <td style="padding:20px 24px;">

                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0"
                           style="margin-bottom:12px;">
                      <tr>
                        <td style="width:50%;vertical-align:top;">
                          <span style="font-size:11px;font-weight:700;color:#94A3B8;
                                       letter-spacing:0.8px;text-transform:uppercase;
                                       font-family:Arial,sans-serif;">
                            N.º de Documento
                          </span>
                          <p style="margin:4px 0 0;font-size:14px;font-weight:600;
                                     color:#1E293B;font-family:Arial,sans-serif;">
                            {documentNumber}
                          </p>
                        </td>
                        <td style="width:50%;vertical-align:top;">
                          <span style="font-size:11px;font-weight:700;color:#94A3B8;
                                       letter-spacing:0.8px;text-transform:uppercase;
                                       font-family:Arial,sans-serif;">
                            Duración del bloqueo
                          </span>
                          <p style="margin:4px 0 0;font-size:14px;font-weight:600;
                                     color:#1E293B;font-family:Arial,sans-serif;">
                            {lockoutMinutes} minutos
                          </p>
                        </td>
                      </tr>
                    </table>

                    <hr style="border:none;border-top:1px solid #E2E8F0;margin:0 0 12px;" />

                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0">
                      <tr>
                        <td>
                          <span style="font-size:11px;font-weight:700;color:#94A3B8;
                                       letter-spacing:0.8px;text-transform:uppercase;
                                       font-family:Arial,sans-serif;">
                            Podrá intentarlo nuevamente el
                          </span>
                          <p style="margin:4px 0 0;font-size:15px;font-weight:700;
                                     color:#0055AB;font-family:Arial,sans-serif;">
                            📅 {lockoutEndFormatted}
                          </p>
                        </td>
                      </tr>
                    </table>

                  </td>
                </tr>
              </table>

              <p style="margin:0 0 10px;font-size:13px;font-weight:700;color:#333333;
                         font-family:Arial,sans-serif;">
                Recomendaciones de seguridad:
              </p>
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0"
                     style="margin-bottom:24px;">
                <tr>
                  <td style="padding-left:8px;">
                    <p style="margin:0 0 6px;font-size:13px;color:#555555;
                               font-family:Arial,sans-serif;">
                      ✅ &nbsp;Verifique que está usando la contraseña correcta.
                    </p>
                    <p style="margin:0 0 6px;font-size:13px;color:#555555;
                               font-family:Arial,sans-serif;">
                      ✅ &nbsp;Active el bloqueo de mayúsculas si lo usa.
                    </p>
                    <p style="margin:0 0 6px;font-size:13px;color:#555555;
                               font-family:Arial,sans-serif;">
                      ✅ &nbsp;Si olvidó su contraseña, comuníquese con soporte.
                    </p>
                    <p style="margin:0;font-size:13px;color:#555555;
                               font-family:Arial,sans-serif;">
                      ✅ &nbsp;Si usted no realizó estos intentos, notifique inmediatamente a TI.
                    </p>
                  </td>
                </tr>
              </table>

              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0"
                     style="background-color:#EFF6FF;border-left:4px solid #0055AB;
                            border-radius:0 6px 6px 0;margin-bottom:8px;">
                <tr>
                  <td style="padding:14px 18px;">
                    <p style="margin:0;font-size:13px;color:#1E40AF;line-height:1.6;
                               font-family:Arial,sans-serif;">
                      <strong>ℹ️ Nota de seguridad:</strong> Si usted no intentó
                      ingresar al sistema, su cuenta podría estar siendo objeto de
                      un acceso no autorizado. Por favor, comuníquese con el área
                      de Tecnología de la Información de CEPLAN.
                    </p>
                  </td>
                </tr>
              </table>

            </td>
          </tr>

          <tr>
            <td style="padding:0 36px 32px;text-align:center;">
              <p style="margin:0 0 16px;font-size:13px;color:#94A3B8;
                         font-family:Arial,sans-serif;">
                Tras el período de bloqueo, podrá intentarlo nuevamente desde:
              </p>
              <a href="https://localhost:7178/Account/Login"
                 style="display:inline-block;background-color:#0055AB;color:#ffffff;
                        text-decoration:none;font-size:14px;font-weight:600;
                        padding:12px 32px;border-radius:6px;
                        font-family:Arial,sans-serif;">
                Ir al inicio de sesión
              </a>
            </td>
          </tr>

          <tr>
            <td style="padding:0 36px;">
              <hr style="border:none;border-top:1px solid #E8EAED;margin:0;" />
            </td>
          </tr>

          <tr>
            <td style="background-color:#F8F9FA;padding:20px 36px;border-radius:0 0 10px 10px;">
              <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0">
                <tr>
                  <td>
                    <p style="margin:0 0 4px;font-size:12px;font-weight:700;color:#A32D2F;
                               font-family:Arial,sans-serif;">
                      CEPLAN — Centro Nacional de Planeamiento Estratégico
                    </p>
                    <p style="margin:0;font-size:11px;color:#94A3B8;line-height:1.5;
                               font-family:Arial,sans-serif;">
                      Este es un correo generado automáticamente. Por favor, no responda
                      a este mensaje. Si necesita asistencia, contáctese con el área de
                      soporte técnico.
                    </p>
                  </td>
                </tr>
                <tr>
                  <td style="padding-top:12px;">
                    <p style="margin:0;font-size:11px;color:#CBD5E1;
                               font-family:Arial,sans-serif;">
                      © {DateTime.Now.Year} CEPLAN. Todos los derechos reservados. |
                      Av. Canaval y Moreyra 480, San Isidro, Lima, Perú.
                    </p>
                  </td>
                </tr>
              </table>
            </td>
          </tr>

        </table>

      </td>
    </tr>
  </table>

</body>
</html>
""";
        }
    }
}
