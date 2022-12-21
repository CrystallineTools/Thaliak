import { useParams } from 'react-router-dom';
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
    return <p>Version not found.</p>;
  }

  const latest = data.version.repository.latestVersion.versionString === data.version.versionString;
  return (
    <>
      <div className='row'>
        <VersionDetail version={data.version} latest={latest} />
      </div>
    </>
  );
}
