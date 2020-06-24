import { Component, OnInit, ViewEncapsulation, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { Terminal } from 'xterm';

@Component({
  encapsulation: ViewEncapsulation.None,
  templateUrl: './terminal.component.html',
  styleUrls: ['./terminal.component.css']
})
export class TerminalComponent implements OnInit, AfterViewInit {
  @ViewChild('myTerminal') terminalDiv: ElementRef;
  term: Terminal;

  constructor() { }

  ngOnInit() {
    this.term = new Terminal({
      cursorBlink: true,
      cursorStyle: "block"
    });
  }

  ngAfterViewInit() {
    let currentLine = "";
    this.term.open(this.terminalDiv.nativeElement);
    this.term.write('$ ');

    // Receive data from socket
    // ws.onmessage = (msg) => {
    //   this.term.write("\r\n" + JSON.parse(msg.data).data);
    //   currentLine = "";
    // };

    this.term.onKey((e: { key: string, domEvent: KeyboardEvent }) => {
      const ev = e.domEvent;
      const printable = !ev.altKey && !ev.ctrlKey && !ev.metaKey;

      if (ev.keyCode === 13) { //Enter
        this.term.write('\r\n$ ');
        this.prompt(currentLine);
        currentLine = "";
      } else if (ev.keyCode === 8) { // Backspace
        // Do not delete the prompt
        if (currentLine) {
          currentLine = currentLine.slice(0, currentLine.length - 1);
          this.term.write("\b \b");
        }
        // if (this.term._core.buffer.x > 2) {
        //   this.term.write('\b \b');
        // }
      } else if (printable) {
        currentLine += e.key;
        this.term.write(e.key);
      }
    });
  }

  prompt(currentLine: string) {
    if (currentLine) {
      const data = { method: "command", command: currentLine };
      //ws.send(JSON.stringify(data));
    }
  }

}
