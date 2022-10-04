import RepositoryDetail from '../../components/thaliak/RepositoryDetail';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTriangleExclamation } from '@fortawesome/free-solid-svg-icons';
import { discordLink } from '../../constants';
import { gql, useQuery } from '@apollo/client';
import { Repository } from '../../api/thaliak/types/repository';
import { useEffect, useState } from 'react';
import Loading from '../../components/shared/Loading';
import ListGroup from '../../components/shared/list/ListGroup';
import { AudioAlert } from '../../components/thaliak/AudioAlert';

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

export default function ThaliakHomePage() {
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
    return <Loading />;
  }

  return (
    <div>
      <div className='relative p-3 mb-4 border rounded bg-orange-200 border-orange-300 text-orange-800' role='alert'>
        <div className='bg-orange-200 border-orange-300 text-orange-800'>
          <h5 className='mb-1 text-xl font-bold'>
            <FontAwesomeIcon icon={faTriangleExclamation} className='mr-2' />
            REST API Deprecation & Removal Schedule
          </h5>
          <p className='mb-0'>
            Thaliak's <a href='src/pages/thaliak/home'>GraphQL API</a> was released on August 14, 2022.<br />
            The legacy REST API was deprecated on the same date. No new features will be added to the REST API.<br />
            <br />
            Barring the unexpected, <b>the REST API will be discontinued after October 15, 2022</b>.<br />
            Please migrate all applications to the GraphQL API before this date.<br />
            <br />
            For questions or support, <a href={discordLink}>join The Ewer Discord server</a>.
          </p>
        </div>
      </div>

      <div className='flex flex-wrap flex-col sm:flex-row mb-2'>
        <div className='relative flex-grow max-w-full flex-1'>
          <span className='text-3xl font-bold'>Game Version Repositories</span>
        </div>
        <div className='sm:w-1/2 sm:text-end text-gray-700 text-sm my-auto'>
          <AudioAlert repositories={data?.repositories} />
          Automatically updates every 15 seconds.
        </div>
      </div>

      {groups.map((group: RepositoryGroup) => (
        <div key={group.name}>
          <div className='flex flex-wrap mb-1'>
            <div className='relative flex-grow max-w-full flex-1'>
              <span className='text-2xl'>
                {GroupIcons.hasOwnProperty(group.name) &&
                  <span className='text-5xl align-middle mr-2'>{GroupIcons[group.name]}</span>
                }
                <span className='font-semibold'>
                  {group.name}
                </span>
              </span>
            </div>
          </div>
          <ListGroup>
            {group.repositories.map((repo: Repository) =>
              <RepositoryDetail repo={repo}
                                latestVersion={repo.latestVersion}
                                key={repo.slug}
                                linkName
                                showLatestVersionInfo
              />)}
          </ListGroup>
        </div>
      ))}

    </div>
  );
}