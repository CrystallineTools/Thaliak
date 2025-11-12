import RepositoryDetailVersion from './RepositoryDetailVersion';
import { Link } from 'react-router';
import { Repository } from '../api/v2client';

export interface RepositoryDetailProps {
  repo: Repository;
  linkName?: boolean;
  showLatestVersionInfo?: boolean;
}

export default function RepositoryDetail({
                                             repo,
                                             linkName,
                                             showLatestVersionInfo
                                           }: RepositoryDetailProps) {
  return (
    <div className='relative block py-4 px-5 border-b border-gray-200 last:border-b-0 transition-colors hover:bg-gray-50'>
      <div className='flex flex-wrap flex-col sm:flex-row gap-3'>
        <div className='relative grow max-w-full flex-1'>
          {linkName ? (
            <Link className='font-mono font-semibold text-primary-700 hover:text-primary-800 transition-colors inline-flex items-center gap-2 group'
                  to={`/repository/${repo.slug}`}>
              {repo.slug}
              <svg className='w-4 h-4 opacity-0 group-hover:opacity-100 transition-opacity' fill='none' stroke='currentColor' viewBox='0 0 24 24'>
                <path strokeLinecap='round' strokeLinejoin='round' strokeWidth={2} d='M9 5l7 7-7 7' />
              </svg>
            </Link>
          ) : (
            <span className='font-mono font-semibold text-gray-900'>{repo.slug}</span>
          )}
          <p className='mt-1 text-sm text-gray-700'>{repo.description}</p>
          <p className='mt-0.5 text-xs text-gray-500'>{repo.name}</p>
        </div>
        {showLatestVersionInfo && repo.latest_patch && (
          <div className='mt-2 sm:w-2/5 sm:mt-0 sm:text-end text-nowrap shrink-0'>
            <RepositoryDetailVersion latestPatch={repo.latest_patch} />
          </div>
        )}
      </div>
    </div>
  );
}
