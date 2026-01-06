import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/auth/login',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/authentication/authentication.module').then(
        m => m.AuthenticationModule
      )
  },
  {
    path: 'patient',
    loadChildren: () =>
      import('./features/patient/patient.module').then(m => m.PatientModule)
  },
  {
    path: 'doctor',
    loadChildren: () =>
      import('./features/doctor/doctor.module').then(m => m.DoctorModule)
  },
  {
    path: 'admin',
    loadChildren: () =>
      import('./features/admin/admin.module').then(m => m.AdminModule)
  },
  {
    path: '**',
    redirectTo: '/auth/login'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
