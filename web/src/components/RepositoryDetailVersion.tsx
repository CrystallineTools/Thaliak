import TimeAgo from 'timeago-react';
import { LatestVersion } from '../api/types/repository';

export default function RepositoryDetailVersion({ latestVersion }: { latestVersion: LatestVersion | undefined | null }) {
  if (!latestVersion) {
    return (
      <div className='text-muted'>
        <span>Unknown</span>
      </div>
    );
  }

  return (
    <span>
      <div className='relative grow max-w-full text-gray-600 text-right'>
        <span className='font-bold block text-md -mb-px'>{latestVersion.versionString}</span>
        <span className='text-sm'>
          First seen: {latestVersion.firstOffered ? <TimeAgo datetime={latestVersion.firstOffered} /> : 'never'}<br />
          Last seen: {latestVersion.lastOffered ? <TimeAgo datetime={latestVersion.lastOffered} /> : 'never'}
        </span>
      </div>
    </span>
  );
}