import RepositoryDetailVersion from './RepositoryDetailVersion';
import { LatestVersion, Repository } from '../../api/thaliak/types/repository';
import Link from '../shared/Link';

export interface RepositoryDetailProps {
  repo: Repository;
  latestVersion?: LatestVersion;
  linkName?: boolean;
  showLatestVersionInfo?: boolean;
}

export default function RepositoryDetail({
                                             repo,
                                             latestVersion,
                                             linkName,
                                             showLatestVersionInfo
                                           }: RepositoryDetailProps) {
  return (
    <div className='relative block py-3 px-0 border-spacing-x-0 border-gray-300 no-underline'>
      <div className='flex flex-wrap flex-col sm:flex-row'>
        <div className='relative flex-grow max-w-full flex-1'>
          {linkName ? (
            <Link className='font-mono font-bold underline decoration-2 underline-offset-2'
                  href={`/thaliak/repository/${repo.slug}`}>{repo.slug}</Link>
          ) : (
            <span className='font-mono font-bold'>{repo.slug}</span>
          )}
          <br />
          {repo.description}
          <br />
          <span className='text-sm text-gray-600'>{repo.name}</span>
        </div>
        {showLatestVersionInfo && latestVersion && (
          <div className='mt-2 sm:w-2/5 sm:mt-0 sm:text-end text-nowrap'>
            <RepositoryDetailVersion latestVersion={latestVersion} />
          </div>
        )}
      </div>
    </div>
  );
}