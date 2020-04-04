import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { RaspberryPiService } from '../shared/services/raspberry-pi.service';
import { BsModalService } from 'ngx-bootstrap';
import { IRaspberryPi} from '../shared/interfaces/raspberry-pi';
import { AuthService } from '../shared/services/auth.service';
import { take } from 'rxjs/operators';
import { RealTimeModalComponent } from './modal/real-time-modal.component';

@Component({
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  raspberryPi: IRaspberryPi;
  errorMessage: string;

  constructor(private _route: ActivatedRoute,
    private router: Router,
    private modalService: BsModalService,
    private raspberryPiService: RaspberryPiService,
    private authService: AuthService) { }

  ngOnInit() {
    this.raspberryPi = this._route.snapshot.data['raspberryPi'];
  }

  openModal() {
    this.modalService.show(
      RealTimeModalComponent,
      {
        class: 'modal-xl'
      });
  }
  
  shutdown() {
    this.raspberryPiService.shutdownRaspberryPi()
      .pipe(take(1))
      .subscribe(
      result => {
        if (result) {
          alert('Shutting down...');
          this.logout();
        }
        else {
          alert('Error');
        }
      },
      error => this.errorMessage = <any>error
    );
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  killProcess(processId: number) {
    this.raspberryPiService.killProcess(processId)
      .pipe(take(1))
      .subscribe(
      result => {
        if (result) {
          alert(`Process #${processId} killed`);
        }
        else {
          alert('Error');
        }
      },
      error => this.errorMessage = <any>error
    );
  }

}
