import { ListGroup } from 'react-bootstrap';
import RepositoryListItem from '../components/RepositoryListItem';
import { useRecoilValue } from 'recoil';
import { LAST_UPDATED, LATEST_VERSIONS, REPOSITORIES } from '../store';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTriangleExclamation } from '@fortawesome/free-solid-svg-icons';
import { discordLink } from '../constants';

export default function HomePage() {
  const repositories = useRecoilValue(REPOSITORIES);
  const versions = useRecoilValue(LATEST_VERSIONS);
  const lastUpdated = useRecoilValue(LAST_UPDATED);

  return (
    <div>
      <div className='alert alert-warning mb-3' role='alert'>
        <div className='alert-warning'>
          <h5 className='card-title'>
            <FontAwesomeIcon icon={faTriangleExclamation} className='me-2' />
            REST API Deprecation & Removal Schedule
          </h5>
          <p className='card-text'>
            Thaliak's <a href='/graphql/'>GraphQL API</a> entered alpha state on August 14, 2022.<br />
            The GraphQL API is slated to reach feature parity with the REST API by the end of August (but likely
            sooner).<br />
            The legacy REST API was deprecated on the same date. No new features will be added to the REST API.<br />
            It is our intent to swiftly replace the REST API once the GraphQL API reaches feature parity.<br />
            <br />
            Barring the unexpected, <b>the REST API will be discontinued after October 15, 2022</b>.<br />
            Please migrate all applications to the GraphQL API before this date.<br />
            <br />
            For questions or support, <a href={discordLink}>join the Thaliak Discord server</a>.
          </p>
        </div>
      </div>

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