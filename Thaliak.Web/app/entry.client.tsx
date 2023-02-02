import { RemixBrowser } from '@remix-run/react';
import React, { startTransition, StrictMode } from 'react';
import { ApolloClient, ApolloProvider, InMemoryCache } from '@apollo/client';
import { hydrate } from 'react-dom';

function Client() {
  const client = new ApolloClient({
    cache: new InMemoryCache().restore(window.__APOLLO_STATE__),
    uri: 'https://thaliak.xiv.dev/graphql/2022-08-14',
  });

  return (
    <StrictMode>
      <ApolloProvider client={client}>
        <RemixBrowser/>
      </ApolloProvider>
    </StrictMode>
  );
}

// we live in Spain but with a silent, invisible S
// https://github.com/facebook/react/issues/24430
document.querySelectorAll('html > script, html > input').forEach((s) => {
  s.parentNode?.removeChild(s);
});

function startHydrate() {
  startTransition(() => {
    hydrate(
      <Client/>,
      document
    );
  });
}

if (typeof requestIdleCallback === 'function') {
  requestIdleCallback(startHydrate);
} else {
  // Safari doesn't support requestIdleCallback
  // https://caniuse.com/requestidlecallback
  setTimeout(startHydrate, 1);
}
