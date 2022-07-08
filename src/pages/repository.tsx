import { useParams } from 'react-router-dom';
import { Accordion } from 'react-bootstrap';
import VersionListItem from '../components/VersionListItem';
import { REPOSITORIES, VERSIONS } from '../store';
import { useRecoilValue } from 'recoil';

export default function RepositoryPage() {
  const { repoName } = useParams();

  const repo = useRecoilValue(REPOSITORIES).find((r) => r.slug === repoName);
  const versions = useRecoilValue(VERSIONS).filter((v) => v.repository.slug === repoName).reverse();

  if (!repo) {
    return <p>Repository not found.</p>;
  }

  return <>
    <div className='row'>
      <div className='col'>
        <strong className='font-monospace'>{repo.slug}</strong>
        <br />
        {repo.description}
        <br />
        <span className='text-muted small'>{repo.name}</span>
      </div>
      <div className='col-3 text-end'>
      </div>
    </div>
    <div className='row mt-3'>
      <div className='col'>
        <Accordion>
          {versions.map((v) => <VersionListItem repoName={repo.slug} version={v} latest={versions[0]} />)}
        </Accordion>
      </div>
    </div>
  </>;
}
