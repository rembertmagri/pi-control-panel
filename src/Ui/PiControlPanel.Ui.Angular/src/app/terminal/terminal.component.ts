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

  ngOnInit() {
    this.terminal = new Terminal({
      cursorBlink: true,
      cursorStyle: "block"
    });
    this.fitAddon = new FitAddon();
    this.webSocket = new WebSocket(`ws://${environment.graphqlEndpoint}/shell`);
  }

  ngAfterViewInit() {
    this.terminal.loadAddon(this.fitAddon);
    this.terminal.open(this.terminalDiv.nativeElement);
    this.fitAddon.fit();
    this.terminal.focus();

    this.webSocket.onopen = (event: Event) => {
      console.log(JSON.stringify(event));
      const token = localStorage.getItem('jwt');
      this.webSocket.send(JSON.stringify(
        { type: WebSocketDataType.TOKEN, payload: token } as IWebSocketData
      ));
      this.webSocket.send(JSON.stringify(
        { type: WebSocketDataType.DIMENSIONS, payload: `${this.terminal.rows}|${this.terminal.cols}` } as IWebSocketData
      ));
    };

    // Receive data from socket
    this.webSocket.onmessage = (messageEvent: MessageEvent) => {
      this.terminal.write(messageEvent.data);
    };

    this.terminal.onKey((e: { key: string, domEvent: KeyboardEvent }) => {
      this.webSocket.send(e.key);
    });
  }

  ngOnDestroy() {
    this.webSocket.close();
  }

}
