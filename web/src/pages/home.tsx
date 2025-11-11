import RepositoryDetail from '../components/RepositoryDetail';
import { gql, useQuery } from '@apollo/client';
import { Repository } from '../api/types/repository';
import { useEffect, useState } from 'react';
import Loading from '../components/Loading';
import ListGroup from '../components/list/ListGroup';
import { AudioAlert } from '../components/AudioAlert';

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
    return <Loading />;
  }

  return (
    <div className='animate-fade-in'>
      <div className='flex flex-wrap flex-col sm:flex-row mb-8 items-start sm:items-center gap-4'>
        <div className='grow'>
          <h1 className='text-4xl font-bold text-gray-900 mb-2'>Game Version Repositories</h1>
        </div>
        <div className='flex flex-col items-end gap-2'>
          <AudioAlert repositories={data?.repositories} />
          <div className='flex items-center gap-2 text-sm text-gray-600 bg-gray-100 px-3 py-2 rounded-lg'>
            <svg className='w-4 h-4 text-primary-500 animate-pulse' fill='currentColor' viewBox='0 0 20 20'>
              <path fillRule='evenodd' d='M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z' clipRule='evenodd' />
            </svg>
            <span>Auto-updates every 15s</span>
          </div>
        </div>
      </div>

      <div className='space-y-8'>
        {groups.map((group: RepositoryGroup) => (
          <div key={group.name} className='animate-slide-up'>
            <div className='flex items-center gap-3 mb-4'>
              {GroupIcons.hasOwnProperty(group.name) && (
                <span className='text-5xl leading-none'>{GroupIcons[group.name]}</span>
              )}
              <h2 className='text-2xl font-semibold text-gray-800'>{group.name}</h2>
              <span className='ml-2 px-2.5 py-0.5 text-xs font-medium bg-gray-200 text-gray-700 rounded-full'>
                {group.repositories.length} {group.repositories.length === 1 ? 'repository' : 'repositories'}
              </span>
            </div>
            <ListGroup>
              {group.repositories.map((repo: Repository) => (
                <RepositoryDetail
                  repo={repo}
                  latestVersion={repo.latestVersion}
                  key={repo.slug}
                  linkName
                  showLatestVersionInfo
                />
              ))}
            </ListGroup>
          </div>
        ))}
      </div>
    </div>
  );
}