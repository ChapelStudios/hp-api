import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import CharacterSheet from '../characterSheet/CharacterSheet'
import ErrorPage from './ErrorPage';

const DcmRouter = () => {
  return (
    <RouterProvider router={createBrowserRouter([
      {
        path: '/',
        element: <CharacterSheet />,
        errorElement: <ErrorPage />,
        index: true,
      },
    ])} />
  );
};

export default DcmRouter;
