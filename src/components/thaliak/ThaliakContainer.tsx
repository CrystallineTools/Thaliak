import { ReactElement } from 'react';
import ThaliakSideNavigation from './navigation/ThaliakSideNavigation';
import MainContainer from '../shared/MainContainer';
import { ApolloClient, ApolloProvider, InMemoryCache } from '@apollo/client';

const gqlClient = new ApolloClient({
  uri: process.env.REACT_APP_GRAPHQL_URL ?? 'https://thaliak.xiv.dev/graphql',
  cache: new InMemoryCache(),
});

export default function ThaliakContainer(): ReactElement {
  return (
    <ApolloProvider client={gqlClient}>
      <MainContainer nav={<ThaliakSideNavigation />} />
    </ApolloProvider>
  );
}
