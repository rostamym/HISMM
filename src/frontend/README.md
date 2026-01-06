# Hospital Appointment System - Frontend

Angular 17+ frontend application for the Hospital Appointment Management System.

## Technology Stack

- **Framework**: Angular 17+
- **UI Library**: Angular Material
- **Language**: TypeScript 5.4+
- **Styling**: SCSS
- **State Management**: RxJS (Services)
- **HTTP Client**: Angular HttpClient with Interceptors

## Project Structure

```
src/
├── app/
│   ├── core/                    # Core module (singleton services, guards, interceptors)
│   │   ├── guards/              # Auth guard, Role guard
│   │   ├── interceptors/        # Auth, Error, Loading interceptors
│   │   ├── services/            # Auth, Token, Notification services
│   │   ├── models/              # Core models (User, ApiResponse, Pagination)
│   │   └── constants/           # API endpoints, constants
│   │
│   ├── features/                # Feature modules (lazy loaded)
│   │   ├── authentication/      # Login, Register, Forgot Password
│   │   ├── patient/             # Patient dashboard and features
│   │   ├── doctor/              # Doctor dashboard and features
│   │   └── admin/               # Admin dashboard and features
│   │
│   ├── shared/                  # Shared module (components, directives, pipes)
│   │
│   ├── app-routing.module.ts    # Main routing configuration
│   ├── app.component.ts         # Root component
│   └── app.module.ts            # Root module
│
├── assets/                      # Static assets
├── environments/                # Environment configurations
└── styles.scss                  # Global styles
```

## Getting Started

### Prerequisites

- Node.js (v18+)
- npm or yarn
- Angular CLI (`npm install -g @angular/cli`)

### Installation

```bash
# Navigate to frontend directory
cd src/frontend

# Install dependencies
npm install
```

### Development Server

```bash
# Start development server
npm start

# Or with Angular CLI
ng serve
```

Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

### Build

```bash
# Build for production
npm run build

# Or with Angular CLI
ng build --configuration production
```

The build artifacts will be stored in the `dist/` directory.

### Running Tests

```bash
# Run unit tests
npm test

# Run tests with coverage
ng test --code-coverage
```

## Features Implemented

### Core Features
- ✅ Angular 17+ setup with TypeScript
- ✅ Angular Material UI components
- ✅ JWT authentication with interceptors
- ✅ Route guards (Auth, Role-based)
- ✅ HTTP interceptors (Auth, Error, Loading)
- ✅ Centralized error handling
- ✅ Toast notifications
- ✅ Path aliases (@core, @features, @shared, @environments)

### Authentication Module
- ✅ Login component with form validation
- ✅ Register component with role selection
- ✅ Forgot password component
- ✅ JWT token management
- ✅ Token refresh logic
- ✅ Role-based navigation

### Feature Modules
- ✅ Patient module with dashboard
- ✅ Doctor module with dashboard
- ✅ Admin module with dashboard
- ✅ Lazy-loaded feature modules
- ✅ Role-based route protection

## TODO - Next Implementation Steps

### 1. Patient Features
- [ ] Doctor search with filters
- [ ] Doctor profile view
- [ ] Appointment booking component
- [ ] Available time slots picker
- [ ] Appointment list (upcoming/past)
- [ ] Appointment details
- [ ] Cancel/reschedule appointment
- [ ] Patient profile management

### 2. Doctor Features
- [ ] Today's schedule view
- [ ] Weekly/monthly calendar view
- [ ] Appointment management
- [ ] Set availability (weekly schedule)
- [ ] Block specific dates
- [ ] Patient appointment history
- [ ] Doctor profile management

### 3. Admin Features
- [ ] System dashboard with analytics
- [ ] User management (list, activate, deactivate)
- [ ] Doctor management
- [ ] Appointment overview
- [ ] System statistics
- [ ] Reports generation

### 4. Shared Components
- [ ] Header with navigation
- [ ] Sidebar menu
- [ ] Loading spinner overlay
- [ ] Confirmation dialog
- [ ] Date picker wrapper
- [ ] Pagination component
- [ ] Search bar component
- [ ] Filter components

### 5. Services
- [ ] Doctor service (search, get, availability)
- [ ] Appointment service (CRUD operations)
- [ ] Patient service
- [ ] Admin service
- [ ] Availability service

### 6. Models
- [ ] Doctor model
- [ ] Appointment model
- [ ] Availability model
- [ ] Specialty model

### 7. Testing
- [ ] Unit tests for services
- [ ] Unit tests for components
- [ ] Integration tests
- [ ] E2E tests with Cypress

## API Configuration

The frontend communicates with the backend API. Update the API URL in the environment files:

**Development** (`src/environments/environment.ts`):
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api'
};
```

**Production** (`src/environments/environment.prod.ts`):
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.hospital-appointment.com/api'
};
```

## Architecture Decisions

### Why Angular Material?
- Well-maintained by Google
- Comprehensive component library
- Built-in accessibility (a11y)
- Consistent design system
- Excellent documentation

### Why Service-based State Management?
- Simpler than NgRx for this scale
- RxJS provides powerful state management
- BehaviorSubjects for reactive state
- Easy to upgrade to NgRx if needed

### Path Aliases
Configured in `tsconfig.json` for cleaner imports:
- `@core/*` → `src/app/core/*`
- `@features/*` → `src/app/features/*`
- `@shared/*` → `src/app/shared/*`
- `@environments/*` → `src/environments/*`

## Contributing

1. Create a feature branch
2. Implement changes with tests
3. Follow Angular style guide
4. Update documentation
5. Submit pull request

## License

Private - Hospital Appointment Management System
