import './App.css';
import AppState from './context/AppContext';
import DcmRouter from './components/layout/DcmRouter';

function App() {
  return (
    <AppState>
      <DcmRouter />
    </AppState>
  );
}

export default App;