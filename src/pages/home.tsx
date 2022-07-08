import { ListGroup } from 'react-bootstrap';
import RepositoryListItem from '../components/RepositoryListItem';
import { useRecoilValue } from 'recoil';
import { LAST_UPDATED, LATEST_VERSIONS, REPOSITORIES } from '../store';

export default function HomePage() {
  const repositories = useRecoilValue(REPOSITORIES);
  const versions = useRecoilValue(LATEST_VERSIONS);
  const lastUpdated = useRecoilValue(LAST_UPDATED);

  return (
    <div>
      <div className='row mb-1'>
        <div className='col'>
          <h2>Game Version Repositories</h2>
        </div>
        <div className='col-3 text-end text-muted small my-auto'>
          Automatically updates every 60 seconds.<br />
          Last updated: {lastUpdated?.toLocaleString()}
        </div>
      </div>

      <ListGroup>
        {repositories.map((repo) => <RepositoryListItem repo={repo} versions={versions} key={repo.slug} />)}
      </ListGroup>
    </div>
  );
}