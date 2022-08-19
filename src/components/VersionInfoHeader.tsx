import Version from '../api/types/version';

export default function VersionInfoHeader({ version, latest }: { version: Version, latest: boolean }) {
  let deco = 'fs-5';
  if (latest) {
    deco += ' fw-bold';
  } else if (!version.isActive) {
    deco += ' text-decoration-line-through';
  }

  return (
    <>
      <div className='relative grow max-w-full me-auto'>
        <span className={deco}>{version.versionString}</span>
        <span className='small ms-2'>{version.patches?.length} patch{version.patches?.length === 1 ? '' : 'es'}</span>
        {!version.isActive && <span className='small text-muted ms-3'>superseded</span>}
        {latest && <span className='small text-muted ms-3'>latest version</span>}
        {(version.prerequisiteVersions?.length ?? 0) > 0 && (
          <div className='small fst-italic text-muted mt-0'>
            requires
            {version.prerequisiteVersions!.length > 1 && ' one of'}:
            {' '}
            {Array.from(version.prerequisiteVersions!.map(v => v.versionString)).join(', ')}
          </div>
        )}
      </div>
      <div className='relative grow max-w-full'>
        <b>First seen:</b> {version.firstOffered ?? 'unknown'}<br />
        <b>Last seen:</b> {version.lastOffered ?? 'unknown'}
      </div>
    </>
  );
}