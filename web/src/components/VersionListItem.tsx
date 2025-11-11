import Version from '../api/types/version';
import VersionDetail from './VersionDetail';

export interface VersionListItemArgs {
  repoName: string;
  version: Version;
  latest: boolean;
}

export default function VersionListItem({ repoName, version, latest }: VersionListItemArgs) {
  return (
    <div>
      <VersionDetail version={version} latest={latest} />
    </div>
  );
}