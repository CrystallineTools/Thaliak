import { RemixServer } from '@remix-run/react';
import { renderToString } from 'react-dom/server';
import { EntryContext } from '@remix-run/node';
import { ApolloClient, ApolloProvider, createHttpLink, InMemoryCache } from '@apollo/client';
import { getDataFromTree } from '@apollo/client/react/ssr';
import { StrictMode } from 'react';

export default async function handleRequest(
  request: Request,
  responseStatusCode: number,
  responseHeaders: Headers,
  remixContext: EntryContext
) {
  const client = new ApolloClient({
    ssrMode: true,
    cache: new InMemoryCache(),
    link: createHttpLink({
      uri: 'https://thaliak.xiv.dev/graphql/2022-08-14'
    }),
  });
  
  const App = (
    <StrictMode>
      <ApolloProvider client={client}>
        <RemixServer context={remixContext} url={request.url}/>
      </ApolloProvider>
    </StrictMode>
  );
  
  await getDataFromTree(App);
  
  const initialState = client.extract();
  let markup = renderToString(
    <>
      {App}
      <script
        dangerouslySetInnerHTML={{
          __html: `window.__APOLLO_STATE__=${JSON.stringify(
            initialState
          ).replace(/</g, "\\u003c")}`, // The replace call escapes the < character to prevent cross-site scripting attacks that are possible via the presence of </script> in a string literal
        }}
      />
    </>
  );
  
  responseHeaders.set('Content-Type', 'text/html');
  
  return new Response('<!DOCTYPE html>' + markup, {
    status: responseStatusCode,
    headers: responseHeaders,
  });
}
