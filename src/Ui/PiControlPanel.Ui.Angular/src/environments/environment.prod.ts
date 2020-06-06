import { version } from '../../package.json';

export const environment = {
  production: true,
  version: version,
  graphqlEndpoint: window.location.host
};
