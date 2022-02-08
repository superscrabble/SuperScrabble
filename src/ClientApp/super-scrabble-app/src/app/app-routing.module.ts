import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GameSummaryComponent } from './pages/game-summary/game-summary.component';
import { GameComponent } from './pages/game/game.component';
import { HomeComponent } from './pages/home/home.component';
import { LoginFormComponent } from './pages/login-form/login-form.component';
import { PartyPageComponent } from './pages/party-page/party-page.component';
import { RegisterFormComponent } from './pages/register-form/register-form.component';

const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'home', component: HomeComponent, pathMatch: 'full' },
  { path: 'register', component: RegisterFormComponent, pathMatch: 'full' },
  { path: 'login', component: LoginFormComponent, pathMatch: 'full' },
  { path: 'games/:id', component: GameComponent, pathMatch: 'full' },
  { path: 'games/:id/summary', component: GameSummaryComponent, pathMatch: 'full'},
  { path: 'party/:id', component: PartyPageComponent, pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
