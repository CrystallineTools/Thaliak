import { Accordion, ListGroup } from 'react-bootstrap';
import Version from '../api/types/version';
import { useState } from 'react';
import Patch from '../api/types/patch';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faAngleRight } from '@fortawesome/free-solid-svg-icons';
import VersionInfoHeader from './VersionInfoHeader';
import { Link } from 'react-router-dom';

export interface VersionListItemArgs {
  repoName: string;
  version: Version;
  latest: Version;
}

export default function VersionListItem({ repoName, version, latest }: VersionListItemArgs) {
  const isLatest = version.version === latest.version;

  // if, in the rare case we have multiple patches, account for it accordingly in the version info
  let firstSeen: Date | undefined, lastSeen: Date | undefined;
  const prereqs: Set<string> = new Set();
  for (const patch of version.patches) {
    const fs = new Date(patch.firstSeen);
    if (!firstSeen || fs < firstSeen) {
      firstSeen = fs;
    }

    const ls = new Date(patch.lastSeen);
    if (!lastSeen || ls > lastSeen) {
      lastSeen = ls;
    }

    patch.prerequisitePatches.forEach((p) => prereqs.add(p));
  }

  const isSuperseded = lastSeen && lastSeen < new Date(new Date(latest.patches[0].lastSeen).getTime() - 1000 * 60);

  let deco = 'fs-5';
  if (isLatest) {
    deco += ' fw-bold';
  } else if (isSuperseded) {
    deco += ' text-decoration-line-through';
  }

  return (
    <ListGroup.Item eventKey={version.version} action to={`/version/${repoName}/${version.version}`} as={Link}>
      <div className='row'>
        <VersionInfoHeader version={version} latest={latest} />

        <div className='col-1 justify-content-end align-items-center d-flex'>
          <FontAwesomeIcon icon={faAngleRight} />
        </div>
      </div>
    </ListGroup.Item>
  );
}