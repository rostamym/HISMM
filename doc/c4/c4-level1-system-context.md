# C4 Model - Level 1: System Context Diagram
## Hospital Appointment Management System

### Overview
The System Context diagram provides a high-level view of the Hospital Appointment Management System, showing how it fits into the world around it. It shows the system as a single box and depicts the relationships between the system and external actors (users) and external systems.

---

## System Context Diagram

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│                     Hospital Appointment Management System                   │
│                                                                              │
│              A comprehensive system for managing doctor appointments,        │
│              patient records, and healthcare facility operations             │
│                                                                              │
│              Technology: .NET 8, Angular 17+, SQL Server                     │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
                                       ▲
                                       │
                    ┌──────────────────┼──────────────────┐
                    │                  │                  │
                    │                  │                  │
              ┌─────▼─────┐      ┌────▼────┐      ┌──────▼──────┐
              │           │      │         │      │             │
              │  Patient  │      │ Doctor  │      │ Admin Staff │
              │           │      │         │      │             │
              │  (Person) │      │(Person) │      │  (Person)   │
              └───────────┘      └─────────┘      └─────────────┘
                    │                  │                  │
                    │                  │                  │
                    └──────────────────┼──────────────────┘
                                       │
                                       ▼
        ┌────────────────────────────────────────────────────────────────┐
        │                                                                │
        │          Hospital Appointment Management System                │
        │                                                                │
        └────────────────────────────────────────────────────────────────┘
                                       │
                    ┌──────────────────┼──────────────────┐
                    │                  │                  │
              ┌─────▼──────┐     ┌────▼─────┐      ┌─────▼────────┐
              │            │     │          │      │              │
              │   Email    │     │   SMS    │      │   Payment    │
              │  Service   │     │ Gateway  │      │   Gateway    │
              │            │     │          │      │              │
              │(Software)  │     │(Software)│      │  (Software)  │
              └────────────┘     └──────────┘      └──────────────┘
              (SendGrid)         (Twilio)         (Stripe/PayPal)
