import * as React from 'react';
import * as ReactDOM from 'react-dom';
// import App from './views/App';
import './assets/style/index.scss';
import registerServiceWorker from './registerServiceWorker';
import Upload from './components/upload';

ReactDOM.render(
  <Upload/>,
  document.getElementById('root') as HTMLElement
);
registerServiceWorker();

