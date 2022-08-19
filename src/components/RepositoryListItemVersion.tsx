import TimeAgo from 'timeago-react';
import { LatestVersion } from '../api/types/repository';

export default function RepositoryListItemVersion({ latestVersion }: { latestVersion: LatestVersion | undefined | null }) {
  if (!latestVersion) {
    return (
      <div className='text-muted'>
        <span>Unknown</span>
      </div>
    );
  }

  return (
    <span>
      <strong>{latestVersion.versionString}</strong>
      <div className='relative grow max-w-full text-gray-600'>
        First seen: {latestVersion.firstOffered ? <TimeAgo datetime={latestVersion.firstOffered} /> : 'never'}<br />
        Last seen: {latestVersion.lastOffered ? <TimeAgo datetime={latestVersion.lastOffered} /> : 'never'}
      </div>
    </span>
  );
}