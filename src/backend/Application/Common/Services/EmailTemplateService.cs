using HospitalAppointmentSystem.Application.Common.Interfaces;

namespace HospitalAppointmentSystem.Application.Common.Services;

/// <summary>
/// Service for generating email templates with HTML formatting
/// </summary>
public class EmailTemplateService : IEmailTemplateService
{
    private const string PrimaryColor = "#4a90e2";
    private const string SuccessColor = "#4caf50";
    private const string WarningColor = "#ff9800";
    private const string DangerColor = "#f44336";
    private const string TextColor = "#333333";
    private const string LightGray = "#f5f5f5";

    public string GenerateAppointmentConfirmation(
        string patientName,
        string doctorName,
        string specialty,
        DateTime appointmentDate,
        string appointmentTime,
        string reason)
    {
        var formattedDate = appointmentDate.ToString("dddd, MMMM dd, yyyy");

        return GenerateEmailTemplate(
            "Appointment Confirmed",
            SuccessColor,
            $@"
                <h1 style='color: {SuccessColor}; margin-bottom: 20px;'>Appointment Confirmed!</h1>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 20px;'>
                    Dear {patientName},
                </p>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 30px;'>
                    Your appointment has been successfully scheduled. Here are the details:
                </p>

                <table style='width: 100%; background: {LightGray}; border-radius: 8px; padding: 20px; margin-bottom: 30px;'>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Doctor:</td>
                        <td style='padding: 10px; color: {TextColor};'>{doctorName}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Specialty:</td>
                        <td style='padding: 10px; color: {TextColor};'>{specialty}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Date:</td>
                        <td style='padding: 10px; color: {TextColor};'>{formattedDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Time:</td>
                        <td style='padding: 10px; color: {TextColor};'>{appointmentTime}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Reason:</td>
                        <td style='padding: 10px; color: {TextColor};'>{reason}</td>
                    </tr>
                </table>

                <p style='font-size: 14px; color: #666; margin-bottom: 20px;'>
                    <strong>Important:</strong> Please arrive 10 minutes before your scheduled time.
                </p>
                <p style='font-size: 14px; color: #666;'>
                    If you need to reschedule or cancel, please do so at least 24 hours in advance.
                </p>
            ");
    }

    public string GenerateAppointmentReminder(
        string patientName,
        string doctorName,
        string specialty,
        DateTime appointmentDate,
        string appointmentTime,
        string reason)
    {
        var formattedDate = appointmentDate.ToString("dddd, MMMM dd, yyyy");

        return GenerateEmailTemplate(
            "Appointment Reminder",
            WarningColor,
            $@"
                <h1 style='color: {WarningColor}; margin-bottom: 20px;'>Appointment Reminder</h1>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 20px;'>
                    Dear {patientName},
                </p>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 30px;'>
                    This is a friendly reminder about your upcoming appointment <strong>tomorrow</strong>:
                </p>

                <table style='width: 100%; background: {LightGray}; border-radius: 8px; padding: 20px; margin-bottom: 30px;'>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Doctor:</td>
                        <td style='padding: 10px; color: {TextColor};'>{doctorName}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Specialty:</td>
                        <td style='padding: 10px; color: {TextColor};'>{specialty}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Date:</td>
                        <td style='padding: 10px; color: {TextColor};'>{formattedDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Time:</td>
                        <td style='padding: 10px; color: {TextColor};'>{appointmentTime}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Reason:</td>
                        <td style='padding: 10px; color: {TextColor};'>{reason}</td>
                    </tr>
                </table>

                <div style='background: #fff3cd; border-left: 4px solid {WarningColor}; padding: 15px; margin-bottom: 20px; border-radius: 4px;'>
                    <p style='margin: 0; color: {TextColor};'>
                        <strong>⏰ Don't forget:</strong> Please arrive 10 minutes early.
                    </p>
                </div>

                <p style='font-size: 14px; color: #666;'>
                    If you cannot make it, please cancel or reschedule as soon as possible.
                </p>
            ");
    }

    public string GenerateAppointmentCancellation(
        string patientName,
        string doctorName,
        DateTime appointmentDate,
        string appointmentTime,
        string cancellationReason)
    {
        var formattedDate = appointmentDate.ToString("dddd, MMMM dd, yyyy");

        return GenerateEmailTemplate(
            "Appointment Cancelled",
            DangerColor,
            $@"
                <h1 style='color: {DangerColor}; margin-bottom: 20px;'>Appointment Cancelled</h1>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 20px;'>
                    Dear {patientName},
                </p>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 30px;'>
                    Your appointment has been cancelled:
                </p>

                <table style='width: 100%; background: {LightGray}; border-radius: 8px; padding: 20px; margin-bottom: 30px;'>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Doctor:</td>
                        <td style='padding: 10px; color: {TextColor};'>{doctorName}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Date:</td>
                        <td style='padding: 10px; color: {TextColor};'>{formattedDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Time:</td>
                        <td style='padding: 10px; color: {TextColor};'>{appointmentTime}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Reason for Cancellation:</td>
                        <td style='padding: 10px; color: {TextColor};'>{cancellationReason}</td>
                    </tr>
                </table>

                <p style='font-size: 14px; color: #666;'>
                    If you need to schedule a new appointment, please log in to your account.
                </p>
            ");
    }

    public string GenerateAppointmentRescheduled(
        string patientName,
        string doctorName,
        DateTime oldDate,
        string oldTime,
        DateTime newDate,
        string newTime)
    {
        var formattedOldDate = oldDate.ToString("dddd, MMMM dd, yyyy");
        var formattedNewDate = newDate.ToString("dddd, MMMM dd, yyyy");

        return GenerateEmailTemplate(
            "Appointment Rescheduled",
            PrimaryColor,
            $@"
                <h1 style='color: {PrimaryColor}; margin-bottom: 20px;'>Appointment Rescheduled</h1>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 20px;'>
                    Dear {patientName},
                </p>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 30px;'>
                    Your appointment with {doctorName} has been rescheduled:
                </p>

                <table style='width: 100%; margin-bottom: 30px;'>
                    <tr>
                        <td style='width: 45%; vertical-align: top;'>
                            <div style='background: #ffebee; border-radius: 8px; padding: 20px;'>
                                <h3 style='color: {DangerColor}; margin-top: 0;'>Previous Appointment</h3>
                                <p style='margin: 5px 0; color: {TextColor};'><strong>Date:</strong> {formattedOldDate}</p>
                                <p style='margin: 5px 0; color: {TextColor};'><strong>Time:</strong> {oldTime}</p>
                            </div>
                        </td>
                        <td style='width: 10%; text-align: center; vertical-align: middle;'>
                            <span style='font-size: 24px;'>→</span>
                        </td>
                        <td style='width: 45%; vertical-align: top;'>
                            <div style='background: #e8f5e9; border-radius: 8px; padding: 20px;'>
                                <h3 style='color: {SuccessColor}; margin-top: 0;'>New Appointment</h3>
                                <p style='margin: 5px 0; color: {TextColor};'><strong>Date:</strong> {formattedNewDate}</p>
                                <p style='margin: 5px 0; color: {TextColor};'><strong>Time:</strong> {newTime}</p>
                            </div>
                        </td>
                    </tr>
                </table>

                <p style='font-size: 14px; color: #666;'>
                    Please arrive 10 minutes before your new scheduled time.
                </p>
            ");
    }

    public string GenerateDoctorNewAppointmentNotification(
        string doctorName,
        string patientName,
        DateTime appointmentDate,
        string appointmentTime,
        string reason)
    {
        var formattedDate = appointmentDate.ToString("dddd, MMMM dd, yyyy");

        return GenerateEmailTemplate(
            "New Appointment Scheduled",
            PrimaryColor,
            $@"
                <h1 style='color: {PrimaryColor}; margin-bottom: 20px;'>New Appointment Scheduled</h1>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 20px;'>
                    Dear Dr. {doctorName},
                </p>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 30px;'>
                    A new appointment has been scheduled with you:
                </p>

                <table style='width: 100%; background: {LightGray}; border-radius: 8px; padding: 20px; margin-bottom: 30px;'>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Patient:</td>
                        <td style='padding: 10px; color: {TextColor};'>{patientName}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Date:</td>
                        <td style='padding: 10px; color: {TextColor};'>{formattedDate}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Time:</td>
                        <td style='padding: 10px; color: {TextColor};'>{appointmentTime}</td>
                    </tr>
                    <tr>
                        <td style='padding: 10px; font-weight: bold; color: {TextColor};'>Reason:</td>
                        <td style='padding: 10px; color: {TextColor};'>{reason}</td>
                    </tr>
                </table>

                <p style='font-size: 14px; color: #666;'>
                    You can view full appointment details by logging into your dashboard.
                </p>
            ");
    }

    public string GenerateAppointmentCompleted(
        string patientName,
        string doctorName,
        DateTime appointmentDate,
        string notes)
    {
        var formattedDate = appointmentDate.ToString("dddd, MMMM dd, yyyy");

        return GenerateEmailTemplate(
            "Appointment Completed",
            SuccessColor,
            $@"
                <h1 style='color: {SuccessColor}; margin-bottom: 20px;'>Appointment Completed</h1>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 20px;'>
                    Dear {patientName},
                </p>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 30px;'>
                    Your appointment with {doctorName} on {formattedDate} has been completed.
                </p>

                {(string.IsNullOrWhiteSpace(notes) ? "" : $@"
                <div style='background: {LightGray}; border-radius: 8px; padding: 20px; margin-bottom: 30px;'>
                    <h3 style='color: {TextColor}; margin-top: 0;'>Doctor's Notes:</h3>
                    <p style='color: {TextColor}; line-height: 1.6;'>{notes}</p>
                </div>
                ")}

                <p style='font-size: 14px; color: #666;'>
                    Thank you for choosing our hospital. We hope you had a positive experience.
                </p>
                <p style='font-size: 14px; color: #666;'>
                    If you have any questions or need to schedule a follow-up appointment, please contact us.
                </p>
            ");
    }

    public string GenerateWelcomeEmail(string userName, string userRole)
    {
        return GenerateEmailTemplate(
            "Welcome to Hospital Appointment System",
            PrimaryColor,
            $@"
                <h1 style='color: {PrimaryColor}; margin-bottom: 20px;'>Welcome to Our Hospital!</h1>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 20px;'>
                    Dear {userName},
                </p>
                <p style='font-size: 16px; color: {TextColor}; margin-bottom: 30px;'>
                    Welcome to the Hospital Appointment Management System! Your account has been successfully created as a <strong>{userRole}</strong>.
                </p>

                <div style='background: {LightGray}; border-radius: 8px; padding: 20px; margin-bottom: 30px;'>
                    <h3 style='color: {TextColor}; margin-top: 0;'>Getting Started:</h3>
                    <ul style='color: {TextColor}; line-height: 1.8;'>
                        {(userRole.ToLower() == "patient" ? @"
                            <li>Browse available doctors and specialties</li>
                            <li>Schedule appointments at your convenience</li>
                            <li>View and manage your appointment history</li>
                            <li>Receive email reminders for upcoming appointments</li>
                        " : userRole.ToLower() == "doctor" ? @"
                            <li>View your appointment schedule</li>
                            <li>Manage patient appointments</li>
                            <li>Add notes and complete consultations</li>
                            <li>Track your weekly statistics</li>
                        " : @"
                            <li>Monitor system statistics</li>
                            <li>Manage users and appointments</li>
                            <li>View comprehensive reports</li>
                            <li>Configure system settings</li>
                        ")}
                    </ul>
                </div>

                <p style='font-size: 14px; color: #666;'>
                    If you have any questions, please don't hesitate to contact our support team.
                </p>
            ");
    }

    /// <summary>
    /// Generate base email template with header and footer
    /// </summary>
    private string GenerateEmailTemplate(string subject, string accentColor, string bodyContent)
    {
        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>{subject}</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table cellpadding='0' cellspacing='0' border='0' width='100%' style='background-color: #f4f4f4; padding: 20px 0;'>
        <tr>
            <td align='center'>
                <table cellpadding='0' cellspacing='0' border='0' width='600' style='background-color: white; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, {accentColor} 0%, {accentColor}dd 100%); padding: 30px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <h2 style='margin: 0; color: white; font-size: 24px;'>🏥 Hospital Appointment System</h2>
                        </td>
                    </tr>

                    <!-- Body -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            {bodyContent}
                        </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                        <td style='background-color: {LightGray}; padding: 20px 30px; text-align: center; border-radius: 0 0 8px 8px;'>
                            <p style='margin: 0 0 10px 0; font-size: 14px; color: #666;'>
                                Hospital Appointment Management System
                            </p>
                            <p style='margin: 0; font-size: 12px; color: #999;'>
                                This is an automated email. Please do not reply to this message.
                            </p>
                            <p style='margin: 10px 0 0 0; font-size: 12px; color: #999;'>
                                © {DateTime.UtcNow.Year} Hospital Appointment System. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
