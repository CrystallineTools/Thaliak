import Version from '../api/types/version';
import TimeAgo from 'timeago-react';

export default function VersionDetail({ version, latest }: { version: Version, latest: boolean }) {
  return (
    <div className='relative block py-4 px-5 border-b border-gray-200 last:border-b-0 transition-colors hover:bg-gray-50'>
      <div className='flex flex-wrap flex-col sm:flex-row gap-4'>
        <div className='relative grow max-w-full flex-1'>
          <div className='flex items-center gap-2 flex-wrap'>
            <span className={`font-mono font-semibold text-xl ${latest ? 'text-primary-700' : !version.isActive ? 'text-gray-400 line-through' : 'text-gray-900'}`}>
              {version.versionString}
            </span>

            <span className='inline-flex items-center gap-1 text-xs px-2 py-1 bg-gray-100 text-gray-600 rounded-md'>
              <svg className='w-3 h-3' fill='currentColor' viewBox='0 0 20 20'>
                <path d='M7 3a1 1 0 000 2h6a1 1 0 100-2H7zM4 7a1 1 0 011-1h10a1 1 0 110 2H5a1 1 0 01-1-1zM2 11a2 2 0 012-2h12a2 2 0 012 2v4a2 2 0 01-2 2H4a2 2 0 01-2-2v-4z' />
              </svg>
              {version.patches?.length} patch{version.patches?.length === 1 ? '' : 'es'}
            </span>

            {!version.isActive && (
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

          {(version.prerequisiteVersions?.length ?? 0) > 0 && (
            <div className='mt-2 text-sm text-gray-600 bg-blue-50 px-3 py-2 rounded-md inline-block'>
              <span className='font-medium text-blue-700'>Requires</span>
              {version.prerequisiteVersions!.length > 1 && ' one of'}:
              {' '}
              <span className='font-mono text-xs'>
                {Array.from(version.prerequisiteVersions!.map(v => v.versionString)).join(', ')}
              </span>
            </div>
          )}
        </div>

        <div className='shrink-0 sm:text-end'>
          <div className='text-xs text-gray-600 space-y-1'>
            <div className='flex items-center justify-end gap-1.5'>
              <span className='text-gray-500'>First seen:</span>
              <span className='font-medium'>
                {version.firstOffered ? <TimeAgo datetime={version.firstOffered} /> : 'unknown'}
              </span>
            </div>
            <div className='flex items-center justify-end gap-1.5'>
              <span className='text-gray-500'>Last seen:</span>
              <span className='font-medium'>
                {version.lastOffered ? <TimeAgo datetime={version.lastOffered} /> : 'unknown'}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}