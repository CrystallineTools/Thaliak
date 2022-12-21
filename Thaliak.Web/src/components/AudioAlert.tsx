import { useEffect, useState } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faVolumeHigh, faVolumeXmark } from '@fortawesome/free-solid-svg-icons';
import { Repository } from '../api/types/repository';
import ReactAudioPlayer from 'react-audio-player';

interface AudioAlertProps {
  repositories: Repository[] | undefined;
}

export function AudioAlert({ repositories }: AudioAlertProps) {
  const [prevVersions, setPrevVersions] = useState<{ [key: string]: string }>({});
  const [enabled, setEnabled] = useState(false);
  const [alarmActive, setAlarmActive] = useState(false);

  useEffect(() => {
    if (!repositories) {
      return;
    }

    let trigger = false;
    for (const repo of repositories) {
      if (repo.latestVersion) {
        if (repo.slug in prevVersions && prevVersions[repo.slug] !== repo.latestVersion.versionString) {
          trigger = true;
        }

        prevVersions[repo.slug] = repo.latestVersion.versionString;
      }
    }

    setPrevVersions(prevVersions);
    if (trigger) {
      console.log('TRIGGERED');
      setAlarmActive(true);
    }
  }, [repositories]);

  useEffect(() => {
    if (!enabled) {
      setAlarmActive(false);
    }
  }, [enabled]);

  return (
    <div className={`mb-1 underline ${enabled ? 'text-green-600' : 'text-black'}`}>
      {alarmActive && (
        <button onClick={() => setAlarmActive(false)} type='button'
                className='py-1 px-2 mr-1 text-white text-center font-semibold text-xs rounded rounded-full bg-yellow-600'>Silence</button>
      )}
      {enabled && (
        <ReactAudioPlayer src='/alert.mp3' muted={!alarmActive} volume={1.0} autoPlay loop />
      )}
      <a onClick={() => setEnabled(!enabled)}>
        <FontAwesomeIcon icon={enabled ? faVolumeHigh : faVolumeXmark} size='lg' className='mr-2' />
        Audio alerts {enabled ? 'enabled' : 'disabled'}. Click to {enabled ? 'disable' : 'enable'}.
      </a>
    </div>
  );
}