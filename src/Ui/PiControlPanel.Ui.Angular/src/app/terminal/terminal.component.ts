import { Component, OnInit, ViewEncapsulation, AfterViewInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { Terminal } from 'xterm';
import { FitAddon } from 'xterm-addon-fit';
import { environment } from '@environments/environment';
import { IWebSocketData, WebSocketDataType } from '@interfaces/webSocketData';

@Component({
  encapsulation: ViewEncapsulation.None,
  templateUrl: './terminal.component.html',
  styleUrls: ['./terminal.component.css']
})
export class TerminalComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('myTerminal') terminalDiv: ElementRef;
  terminal: Terminal;
  fitAddon: FitAddon;
  webSocket: WebSocket;

  constructor() { }

  ngOnInit() {
    this.terminal = new Terminal({
      cursorBlink: true,
      cursorStyle: "block"
    });
    this.fitAddon = new FitAddon();
    this.webSocket = new WebSocket(`ws://${environment.graphqlEndpoint}/shell`);
    this.webSocket.onopen = (event: Event) => {
      console.log(JSON.stringify(event));
      const token = localStorage.getItem('jwt');
      const data = { type: WebSocketDataType.TOKEN, payload: token } as IWebSocketData;
      this.webSocket.send(JSON.stringify(data));
    };
  }

  ngAfterViewInit() {
    let currentLine = "";
    this.terminal.loadAddon(this.fitAddon);
    this.terminal.open(this.terminalDiv.nativeElement);
    this.fitAddon.fit();
    this.terminal.write('$ ');
    this.terminal.focus();

    // Receive data from socket
    this.webSocket.onmessage = (messageEvent: MessageEvent) => {
      const data = JSON.parse(messageEvent.data) as IWebSocketData;
      switch (data.type) {
        case WebSocketDataType.STANDARD_OUTPUT:
          this.terminal.write(`${data.payload}\r\n$ `);
          break;
        case WebSocketDataType.STANDARD_ERROR:
          this.terminal.write(`ERROR: ${data.payload}\r\n$ `);
          break;
        default:
          console.error(`Invalid data type: ${data.type} (payload was '${data.payload}')`);
          break;
      }
    };

    this.terminal.onKey((e: { key: string, domEvent: KeyboardEvent }) => {
      const ev = e.domEvent;
      const printable = !ev.altKey && !ev.ctrlKey && !ev.metaKey;

      if (ev.keyCode === 13) { //Enter
        this.terminal.write('\r\n');
        if (currentLine) {
          const data = { type: WebSocketDataType.STANDARD_INPUT, payload: currentLine } as IWebSocketData;
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
