import React from 'react';
import ReactDOM from 'react-dom/client';
import './custom.scss';
import Navigation from './components/Navigation';
import { Container, ThemeProvider } from 'react-bootstrap';
import HomePage from './pages/home';
import Footer from './components/Footer';
import RepositoryPage from './pages/repository';
import VersionPage from './pages/version';
import { RecoilRoot } from 'recoil';
import ThaliakContainer from './components/ThaliakContainer';
import { BrowserRouter, Route, Routes } from 'react-router-dom';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <ThemeProvider>
      <BrowserRouter>
        <RecoilRoot>
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
        </RecoilRoot>
      </BrowserRouter>
    </ThemeProvider>
  </React.StrictMode>
);
