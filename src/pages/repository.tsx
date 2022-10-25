import { useParams } from 'react-router-dom';
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
    return <p>Repository not found.</p>;
  }

  return <>
    <RepositoryDetail
      repo={data.repository}
    />
    <ListGroup>
      {sortedVersions.map((v: Version) =>
        <VersionListItem
          repoName={data.repository.slug}
          key={v.versionString}
          version={v}
          latest={v.versionString === data.repository.latestVersion.versionString}
        />)}
    </ListGroup>
  </>;
}
