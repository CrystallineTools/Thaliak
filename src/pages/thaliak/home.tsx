import RepositoryDetail from '../../components/thaliak/RepositoryDetail';
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