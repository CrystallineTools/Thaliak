import { Patch } from '../api/v2client';
import VersionDetail from './VersionDetail';

export interface VersionListItemArgs {
  repoName: string;
  patch: Patch;
  latest: boolean;
}

export default function VersionListItem({ patch, latest }: VersionListItemArgs) {
  return (
    <div>
      <VersionDetail patch={patch} latest={latest} />
    </div>
  );
}
