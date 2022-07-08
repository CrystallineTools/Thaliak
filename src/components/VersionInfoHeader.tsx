import Version from '../api/types/version';

export default function VersionInfoHeader({ version, latest }: { version: Version, latest: Version }) {
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
    <>
      <div className='col me-auto'>
        <span className={deco}>{version.version}</span>
        <span className='small ms-2'>{version.patches.length} patch{version.patches.length === 1 ? '' : 'es'}</span>
        {isSuperseded ? <span className='small text-muted ms-3'>superseded</span> : null}
        {isLatest ? <span className='small text-muted ms-3'>latest version</span> : null}
        {prereqs.size > 0 && (
          <div className='small fst-italic text-muted mt-0'>
            requires
            {prereqs.size > 1 && ' one of'}:
            {' '}
            {Array.from(prereqs).join(', ')}
          </div>
        )}
      </div>
      <div className='col text-end'>
        <b>First seen:</b> {firstSeen?.toISOString() ?? 'unknown'}<br />
        <b>Last seen:</b> {lastSeen?.toISOString() ?? 'unknown'}
      </div>
    </>
  );
}