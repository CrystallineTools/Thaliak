import { useEffect, useState, useRef } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faVolumeHigh, faVolumeXmark, faPlay } from '@fortawesome/free-solid-svg-icons';
import { Repository } from '../api/v2client';

interface AudioAlertProps {
  repositories: Repository[] | undefined;
}

export function AudioAlert({ repositories }: AudioAlertProps) {
  const [prevVersions, setPrevVersions] = useState<{ [key: string]: string }>({});
  const [enabled, setEnabled] = useState(false);
  const [alarmActive, setAlarmActive] = useState(false);
  const audioRef = useRef<HTMLAudioElement | null>(null);

  // Initialize audio element on mount
  useEffect(() => {
    const audio = new Audio('/alert.mp3');
    audio.loop = true;
    audio.volume = 1.0;
    audioRef.current = audio;

    return () => {
      // Cleanup: stop and remove audio on unmount
      if (audioRef.current) {
        audioRef.current.pause();
        audioRef.current = null;
      }
    };
  }, []);

  // Handle alarm activation - play/stop audio
  useEffect(() => {
    if (!audioRef.current) return;

    if (alarmActive && enabled) {
      // Play audio
      const playPromise = audioRef.current.play();
      if (playPromise !== undefined) {
        playPromise
          .then(() => {
            console.log('Audio alert playing successfully');
          })
          .catch((error) => {
            console.error('Failed to play audio alert:', error);
            // Browser blocked autoplay - user needs to interact first
          });
      }
    } else {
      // Stop audio
      audioRef.current.pause();
      audioRef.current.currentTime = 0;
    }
  }, [alarmActive, enabled]);

  // Check for version changes
  useEffect(() => {
    if (!repositories) {
      return;
    }

    let trigger = false;
    const newVersions: { [key: string]: string } = {};

    for (const repo of repositories) {
      if (repo.latest_patch) {
        // Check if version changed (and we have a previous version to compare)
        if (repo.slug in prevVersions && prevVersions[repo.slug] !== repo.latest_patch.version_string) {
          trigger = true;
          console.log(`Version changed for ${repo.slug}: ${prevVersions[repo.slug]} -> ${repo.latest_patch.version_string}`);
        }

        newVersions[repo.slug] = repo.latest_patch.version_string;
      }
    }

    // Only update state if there are changes
    if (JSON.stringify(prevVersions) !== JSON.stringify(newVersions)) {
      setPrevVersions(newVersions);
    }

    if (trigger && enabled) {
      console.log('TRIGGERED - Activating audio alert');
      setAlarmActive(true);
    }
  }, [repositories, prevVersions, enabled]);

  // Disable alarm when alerts are disabled
  useEffect(() => {
    if (!enabled) {
      setAlarmActive(false);
    }
  }, [enabled]);

  const handleEnableToggle = async () => {
    if (!enabled && audioRef.current) {
      // When enabling, play a short test sound to "unlock" audio in browser
      try {
        audioRef.current.currentTime = 0;
        await audioRef.current.play();
        audioRef.current.pause();
        audioRef.current.currentTime = 0;
        console.log('Audio unlocked successfully');
      } catch (error) {
        console.warn('Could not unlock audio:', error);
      }
    }
    setEnabled(!enabled);
  };

  const handleTestSound = async () => {
    if (!audioRef.current) return;

    try {
      audioRef.current.currentTime = 0;
      await audioRef.current.play();
      // Play for 2 seconds then stop
      setTimeout(() => {
        if (audioRef.current) {
          audioRef.current.pause();
          audioRef.current.currentTime = 0;
        }
      }, 2000);
    } catch (error) {
      console.error('Failed to play test sound:', error);
      alert('Failed to play sound. Please check your browser permissions and volume.');
    }
  };

  return (
    <div className='flex items-center gap-2'>
      {alarmActive && (
        <button
          onClick={() => setAlarmActive(false)}
          type='button'
          className='px-3 py-1.5 text-xs font-medium text-white bg-amber-500 hover:bg-amber-600 rounded-lg transition-colors shadow-xs animate-pulse'>
          Silence Alert
        </button>
      )}
      <button
        onClick={handleEnableToggle}
        className={`inline-flex items-center gap-2 px-3 py-1.5 text-xs font-medium rounded-lg transition-all ${
          enabled
            ? 'bg-green-100 text-green-700 hover:bg-green-200'
            : 'bg-gray-100 text-gray-600 hover:bg-gray-200'
        }`}
        type='button'>
        <FontAwesomeIcon icon={enabled ? faVolumeHigh : faVolumeXmark} />
        <span>Audio alerts {enabled ? 'enabled' : 'disabled'}</span>
      </button>
      {enabled && (
        <button
          onClick={handleTestSound}
          className='inline-flex items-center gap-1.5 px-2.5 py-1.5 text-xs font-medium text-blue-600 bg-blue-50 hover:bg-blue-100 rounded-lg transition-colors'
          type='button'
          title='Test audio alert'>
          <FontAwesomeIcon icon={faPlay} className='text-xs' />
          <span>Test</span>
        </button>
      )}
    </div>
  );
}
