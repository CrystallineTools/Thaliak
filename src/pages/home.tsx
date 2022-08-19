import { ListGroup, Spinner } from 'react-bootstrap';
import RepositoryListItem from '../components/RepositoryListItem';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTriangleExclamation } from '@fortawesome/free-solid-svg-icons';
import { discordLink } from '../constants';
import { gql, useQuery } from '@apollo/client';
import { Repository } from '../api/types/repository';
import { useEffect, useState } from 'react';

const QUERY = gql`
  query GetRepositoriesWithLatestVersions {
    repositories {
      id
      name
      description
      slug
      latestVersion {
        id
        versionString
        firstOffered
        lastOffered
      }
    }
  }
`;

interface RepositoryGroup {
  name: string;
  repositories: Repository[];
}

const GroupIcons: { [key: string]: string } = {
  'FFXIV Global/JP': '🇺🇳',
  'FFXIV Korea': '🇰🇷',
  'FFXIV China': '🇨🇳'
};

export default function HomePage() {
  const { loading, data } = useQuery(QUERY, {
    fetchPolicy: 'cache-and-network',
    pollInterval: 15000
  });

  const [groups, setGroups] = useState<RepositoryGroup[]>([]);
  useEffect(() => {
    if (!data?.repositories) {
      setGroups([]);
      return;
    }

    const newGroups: RepositoryGroup[] = [];
    for (const repository of data.repositories) {
      const split = repository.description.split('-');
      const groupName = split[0].trim();

      let group = newGroups.find(g => g.name === groupName);
      if (!group) {
        group = { name: groupName, repositories: [] };
        newGroups.push(group);
      }

      group.repositories.push(repository);
    }

    setGroups(newGroups);
  }, [data]);

  if (loading) {
    return <Spinner animation='border' />;
  }

  return (
    <div>
      <div className='alert alert-warning mb-3' role='alert'>
        <div className='alert-warning'>
          <h5 className='card-title'>
            <FontAwesomeIcon icon={faTriangleExclamation} className='me-2' />
            REST API Deprecation & Removal Schedule
          </h5>
          <p className='card-text'>
            Thaliak's <a href='/graphql/'>GraphQL API</a> was released on August 14, 2022.<br />
            The legacy REST API was deprecated on the same date. No new features will be added to the REST API.<br />
            <br />
            Barring the unexpected, <b>the REST API will be discontinued after October 15, 2022</b>.<br />
            Please migrate all applications to the GraphQL API before this date.<br />
            <br />
            For questions or support, <a href={discordLink}>join the Thaliak Discord server</a>.
          </p>
        </div>
      </div>

      <div className='row mb-2'>
        <div className='col'>
          <h2>Game Version Repositories</h2>
        </div>
        <div className='col-3 text-end text-muted small my-auto'>
          Automatically updates every 15 seconds.
        </div>
      </div>

      {groups.map((group: RepositoryGroup) => (
        <div key={group.name}>
          <div className='row mb-1'>
            <div className='col'>
              <h4>
                {GroupIcons.hasOwnProperty(group.name) &&
                  <span className='fs-1 align-middle me-2'>{GroupIcons[group.name]}</span>
                }
                {group.name}
              </h4>
            </div>
          </div>
          <ListGroup className='mb-3'>
            {group.repositories.map((repo: Repository) => <RepositoryListItem repo={repo}
                                                                              latestVersion={repo.latestVersion}
                                                                              key={repo.slug} />)}
          </ListGroup>
        </div>
      ))}

    </div>
  );
}