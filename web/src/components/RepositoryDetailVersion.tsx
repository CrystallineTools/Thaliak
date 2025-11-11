import TimeAgo from 'timeago-react';
import { LatestVersion } from '../api/types/repository';

export default function RepositoryDetailVersion({ latestVersion }: { latestVersion: LatestVersion | undefined | null }) {
  if (!latestVersion) {
    return (
      <div className='text-gray-500'>
        <span className='text-sm italic'>Unknown</span>
      </div>
    );
  }

  return (
    <div className='inline-flex flex-col items-end gap-1'>
      <span className='font-mono font-semibold text-lg text-primary-600 bg-primary-50 px-3 py-1 rounded-md'>
        {latestVersion.versionString}
      </span>
      <div className='text-xs text-gray-600 space-y-0.5'>
        <div className='flex items-center justify-end gap-1.5'>
          <span className='text-gray-500'>First seen:</span>
          <span className='font-medium'>{latestVersion.firstOffered ? <TimeAgo datetime={latestVersion.firstOffered} /> : 'never'}</span>
        </div>
        <div className='flex items-center justify-end gap-1.5'>
          <span className='text-gray-500'>Last seen:</span>
          <span className='font-medium'>{latestVersion.lastOffered ? <TimeAgo datetime={latestVersion.lastOffered} /> : 'never'}</span>
        </div>
      </div>
    </div>
  );
}