export interface IWebSocketData {
  type: WebSocketDataType;
  payload: string;
}

export enum WebSocketDataType {
  TOKEN = 'Token',
  COMMAND = 'Command'
}
