import { useParams } from 'react-router';
import VersionListItem from '../components/VersionListItem';
import { gql, useQuery } from '@apollo/client';
import Version from '../api/types/version';
import { useEffect, useState } from 'react';
import Loading from '../components/Loading';
import RepositoryDetail from '../components/RepositoryDetail';
import ListGroup from '../components/list/ListGroup';

const QUERY = gql`
  query GetVersionList($repositorySlug: String!) {
    repository(slug: $repositorySlug) {
      id
      slug
      name
      description
      latestVersion {
        id
        versionString
      }

      versions {
        id
        versionId
        versionString
        isActive
        firstOffered
        lastOffered

        prerequisiteVersions {
          id
          versionString
        }

        dependentVersions {
          id
          versionString
        }

        patches {
          id
        }
      }
    }
  }
`;

export default function RepositoryPage() {
  const { repoName } = useParams();

  const { loading, data } = useQuery(QUERY, { variables: { repositorySlug: repoName } });
  const [sortedVersions, setSortedVersions] = useState<Version[]>([]);
  useEffect(() => {
    if (data?.repository?.versions) {
      const sorted = data.repository.versions.slice().sort((a: Version, b: Version) => b.versionId - a.versionId);
      setSortedVersions(sorted);
    } else {
      setSortedVersions([]);
    }
  }, [data]);

  if (loading) {
    return <Loading />;
  }

  if (!data.repository) {
    return (
      <div className='bg-white rounded-lg shadow-soft p-8 text-center'>
        <svg className='w-16 h-16 text-gray-400 mx-auto mb-4' fill='none' stroke='currentColor' viewBox='0 0 24 24'>
          <path strokeLinecap='round' strokeLinejoin='round' strokeWidth={2} d='M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z' />
        </svg>
        <p className='text-xl text-gray-600'>Repository not found.</p>
      </div>
    );
  }

  return (
    <div className='animate-fade-in space-y-6'>
      <div className='bg-white rounded-lg shadow-soft p-6'>
        <RepositoryDetail repo={data.repository} />
      </div>

      <div>
        <div className='flex items-center justify-between mb-4'>
          <h2 className='text-2xl font-semibold text-gray-800'>Version History</h2>
          <span className='px-3 py-1 text-sm font-medium bg-primary-100 text-primary-700 rounded-full'>
            {sortedVersions.length} {sortedVersions.length === 1 ? 'version' : 'versions'}
          </span>
        </div>
        <ListGroup>
          {sortedVersions.map((v: Version) => (
            <VersionListItem
              repoName={data.repository.slug}
              key={v.versionString}
              version={v}
              latest={v.versionString === data.repository.latestVersion.versionString}
            />
          ))}
        </ListGroup>
      </div>
    </div>
  );
}
