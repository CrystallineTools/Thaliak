import { ReactElement } from 'react';
import { Outlet } from 'react-router-dom';

export default function MainContainer({ nav }: { nav?: ReactElement }): ReactElement {
  return (
    <div className='container mx-auto mt-16 flex'>
      {nav}

      <div className='px-1 sm:px-4 w-full'>
        <Outlet />
      </div>
    </div>
  );
}