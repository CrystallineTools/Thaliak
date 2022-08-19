import RepositoryListItemVersion from './RepositoryListItemVersion';
import { Link } from 'react-router-dom';
import { LatestVersion, Repository } from '../api/types/repository';

function GraphQLButtons({ repo, latestVersion }: { repo: Repository, latestVersion: LatestVersion }) {
  return (
    <div className='btn-group btn-group-sm ms-2'>
      <div className='btn btn-outline-secondary disabled d-inline-block'>
        <img src='/icon-graphql.svg' alt='GraphQL icon' className='me-1' style={{ height: '1.2rem' }} />
        <span>
                GraphQL
              </span>
      </div>
      <a className='btn btn-warning'
         href={`https://thaliak.xiv.dev/api/versions/${repo.slug}/${latestVersion?.versionString}`}>
        This Version
      </a>
      <a className='btn btn-success' href={`https://thaliak.xiv.dev/api/versions/${repo.slug}/latest`}>
        Latest Version
      </a>
      <a className='btn btn-primary' href={`https://thaliak.xiv.dev/api/versions/${repo.slug}`}>
        All Versions
      </a>
    </div>
  );
}

export default function RepositoryListItem({
                                             repo,
                                             latestVersion
                                           }: { repo: Repository, latestVersion: LatestVersion }) {
  return (
    <div className='relative block py-3 px-0 border-spacing-x-0 border-gray-300 no-underline'>
      <div className='flex flex-wrap'>
        <div className='relative flex-grow max-w-full flex-1'>
          <Link className='font-mono font-bold underline decoration-2 underline-offset-2'
                to={`/repository/${repo.slug}`}>{repo.slug}</Link>
          {/* temporarily disabled while I figure out how to make this work with playground */}
          {/*<GraphQLButtons repo={repo} latestVersion={latestVersion} />*/}
          <br />
          {repo.description}
          <br />
          <span className='text-sm text-gray-600'>{repo.name}</span>
        </div>
        <div className='w-1/4 text-end'>
          <RepositoryListItemVersion latestVersion={latestVersion} />
        </div>
      </div>
    </div>
  );
}