import { useParams } from 'react-router';
import VersionListItem from '../components/VersionListItem';
import { useEffect, useState } from 'react';
import Loading from '../components/Loading';
import RepositoryDetail from '../components/RepositoryDetail';
import ListGroup from '../components/list/ListGroup';
import { getRepository, getRepositoryPatches, Repository, Patch } from '../api/v2client';

export default function RepositoryPage() {
  const { repoName } = useParams();

  const [loading, setLoading] = useState(true);
  const [repository, setRepository] = useState<Repository | null>(null);
  const [patches, setPatches] = useState<Patch[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      if (!repoName) return;

      try {
        setLoading(true);
        const [repo, patchesResponse] = await Promise.all([
          getRepository(repoName),
          getRepositoryPatches(repoName, { all: true })
        ]);

        setRepository(repo);

        const sortedPatches = patchesResponse.patches.slice().sort((a, b) => {
          const aVersion = a.version_string.replace(/^[HD]+/, '');
          const bVersion = b.version_string.replace(/^[HD]+/, '');
          return bVersion.localeCompare(aVersion);
        });
        setPatches(sortedPatches);
      } catch (error) {
        console.error('Error fetching repository:', error);
        setRepository(null);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [repoName]);

  if (loading) {
    return <Loading />;
  }

  if (!repository) {
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
        <RepositoryDetail repo={repository} />
      </div>

      <div>
        <div className='flex items-center justify-between mb-4'>
          <h2 className='text-2xl font-semibold text-gray-800'>Version History</h2>
          <span className='px-3 py-1 text-sm font-medium bg-primary-100 text-primary-700 rounded-full'>
            {patches.length} {patches.length === 1 ? 'patch' : 'patches'}
          </span>
        </div>
        <ListGroup>
          {patches.map((patch: Patch) => (
            <VersionListItem
              repoName={repository.slug}
              key={patch.version_string}
              patch={patch}
              latest={patch.version_string === repository.latest_patch?.version_string}
            />
          ))}
        </ListGroup>
      </div>
    </div>
  );
}
