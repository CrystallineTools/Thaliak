import { GraphiQL } from 'graphiql';
import 'graphiql/graphiql.min.css';
import { useEffect, useMemo, useState } from 'react';
import { useTheme } from '@graphiql/react';
import cn from 'classnames';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCopy } from '@fortawesome/free-solid-svg-icons';
import { createGraphiQLFetcher } from '@graphiql/toolkit';
import { useParams } from 'react-router-dom';

const apiVersions = ['2022-08-14', 'beta'];
const defaultQuery = `query {
  repositories {
    id
    slug
    name
    description
    latestVersion {
      versionString
      firstOffered
      lastOffered
    }
  }
}
`;

export default function GraphQLPage() {
  const { selectedVersion } = useParams();
  const { setTheme } = useTheme();

  const [loading, setLoading] = useState(true);
  const [version, setVersion] = useState(apiVersions[0]);

  const endpoint = useMemo(() => `https://thaliak.xiv.dev/graphql/${version}`, [version]);
  const fetcher = useMemo(() => createGraphiQLFetcher({ url: endpoint }), [endpoint]);

  useEffect(() => {
    if (selectedVersion !== undefined) {
      if (apiVersions.includes(selectedVersion)) {
        setVersion(selectedVersion);
      } else {
        alert(`Invalid API version: ${selectedVersion}. Defaulting to ${version}.\nYou can select a version from the dropdown.`);
      }
    }

    setTheme('light');
    setLoading(false);
  }, []);

  return (
    <div className={cn('flex flex-col h-screen', { 'hidden': loading })}>
      <GraphiQL
        defaultQuery={defaultQuery}
        fetcher={fetcher}>
        <GraphiQL.Footer>
          <div className='flex flex-col px-1 py-2 space-y-2'>
            <div className='flex flex-row'>
              <strong className='w-1/6 font-bold whitespace-nowrap my-auto'>
                API Version
              </strong>
              <select
                className='w-5/6 bg-white rounded p-2 rounded-md my-auto'
                value={version}
                onChange={(e) => setVersion(e.target.value)}>
                {apiVersions.map((v) => (
                  <option key={v} value={v}>
                    {v}
                  </option>
                ))}
              </select>
            </div>
            <div className='flex flex-row'>
              <strong className='w-1/6 font-bold whitespace-nowrap my-auto'>
                API Endpoint
              </strong>
              <span className='w-5/6 my-auto'>
                {endpoint}
                <button
                  className='ml-2 text-gray-400 hover:text-gray-700 active:text-gray-900'
                  onClick={() => {
                    navigator.clipboard.writeText(endpoint);
                  }}>
                  <FontAwesomeIcon icon={faCopy} />
                </button>
              </span>
            </div>
          </div>
        </GraphiQL.Footer>
      </GraphiQL>
    </div>
  );
}
