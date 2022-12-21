import Version from '../api/types/version';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleRight } from '@fortawesome/free-solid-svg-icons';
import VersionDetail from './VersionDetail';
import { Link } from 'react-router-dom';

export interface VersionListItemArgs {
  repoName: string;
  version: Version;
  latest: boolean;
}

export default function VersionListItem({ repoName, version, latest }: VersionListItemArgs) {
  return (
    <div>
      <VersionDetail version={version} latest={latest} />
    </div>
  )
  // return (
  //   <ListGroup.Item eventKey={version.versionString} action to={`/version/${repoName}/${version.versionString}`} as={Link}>
  //     <div className='row'>
  //       <VersionInfoHeader version={version} latest={latest} />
  //
  //       <div className='col-1 justify-content-end align-items-center d-flex'>
  //         <FontAwesomeIcon icon={faAngleRight} />
  //       </div>
  //     </div>
  //   </ListGroup.Item>
  // );
}