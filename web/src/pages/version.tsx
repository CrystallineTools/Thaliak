import { useParams } from 'react-router';
import VersionDetail from '../components/VersionDetail';
import { gql, useQuery } from '@apollo/client';
import Loading from '../components/Loading';

const QUERY = gql`
  query GetVersionInfo($repositorySlug: String!, $versionString: String!) {
    version(repositorySlug: $repositorySlug, versionString: $versionString) {
      id
      versionString
      isActive
      firstOffered
      lastOffered
      
      repository {
        latestVersion {
          id
          versionString
        }
      }

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
`;

export default function VersionPage() {
  const { repoName, versionId } = useParams();

  const { loading, data } = useQuery(QUERY, {
    variables: {
      repositorySlug: repoName,
      versionString: versionId,
    }
  });

  if (loading) {
    return <Loading />;
  }

  if (!data.version) {
    return (
      <div className='bg-white rounded-lg shadow-soft p-8 text-center'>
        <svg className='w-16 h-16 text-gray-400 mx-auto mb-4' fill='none' stroke='currentColor' viewBox='0 0 24 24'>
          <path strokeLinecap='round' strokeLinejoin='round' strokeWidth={2} d='M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z' />
        </svg>
        <p className='text-xl text-gray-600'>Version not found.</p>
      </div>
    );
  }

  const latest = data.version.repository.latestVersion.versionString === data.version.versionString;
  return (
    <div className='animate-fade-in'>
      <div className='bg-white rounded-lg shadow-soft overflow-hidden'>
        <VersionDetail version={data.version} latest={latest} />
      </div>
    </div>
  );
}
