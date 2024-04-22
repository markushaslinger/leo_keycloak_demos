import { ChangeDetectionStrategy, Component, inject, signal, WritableSignal } from "@angular/core";
import { KeycloakService } from "keycloak-angular";
import { MatButton } from "@angular/material/button";
import { createLeoUser, LeoUser, Role } from "../../core/util/leo-token";

@Component({
  selector: 'app-counter',
  standalone: true,
  imports: [
    MatButton
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './counter.component.html',
  styleUrl: './counter.component.scss'
})
export class CounterComponent {
  private readonly keycloakService: KeycloakService = inject(KeycloakService);
  private incrementAmount: number | null = null;
  public readonly count: WritableSignal<number> = signal(0);

  public async increment(): Promise<void> {
    const incrementAmount: number = await this.getIncrementAmount();
    this.count.update(value => value + incrementAmount);
  }

  private async getIncrementAmount(): Promise<number> {
    if (this.incrementAmount === null) {
      const leoUser: LeoUser = await createLeoUser(this.keycloakService);

      if (leoUser.hasRole(Role.Student)) {
        this.incrementAmount = 2;
      } else if (leoUser.hasRole(Role.Teacher)) {
        this.incrementAmount = 1;
      } else {
        this.incrementAmount = 0;
      }
    }

    return this.incrementAmount;
  }
}
