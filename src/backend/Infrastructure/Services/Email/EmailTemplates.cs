namespace HospitalAppointmentSystem.Infrastructure.Services.Email;

/// <summary>
/// Email templates for various notifications
/// </summary>
public static class EmailTemplates
{
    public static string GetRegistrationConfirmationTemplate(string firstName, string lastName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #2196F3;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .content {{
            background-color: #f9f9f9;
            padding: 30px;
            border: 1px solid #ddd;
            border-radius: 0 0 5px 5px;
        }}
        .button {{
            display: inline-block;
            padding: 12px 24px;
            background-color: #2196F3;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .footer {{
            text-align: center;
            margin-top: 20px;
            color: #666;
            font-size: 12px;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Welcome to Hospital Appointment System</h1>
    </div>
    <div class=""content"">
        <h2>Hello {firstName} {lastName},</h2>
        <p>Thank you for registering with our Hospital Appointment System!</p>
        <p>Your account has been successfully created. You can now:</p>
        <ul>
            <li>Search for doctors by specialty and location</li>
            <li>Book appointments with available doctors</li>
            <li>Manage your appointments</li>
            <li>View your appointment history</li>
        </ul>
        <p>We're here to make your healthcare experience as smooth as possible.</p>
        <a href=""http://localhost:4200/auth/login"" class=""button"">Login to Your Account</a>
        <p>If you have any questions, please don't hesitate to contact our support team.</p>
    </div>
    <div class=""footer"">
        <p>&copy; 2026 Hospital Appointment System. All rights reserved.</p>
        <p>This is an automated email. Please do not reply to this message.</p>
    </div>
</body>
</html>";
    }

    public static string GetAppointmentConfirmationTemplate(
        string patientName,
        string doctorName,
        string specialty,
        DateTime appointmentDate,
        string reason)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #4CAF50;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .content {{
            background-color: #f9f9f9;
            padding: 30px;
            border: 1px solid #ddd;
            border-radius: 0 0 5px 5px;
        }}
        .appointment-details {{
            background-color: white;
            padding: 20px;
            border-left: 4px solid #4CAF50;
            margin: 20px 0;
        }}
        .detail-row {{
            margin: 10px 0;
        }}
        .label {{
            font-weight: bold;
            color: #666;
        }}
        .footer {{
            text-align: center;
            margin-top: 20px;
            color: #666;
            font-size: 12px;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>✓ Appointment Confirmed</h1>
    </div>
    <div class=""content"">
        <h2>Hello {patientName},</h2>
        <p>Your appointment has been successfully confirmed!</p>
        <div class=""appointment-details"">
            <div class=""detail-row"">
                <span class=""label"">Doctor:</span> Dr. {doctorName}
            </div>
            <div class=""detail-row"">
                <span class=""label"">Specialty:</span> {specialty}
            </div>
            <div class=""detail-row"">
                <span class=""label"">Date & Time:</span> {appointmentDate:dddd, MMMM dd, yyyy 'at' hh:mm tt}
            </div>
            <div class=""detail-row"">
                <span class=""label"">Reason:</span> {reason}
            </div>
        </div>
        <p><strong>Important reminders:</strong></p>
        <ul>
            <li>Please arrive 10 minutes before your appointment time</li>
            <li>Bring any relevant medical records or test results</li>
            <li>If you need to reschedule or cancel, please do so at least 24 hours in advance</li>
        </ul>
        <p>We look forward to seeing you!</p>
    </div>
    <div class=""footer"">
        <p>&copy; 2026 Hospital Appointment System. All rights reserved.</p>
    </div>
</body>
</html>";
    }

    public static string GetAppointmentReminderTemplate(
        string patientName,
        string doctorName,
        DateTime appointmentDate,
        int hoursUntil)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #FF9800;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .content {{
            background-color: #f9f9f9;
            padding: 30px;
            border: 1px solid #ddd;
            border-radius: 0 0 5px 5px;
        }}
        .reminder-box {{
            background-color: #FFF3E0;
            border: 2px solid #FF9800;
            padding: 20px;
            margin: 20px 0;
            text-align: center;
            border-radius: 5px;
        }}
        .time-highlight {{
            font-size: 24px;
            font-weight: bold;
            color: #E65100;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>⏰ Appointment Reminder</h1>
    </div>
    <div class=""content"">
        <h2>Hello {patientName},</h2>
        <p>This is a friendly reminder about your upcoming appointment:</p>
        <div class=""reminder-box"">
            <p class=""time-highlight"">In {hoursUntil} hours</p>
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Date & Time:</strong> {appointmentDate:dddd, MMMM dd, yyyy 'at' hh:mm tt}</p>
        </div>
        <p><strong>Please remember to:</strong></p>
        <ul>
            <li>Arrive 10 minutes early</li>
            <li>Bring your ID and insurance card</li>
            <li>Bring any relevant medical documents</li>
        </ul>
        <p>If you need to reschedule or cancel, please contact us as soon as possible.</p>
    </div>
    <div class=""footer"">
        <p>&copy; 2026 Hospital Appointment System. All rights reserved.</p>
    </div>
</body>
</html>";
    }

    public static string GetAppointmentCancellationTemplate(
        string patientName,
        string doctorName,
        DateTime appointmentDate)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #F44336;
            color: white;
            padding: 20px;
            text-align: center;
            border-radius: 5px 5px 0 0;
        }}
        .content {{
            background-color: #f9f9f9;
            padding: 30px;
            border: 1px solid #ddd;
            border-radius: 0 0 5px 5px;
        }}
        .cancellation-box {{
            background-color: #FFEBEE;
            border-left: 4px solid #F44336;
            padding: 20px;
            margin: 20px 0;
        }}
        .button {{
            display: inline-block;
            padding: 12px 24px;
            background-color: #2196F3;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin: 20px 0;
        }}
    </style>
</head>
<body>
    <div class=""header"">
        <h1>Appointment Cancelled</h1>
    </div>
    <div class=""content"">
        <h2>Hello {patientName},</h2>
        <p>Your appointment has been cancelled.</p>
        <div class=""cancellation-box"">
            <p><strong>Doctor:</strong> Dr. {doctorName}</p>
            <p><strong>Original Date & Time:</strong> {appointmentDate:dddd, MMMM dd, yyyy 'at' hh:mm tt}</p>
        </div>
        <p>If you would like to book a new appointment, please visit our website:</p>
        <a href=""http://localhost:4200/patient/dashboard"" class=""button"">Book New Appointment</a>
        <p>If you have any questions or concerns, please contact our support team.</p>
    </div>
    <div class=""footer"">
        <p>&copy; 2026 Hospital Appointment System. All rights reserved.</p>
    </div>
</body>
</html>";
    }
}
