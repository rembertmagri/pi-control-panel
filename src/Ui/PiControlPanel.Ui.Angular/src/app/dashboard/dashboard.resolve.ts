import { Injectable } from "@angular/core";
import { Resolve, ActivatedRouteSnapshot } from "@angular/router";
import { RaspberryPiService } from '@services/raspberry-pi.service';
import { IRaspberryPi } from "@interfaces/raspberry-pi";

@Injectable()
export class DashboardResolve implements Resolve<IRaspberryPi> {

  constructor(protected service: RaspberryPiService) { }

  resolve(route: ActivatedRouteSnapshot) {
    return this.service.getRaspberryPi();
  }
}
