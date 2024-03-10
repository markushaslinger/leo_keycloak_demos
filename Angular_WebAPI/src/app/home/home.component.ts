import {Component} from '@angular/core';
import {KeycloakService} from "keycloak-angular";
import {MatButton} from "@angular/material/button";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    MatButton
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  isLoggedIn = false;

  constructor(private keycloakService: KeycloakService) {
    this.isLoggedIn = this.keycloakService.isLoggedIn();
  }

  async login(): Promise<void> {
    if (this.isLoggedIn) {
      return
    }
    await this.keycloakService.login();

  }

  async logout(): Promise<void> {
    if (!this.isLoggedIn) {
      return;
    }
    await this.keycloakService.logout();
  }
}
