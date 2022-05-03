import { useEffect, useState } from 'react';
import Repository from '../types/repository';
import { ListGroup } from 'react-bootstrap';
import RepositoryListItem from '../components/RepositoryListItem';
import Version from '../types/version';

export default function Home() {
  const [repositories, setRepositories] = useState<Repository[]>([]);
  const [versions, setVersions] = useState<Version[]>([]);
  const [lastUpdated, setLastUpdated] = useState<Date>();

  useEffect(() => {
    async function refresh() {
      await Promise.all(
        [
          fetch('https://thaliak.xiv.dev/api/repositories')
            .then(async (response) => setRepositories(await response.json())),
          fetch('https://thaliak.xiv.dev/api/versions/all/latest')
            .then(async (response) => setVersions(await response.json()))
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