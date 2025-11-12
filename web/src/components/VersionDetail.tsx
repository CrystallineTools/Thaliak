import { Patch } from '../api/v2client';
import TimeAgo from 'timeago-react';

export default function VersionDetail({ patch, latest }: { patch: Patch, latest: boolean }) {
  return (
    <div className='relative block py-4 px-5 border-b border-gray-200 last:border-b-0 transition-colors hover:bg-gray-50'>
      <div className='flex flex-wrap flex-col sm:flex-row gap-4'>
        <div className='relative grow max-w-full flex-1'>
          <div className='flex items-center gap-2 flex-wrap'>
            <span className={`font-mono font-semibold text-xl ${latest ? 'text-primary-700' : !patch.is_active ? 'text-gray-400 line-through' : 'text-gray-900'}`}>
              {patch.version_string}
            </span>

            {!patch.is_active && (
              <span className='inline-flex items-center text-xs px-2 py-1 bg-amber-50 text-amber-700 rounded-md'>
                Superseded
              </span>
            )}

            {latest && (
              <span className='inline-flex items-center gap-1 text-xs px-2 py-1 bg-primary-50 text-primary-700 rounded-md font-medium'>
                <svg className='w-3 h-3' fill='currentColor' viewBox='0 0 20 20'>
                  <path d='M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z' />
                </svg>
                Latest
              </span>
            )}
          </div>
        </div>

        <div className='shrink-0 sm:text-end'>
          <div className='text-xs text-gray-600 space-y-1'>
            <div className='flex items-center justify-end gap-1.5'>
              <span className='text-gray-500'>First offered:</span>
              <span className='font-medium'>
                {patch.first_offered ? <TimeAgo datetime={patch.first_offered} /> : 'unknown'}
              </span>
            </div>
            <div className='flex items-center justify-end gap-1.5'>
              <span className='text-gray-500'>Last offered:</span>
              <span className='font-medium'>
                {patch.last_offered ? <TimeAgo datetime={patch.last_offered} /> : 'unknown'}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
