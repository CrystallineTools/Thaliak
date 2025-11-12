import TimeAgo from 'timeago-react';
import { LatestPatchInfo } from '../api/v2client';

export default function RepositoryDetailVersion({ latestPatch }: { latestPatch: LatestPatchInfo | undefined | null }) {
  if (!latestPatch) {
    return (
      <div className='text-gray-500'>
        <span className='text-sm italic'>Unknown</span>
      </div>
    );
  }

  return (
    <div className='inline-flex flex-col items-end gap-1'>
      <span className='font-mono font-semibold text-lg text-primary-600 bg-primary-50 px-3 py-1 rounded-md'>
        {latestPatch.version_string}
      </span>
      <div className='text-xs text-gray-600 space-y-0.5'>
        <div className='flex items-center justify-end gap-1.5'>
          <span className='text-gray-500'>First offered:</span>
          <span className='font-medium'>{latestPatch.first_offered ? <TimeAgo datetime={latestPatch.first_offered} /> : 'never'}</span>
        </div>
        <div className='flex items-center justify-end gap-1.5'>
          <span className='text-gray-500'>Last offered:</span>
          <span className='font-medium'>{latestPatch.last_offered ? <TimeAgo datetime={latestPatch.last_offered} /> : 'never'}</span>
        </div>
      </div>
    </div>
  );
}
