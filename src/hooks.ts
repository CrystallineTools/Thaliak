import { Application } from './application';
import { useLocation } from 'react-router-dom';

export function useCurrentApplication(): Application {
  const location = useLocation();

  if (location.pathname.startsWith('/thaliak')) {
    return Application.Thaliak;
  } else if (location.pathname.startsWith('/wiki')) {
    return Application.Wiki;
  } else {
    return Application.Ewer;
  }
}
