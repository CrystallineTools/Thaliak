import Version from '../../api/thaliak/types/version';
import VersionDetail from './VersionDetail';

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
  //   <ListGroup.Item eventKey={version.versionString} action to={`/thaliak/version/${repoName}/${version.versionString}`} as={Link}>
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