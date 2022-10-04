import Version from '../../api/thaliak/types/version';

export default function VersionDetail({ version, latest }: { version: Version, latest: boolean }) {
  let deco = 'text-xl';
  if (latest) {
    deco += ' font-bold';
  } else if (!version.isActive) {
    deco += ' line-through';
  }

  return (
    <div className='relative block py-3 px-0 border-spacing-x-0 border-gray-300 no-underline'>
      <div className='flex flex-wrap flex-col sm:flex-row'>
        <div className='relative flex-grow max-w-full flex-1 whitespace-nowrap'>
          <span className={deco}>{version.versionString}</span>
          <span
            className='text-xs ml-2 whitespace-nowrap'>{version.patches?.length} patch{version.patches?.length === 1 ? '' : 'es'}</span>
          {!version.isActive && <span className='text-xs text-gray-500 ml-3 whitespace-nowrap'>superseded</span>}
          {latest && <span className='text-xs text-gray-500 ml-3 whitespace-nowrap'>latest version</span>}
          {(version.prerequisiteVersions?.length ?? 0) > 0 && (
            <div className='text-xs italic text-gray-500 mt-0'>
              requires
              {version.prerequisiteVersions!.length > 1 && ' one of'}:
              {' '}
              {Array.from(version.prerequisiteVersions!.map(v => v.versionString)).join(', ')}
            </div>
          )}
        </div>
        <div className='mt-2 sm:w-2/5 sm:mt-0 sm:text-end text-nowrap'>
          <b>First seen:</b> {version.firstOffered ?? 'unknown'}<br />
          <b>Last seen:</b> {version.lastOffered ?? 'unknown'}
        </div>
      </div>
    </div>
  );
}