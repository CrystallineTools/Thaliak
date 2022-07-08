import { useParams } from 'react-router-dom';
import VersionInfoHeader from '../components/VersionInfoHeader';
import { useRecoilValue } from 'recoil';
import { VERSIONS } from '../store';

export default function VersionPage() {
  const { repoName, versionId } = useParams();

  const versions = useRecoilValue(VERSIONS).filter((v) => v.repository.slug === repoName).reverse();
  const version = versions.find((v) => v.version === versionId);
  const latestVersion = versions[0];

  if (!version) {
    return <p>Version not found.</p>;
  }

  return (
    <>
      <div className='row'>
        <VersionInfoHeader version={version} latest={latestVersion} />
      </div>
    </>
  );
}
