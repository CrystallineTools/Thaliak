import Version from '../api/types/version';
import TimeAgo from 'timeago-react';
import { useEffect, useState } from 'react';

export default function RepositoryListItemVersion({ version }: { version: Version | undefined | null }) {
  const [firstSeen, setFirstSeen] = useState<string>();
  const [lastSeen, setLastSeen] = useState<string>();

  useEffect(() => {
    if (!version || version.patches.length < 1) {
      return;
    }

    setFirstSeen(version.patches[0].firstSeen);
    setLastSeen(version.patches[0].lastSeen);
  }, [version]);

  if (!version) {
    return (
      <div className='text-muted'>
        <span>Unknown</span>
      </div>
    );
  }

  return (
    <span>
      <strong>{version.version}</strong>
      <div className='text-muted small'>
        First seen: {firstSeen ? <TimeAgo datetime={firstSeen} /> : 'never'}<br />
        Last seen: {lastSeen ? <TimeAgo datetime={lastSeen} /> : 'never'}
      </div>
    </span>
  );
}