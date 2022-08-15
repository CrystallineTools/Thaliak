import Repository from '../api/types/repository';
import { Badge, ListGroup, Spinner } from 'react-bootstrap';
import Version from '../api/types/version';
import { useEffect, useState } from 'react';
import RepositoryListItemVersion from './RepositoryListItemVersion';
import { Link } from 'react-router-dom';

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
          <Link className='font-monospace fw-bold' to={`/repository/${repo.slug}`}>{repo.slug}</Link>
          <div className='btn-group btn-group-sm ms-2'>
            <span className='btn btn-outline-secondary disabled'>
              <Badge pill bg='warning'>
                  Deprecated
              </Badge>
              {' '}
              REST API
            </span>
            <a className='btn btn-warning'
               href={`https://thaliak.xiv.dev/api/versions/${repo.slug}/${version?.version}`}>
              This Version
            </a>
            <a className='btn btn-success' href={`https://thaliak.xiv.dev/api/versions/${repo.slug}/latest`}>
              Latest Version
            </a>
            <a className='btn btn-primary' href={`https://thaliak.xiv.dev/api/versions/${repo.slug}`}>
              All Versions
            </a>
          </div>
          <br />
          {repo.description}
          <br />
          <span className='text-muted small'>{repo.name}</span>
        </div>
        <div className='col-3 text-end'>
          {loading ? <Spinner animation='border' /> : <RepositoryListItemVersion version={version} />}
        </div>
      </div>
    </ListGroup.Item>
  );
}