```

---

## System Description

### Hospital Appointment Management System
**Type**: Software System

**Description**:
A comprehensive web-based platform that enables patients to book appointments with doctors, allows doctors to manage their schedules and appointments, and provides administrators with tools to oversee the entire system. The system handles user authentication, appointment scheduling, availability management, notifications, and reporting.

**Key Responsibilities**:
- User authentication and authorization
- Doctor profile management
- Appointment booking and scheduling
- Availability management
- Automated notifications and reminders
- Medical records management
- Analytics and reporting
- Payment processing

**Technology Stack**:
- Backend: ASP.NET Core 8.0 Web API
- Frontend: Angular 17+
- Database: SQL Server
- Architecture: Clean Architecture with CQRS pattern

---

## External Actors (Users)

### 1. Patient
**Type**: Person

**Description**:
End-users who need medical consultation and use the system to find doctors, book appointments, and manage their healthcare schedule.

**Key Interactions**:
- Register and create patient account
- Search for doctors by specialty, location, and availability
- View doctor profiles and reviews
- Book appointments with available time slots
- View upcoming and past appointments
- Reschedule or cancel appointments
- Receive appointment confirmations and reminders
- Update personal profile and medical information
- Make payments for consultations

**Relationship with System**:
Uses the web application via browser to access healthcare services

---

### 2. Doctor
**Type**: Person

**Description**:
Medical professionals who provide healthcare services and use the system to manage their schedule, appointments, and patient interactions.

**Key Interactions**:
- Register and create doctor profile
- Set availability schedule (weekly recurring slots)
- Block specific dates for holidays/time off
- View daily/weekly appointment schedule
- View patient information for appointments
- Mark appointments as completed
- Add notes to patient appointments
- Receive notifications for new bookings
- View appointment statistics and analytics
- Update professional profile and credentials

**Relationship with System**:
Uses the web application via browser to manage appointments and schedule

---

### 3. Administrator / Admin Staff
**Type**: Person

**Description**:
Healthcare facility staff members who oversee system operations, manage users, and ensure smooth functioning of the appointment system.

**Key Interactions**:
- Access admin dashboard with system overview
- Manage user accounts (activate/deactivate)
- Manage doctor profiles and credentials
- View all appointments across the system
- Generate reports and analytics
- Configure system settings
- Handle disputes and issues
- Monitor system health and performance
- Manage specialties and categories
- Oversee payment transactions

**Relationship with System**:
Uses the admin panel via browser to oversee and manage the system

---

## External Systems

### 1. Email Service (SendGrid)
**Type**: External Software System

**Description**:
Third-party email delivery service used to send transactional emails and notifications to users.

**Key Interactions**:
- **Appointment Confirmation**: Sent when appointment is booked
- **Appointment Reminder**: Sent 24 hours and 2 hours before appointment
- **Appointment Cancellation**: Sent when appointment is cancelled or rescheduled
- **Registration Confirmation**: Sent when new user registers
- **Password Reset**: Sent when user requests password reset
- **Doctor Notifications**: Sent when new appointment is booked

**Protocol**: HTTPS (REST API)

**Relationship with System**:
Hospital system sends email requests to SendGrid API, which handles email delivery to recipients

---

### 2. SMS Gateway (Twilio)
**Type**: External Software System

**Description**:
Third-party SMS messaging service used to send text message notifications to users' mobile phones.

**Key Interactions**:
- **Appointment Reminder SMS**: Sent 24 hours before appointment
- **Urgent Notifications**: Sent for time-sensitive updates
- **Verification Codes**: Sent for two-factor authentication (optional)
- **Appointment Changes**: Sent when doctor reschedules or cancels

**Protocol**: HTTPS (REST API)

**Relationship with System**:
Hospital system sends SMS requests to Twilio API, which delivers messages to users' phone numbers

---

### 3. Payment Gateway (Stripe/PayPal)
**Type**: External Software System

**Description**:
Third-party payment processing service used to handle online payments for medical consultations and services.

**Key Interactions**:
- **Payment Processing**: Process credit card and online payments
- **Payment Confirmation**: Confirm successful transactions
- **Refund Processing**: Handle refunds for cancelled appointments
- **Payment History**: Retrieve transaction history
- **Invoice Generation**: Generate payment receipts

**Protocol**: HTTPS (REST API)

**Relationship with System**:
Hospital system integrates with payment gateway API to process financial transactions securely

---

## Key Relationships and Data Flows

### Patient → System
**Direction**: Bidirectional

**Data Flows**:
- **Inbound**: Registration data, appointment bookings, search queries, profile updates
- **Outbound**: Doctor listings, available time slots, appointment confirmations, medical records

---

### Doctor → System
**Direction**: Bidirectional

**Data Flows**:
- **Inbound**: Availability schedules, appointment notes, profile updates, date blocks
- **Outbound**: Appointment schedules, patient information, notifications, statistics

---

### Administrator → System
**Direction**: Bidirectional

**Data Flows**:
- **Inbound**: User management actions, system configurations, report parameters
- **Outbound**: System analytics, user lists, appointment data, performance metrics

---

### System → Email Service
**Direction**: Outbound (One-way)

**Data Flows**:
- **Outbound**: Email templates, recipient addresses, appointment details, notification content
- **Inbound**: Delivery status, bounce notifications (webhooks)

**Trigger**: Automated based on events (appointment booked, reminder scheduled, etc.)

---

### System → SMS Gateway
**Direction**: Outbound (One-way)

**Data Flows**:
- **Outbound**: Phone numbers, SMS message content, scheduling information
- **Inbound**: Delivery status, error reports (webhooks)

**Trigger**: Automated based on events and user preferences

---

### System → Payment Gateway
**Direction**: Bidirectional

**Data Flows**:
- **Outbound**: Payment requests, transaction amount, customer details
- **Inbound**: Payment confirmation, transaction ID, receipt data, refund confirmations

**Trigger**: User-initiated payment actions and appointment bookings

---

## Security Boundaries

### Authentication & Authorization
- All external actors (Patient, Doctor, Admin) must authenticate via JWT tokens
- Role-based access control (RBAC) enforces different permission levels
- External systems authenticate via API keys and OAuth tokens

### Data Protection
- All communications use HTTPS/TLS encryption
- Sensitive data (PHI - Personal Health Information) is encrypted at rest
- Compliance with healthcare regulations (HIPAA/GDPR)

### API Security
- Rate limiting to prevent abuse
- Input validation and sanitization
- Protection against OWASP Top 10 vulnerabilities

---

## System Boundaries

### Inside the System Boundary
- User authentication and authorization
- Appointment booking logic
- Schedule management
- Notification orchestration
- Data persistence
- Business rule enforcement
- API endpoints

### Outside the System Boundary
- Email delivery infrastructure
- SMS messaging infrastructure
- Payment processing and card data handling
- User devices (browsers, mobile devices)
- External identity providers (optional OAuth)

---

## Key Features Summary

1. **User Management**: Registration, authentication, role-based access
2. **Doctor Discovery**: Search, filter, and view doctor profiles
3. **Appointment Booking**: Real-time availability checking and booking
4. **Schedule Management**: Doctor availability configuration and management
5. **Notifications**: Automated email and SMS reminders
6. **Payments**: Secure online payment processing
7. **Admin Tools**: User management, analytics, and system configuration
8. **Reporting**: Appointment statistics and business analytics

---

## Technology Choices Rationale

### Why .NET 8 + Angular?
- **Enterprise-Grade**: Proven technologies for healthcare systems
- **Type Safety**: Strong typing in both backend (C#) and frontend (TypeScript)
- **Performance**: High-performance runtime and efficient rendering
- **Security**: Built-in security features and regular updates
- **Scalability**: Easy to scale horizontally and vertically
- **Community**: Large community and extensive third-party libraries

### Why SQL Server?
- **ACID Compliance**: Critical for healthcare data integrity
- **Relational Model**: Appropriate for appointment and user relationships
- **Mature Tooling**: Excellent management and monitoring tools
- **Integration**: Seamless integration with .NET ecosystem

---

## Future Considerations

### Potential Future Integrations
- **Electronic Health Records (EHR)**: Integration with existing EHR systems
- **Telemedicine Platform**: Video consultation capabilities
- **Lab Systems**: Integration with laboratory information systems
- **Pharmacy Systems**: E-prescription capabilities
- **Insurance Verification**: Real-time insurance eligibility checking
- **Analytics Platform**: Advanced business intelligence integration

### Scalability Considerations
- **Microservices**: Potential decomposition into microservices
- **API Gateway**: Centralized API management
- **Caching Layer**: Redis for improved performance
- **CDN**: Content delivery for static assets
- **Load Balancing**: Multiple application server instances

---

## Compliance and Regulations

### Healthcare Regulations
- **HIPAA** (Health Insurance Portability and Accountability Act) - US
- **GDPR** (General Data Protection Regulation) - EU
- **HITECH** (Health Information Technology for Economic and Clinical Health)

### Security Standards
- **OWASP Top 10**: Protection against common vulnerabilities
- **SOC 2 Type II**: Service organization control compliance
- **ISO 27001**: Information security management

---

## Deployment Context

### Hosting Environment
- **Cloud**: Azure, AWS, or on-premises data center
- **Containers**: Docker containers with Kubernetes orchestration
- **Database**: Managed SQL Server instance or self-hosted
- **CI/CD**: Automated deployment pipeline

### Availability
- **Target**: 99.9% uptime
- **Backup**: Daily automated backups
- **Disaster Recovery**: Geo-redundant backup strategy

---

## Document Metadata

- **Document Version**: 1.0
- **Last Updated**: 2026-01-06
- **Author**: System Architect
- **Status**: Draft
- **C4 Level**: 1 (System Context)
