import { Component, OnInit, ViewEncapsulation, AfterViewInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { Terminal } from 'xterm';
import { environment } from '@environments/environment';

@Component({
  encapsulation: ViewEncapsulation.None,
  templateUrl: './terminal.component.html',
  styleUrls: ['./terminal.component.css']
})
export class TerminalComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('myTerminal') terminalDiv: ElementRef;
  terminal: Terminal;
  webSocket: WebSocket;

  constructor() { }

  ngOnInit() {
    this.terminal = new Terminal({
      cursorBlink: true,
      cursorStyle: "block"
    });
    this.webSocket = new WebSocket(`ws://${environment.graphqlEndpoint}/shell`);
  }

  ngAfterViewInit() {
    let currentLine = "";
    this.terminal.open(this.terminalDiv.nativeElement);
    this.terminal.write('$ ');
    this.terminal.focus();

    // Receive data from socket
    this.webSocket.onmessage = (messageEvent: MessageEvent) => {
      const data = JSON.parse(messageEvent.data);
      this.terminal.write(`${data.payload}\r\n$ `);
    };

    this.terminal.onKey((e: { key: string, domEvent: KeyboardEvent }) => {
      const ev = e.domEvent;
      const printable = !ev.altKey && !ev.ctrlKey && !ev.metaKey;

      if (ev.keyCode === 13) { //Enter
        this.terminal.write('\r\n');
        if (currentLine) {
          const data = { type: "command", payload: currentLine };
          this.webSocket.send(JSON.stringify(data));
          currentLine = "";
        }
        else {
          this.terminal.write('$ ');
        }
      } else if (ev.keyCode === 8) { // Backspace
        // Do not delete the prompt
        if (currentLine) {
          currentLine = currentLine.slice(0, currentLine.length - 1);
          this.terminal.write("\b \b");
        }
      } else if (printable) {
        currentLine += e.key;
        this.terminal.write(e.key);
      }
    });
  }

  ngOnDestroy() {
    this.webSocket.close();
  }

}
