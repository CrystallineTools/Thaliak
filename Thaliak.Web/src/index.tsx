import React from 'react';
import ReactDOM from 'react-dom/client';
import './custom.css';
import TopNavigation from './components/navigation/TopNavigation';
import HomePage from './pages/home';
import Footer from './components/Footer';
import RepositoryPage from './pages/repository';
import VersionPage from './pages/version';
import ThaliakContainer from './components/ThaliakContainer';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { ApolloClient, ApolloProvider, InMemoryCache } from '@apollo/client';
import GraphQLPage from './pages/graphql';

const gqlClient = new ApolloClient({
  uri: process.env.REACT_APP_GRAPHQL_URL ?? 'https://thaliak.xiv.dev/graphql/2022-08-14',
  cache: new InMemoryCache(),
});

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <ApolloProvider client={gqlClient}>
      <BrowserRouter>
        <TopNavigation />

        <ThaliakContainer>
          <div className='container mt-16 mx-auto px-1 sm:px-4 min-h-screen'>
            <Routes>
              <Route path='/' element={<HomePage />} />
              <Route path='/graphql' element={<GraphQLPage />} />
              <Route path='/graphql/:selectedVersion' element={<GraphQLPage />} />
              <Route path='/repository/:repoName' element={<RepositoryPage />} />
              <Route path='/version/:repoName/:versionId' element={<VersionPage />} />
            </Routes>
            <Footer />
          </div>
        </ThaliakContainer>
      </BrowserRouter>
    </ApolloProvider>
  </React.StrictMode>
);
