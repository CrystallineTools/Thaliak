import { useParams } from 'react-router';
import VersionDetail from '../components/VersionDetail';
import Loading from '../components/Loading';
import { useEffect, useState } from 'react';
import { getPatch, getRepository, Patch } from '../api/v2client';

export default function VersionPage() {
  const { repoName, versionId } = useParams();

  const [loading, setLoading] = useState(true);
  const [patch, setPatch] = useState<Patch | null>(null);
  const [isLatest, setIsLatest] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      if (!repoName || !versionId) return;

      try {
        setLoading(true);

        // Fetch the specific patch and repository info
        const [patchData, repoData] = await Promise.all([
          getPatch(repoName, versionId),
          getRepository(repoName)
        ]);

        setPatch(patchData);
        setIsLatest(patchData.version_string === repoData.latest_patch?.version_string);
      } catch (error) {
        console.error('Error fetching patch:', error);
        setPatch(null);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [repoName, versionId]);

  if (loading) {
    return <Loading />;
  }

  if (!patch) {
    return (
      <div className='bg-white rounded-lg shadow-soft p-8 text-center'>
        <svg className='w-16 h-16 text-gray-400 mx-auto mb-4' fill='none' stroke='currentColor' viewBox='0 0 24 24'>
          <path strokeLinecap='round' strokeLinejoin='round' strokeWidth={2} d='M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z' />
        </svg>
        <p className='text-xl text-gray-600'>Version not found.</p>
      </div>
    );
  }

  return (
    <div className='animate-fade-in'>
      <div className='bg-white rounded-lg shadow-soft overflow-hidden'>
        <VersionDetail patch={patch} latest={isLatest} />
      </div>
    </div>
  );
}
