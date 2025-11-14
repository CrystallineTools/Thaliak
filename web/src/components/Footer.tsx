import { useEffect, useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCodeCommit } from '@fortawesome/free-solid-svg-icons';
import { getStatus, type StatusResponse } from '../api/v2client';

export default function Footer() {
  const [apiStatus, setApiStatus] = useState<StatusResponse | null>(null);

  useEffect(() => {
    getStatus()
      .then(data => setApiStatus(data))
      .catch(err => console.error('Failed to fetch API status:', err));
  }, []);

  return (
    <footer className='mt-12 py-8 border-t border-gray-200'>
      <div className='container mx-auto'>
        <div className='flex flex-wrap flex-col sm:flex-row gap-4 items-start sm:items-center justify-between text-sm text-gray-600'>
          <div className='grow'>
            <p className='mb-1'>FINAL FANTASY is a registered trademark of Square Enix Holdings Co., Ltd.</p>
            <p className='mb-1'>FINAL FANTASY XIV © 2010-2025 SQUARE ENIX CO., LTD. All Rights Reserved.</p>
            <p>We are not affiliated with SQUARE ENIX CO., LTD. in any way.</p>
          </div>
          <div className='flex flex-col gap-0.5'>
            {import.meta.env.VITE_GIT_SHA && (
              <div className='flex items-center gap-1 bg-gray-100 px-3 rounded-lg'>
                <FontAwesomeIcon icon={faCodeCommit} className='text-gray-500' />
                <span className='text-gray-600'>
                  web{' '}
                  <a
                    className='font-mono text-primary-600 hover:text-primary-700 font-medium'
                    href={`https://github.com/CrystallineTools/Thaliak/commit/${import.meta.env.VITE_GIT_SHA.replace(/-dirty$/, '')}`}
                    target='_blank'
                    rel='noopener noreferrer'>
                    {import.meta.env.VITE_GIT_SHA}
                  </a>
                </span>
              </div>
            )}
            {apiStatus?.components?.map(component => (
              <div key={component.component} className='flex items-center gap-1 bg-gray-100 px-3 rounded-lg'>
                <FontAwesomeIcon icon={faCodeCommit} className='text-gray-500' />
                <span className='text-gray-600'>
                  {component.component}{' '}
                  <a
                    className='font-mono text-primary-600 hover:text-primary-700 font-medium'
                    href={`https://github.com/CrystallineTools/Thaliak/commit/${component.commit.replace(/-dirty$/, '')}`}
                    target='_blank'
                    rel='noopener noreferrer'>
                    {component.commit}
                  </a>
                </span>
              </div>
            ))}
          </div>
        </div>
      </div>
    </footer>
  );
}
