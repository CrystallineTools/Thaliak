import { ReactElement, useEffect } from 'react';
import Api from '../api/client';
import { useRecoilState } from 'recoil';
import { LAST_UPDATED, LATEST_VERSIONS, REPOSITORIES, VERSIONS } from '../store';
import { Spinner } from 'react-bootstrap';

export default function ThaliakContainer({ children }: { children: ReactElement }) {
  const [lastUpdated, setLastUpdated] = useRecoilState(LAST_UPDATED);
  const [repositories, setRepositories] = useRecoilState(REPOSITORIES);
  const [versions, setVersions] = useRecoilState(VERSIONS);
  const [latestVersions, setLatestVersions] = useRecoilState(LATEST_VERSIONS);

  useEffect(() => {
    async function refresh() {
      await Promise.all(
        [
          Api.getRepositories().then(setRepositories),
          Api.getAllVersions().then(setVersions),
          Api.getLatestVersions().then(setLatestVersions)
        ]
      );

      setLastUpdated(new Date());
    }

    refresh();

    const timer = setInterval(refresh, 60 * 1000);
    return () => clearInterval(timer);
  }, []);

  if (!lastUpdated) {
    return <Spinner animation='border' />;
  }

  return children;
}
