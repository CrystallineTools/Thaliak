import { useEffect, useState } from 'react';
import Repository from '../api/types/repository';
import { ListGroup } from 'react-bootstrap';
import RepositoryListItem from '../components/RepositoryListItem';
import Version from '../api/types/version';
import Api from '../api/client';

export default function Home() {
  const [repositories, setRepositories] = useState<Repository[]>([]);
  const [versions, setVersions] = useState<Version[]>([]);
  const [lastUpdated, setLastUpdated] = useState<Date>();

  useEffect(() => {
    async function refresh() {
      await Promise.all(
        [
          Api.getRespositories().then(setRepositories),
          Api.getLatestVersions().then(setVersions)
        ]
      );

      setLastUpdated(new Date());
    }

    refresh();

    const timer = setTimeout(refresh, 60 * 1000);
    return () => clearTimeout(timer);
  }, []);

  repositories.map((repo) => console.log(repo));

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
        {repositories.map((repo) => <RepositoryListItem repo={repo} versions={versions} />)}
      </ListGroup>
    </div>
  );
}