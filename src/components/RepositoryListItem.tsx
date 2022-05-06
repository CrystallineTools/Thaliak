import Repository from '../api/types/repository';
import { ListGroup, Spinner } from 'react-bootstrap';
import Version from '../api/types/version';
import { useEffect, useState } from 'react';
import RepositoryListItemVersion from './RepositoryListItemVersion';

export default function RepositoryListItem({ repo, versions }: { repo: Repository, versions: Version[] }) {
  const [loading, setLoading] = useState<boolean>(true);
  const [version, setVersion] = useState<Version>();

  useEffect(() => {
    for (const version of versions) {
      if (version.repository.slug === repo.slug) {
        setVersion(version);
        break;
      }
    }

    setLoading(false);
  }, [versions, repo]);

  return (
    <ListGroup.Item>
      <div className='row'>
        <div className='col'>
          <strong>{repo.name}</strong> ({repo.slug})
          <br />
          {repo.description}
        </div>
        <div className='col-3 text-end'>
          {loading ? <Spinner animation='border' /> : <RepositoryListItemVersion version={version} />}
        </div>
      </div>
    </ListGroup.Item>
  );
}