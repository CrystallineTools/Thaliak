import React from 'react';
import ReactDOM from 'react-dom/client';
import './custom.scss';
import Navigation from './components/Navigation';
import { Container, ThemeProvider } from 'react-bootstrap';
import HomePage from './pages/home';
import Footer from './components/Footer';
import RepositoryPage from './pages/repository';
import VersionPage from './pages/version';
import ThaliakContainer from './components/ThaliakContainer';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { ApolloClient, ApolloProvider, InMemoryCache } from '@apollo/client';

const gqlClient = new ApolloClient({
  uri: process.env.REACT_APP_GRAPHQL_URL ?? 'https://thaliak.xiv.dev/graphql',
  cache: new InMemoryCache(),
});

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <ApolloProvider client={gqlClient}>
      <ThemeProvider>
        <BrowserRouter>
          <Navigation />

          <ThaliakContainer>
            <Container>
              <Routes>
                <Route path='/' element={<HomePage />} />
                <Route path='/repository/:repoName' element={<RepositoryPage />} />
                <Route path='/version/:repoName/:versionId' element={<VersionPage />} />
              </Routes>
            </Container>
          </ThaliakContainer>

          <Footer />
        </BrowserRouter>
      </ThemeProvider>
    </ApolloProvider>
  </React.StrictMode>
);
