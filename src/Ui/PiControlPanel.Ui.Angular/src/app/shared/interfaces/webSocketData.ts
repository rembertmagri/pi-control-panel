export interface IWebSocketData {
  type: WebSocketDataType;
  payload: string;
}

export enum WebSocketDataType {
  TOKEN = 'Token',
  STANDARD_INPUT = 'StandardInput',
  STANDARD_OUTPUT = 'StandardOutput',
  STANDARD_ERROR = 'StandardError'
}
