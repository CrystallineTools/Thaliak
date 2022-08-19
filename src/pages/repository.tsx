import { useParams } from 'react-router-dom';
import { Accordion, Spinner } from 'react-bootstrap';
import VersionListItem from '../components/VersionListItem';
import { gql, useQuery } from '@apollo/client';
import Version from '../api/types/version';
import { useEffect, useState } from 'react';

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
    return <Spinner animation='border' />;
  }

  if (!data.repository) {
    return <p>Repository not found.</p>;
  }

  return <>
    <div className='row'>
      <div className='col'>
        <strong className='font-monospace'>{data.repository.slug}</strong>
        <br />
        {data.repository.description}
        <br />
        <span className='text-muted small'>{data.repository.name}</span>
      </div>
      <div className='col-3 text-end'>
      </div>
    </div>
    <div className='row mt-3'>
      <div className='col'>
        <Accordion>
          {sortedVersions.map((v: Version) =>
            <VersionListItem
              repoName={data.repository.slug}
              key={v.versionString}
              version={v}
              latest={v.versionString === data.repository.latestVersion.versionString}
            />)}
        </Accordion>
      </div>
    </div>
  </>;
}
