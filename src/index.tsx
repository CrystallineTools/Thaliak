import React from 'react';
import ReactDOM from 'react-dom/client';
import './fonts/global-fonts.css';
import './custom.css';
import TopNavigation from './components/shared/navigation/top/TopNavigation';
import ThaliakHomePage from './pages/thaliak/home';
import Footer from './components/shared/Footer';
import ThaliakRepositoryPage from './pages/thaliak/repository';
import ThaliakVersionPage from './pages/thaliak/version';
import ThaliakContainer from './components/thaliak/ThaliakContainer';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { RootWikiPage } from './pages/wiki';
import HomePage from './pages/home';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <BrowserRouter>
      <TopNavigation />

      <Routes>
        <Route path='/' element={<HomePage />} />
        <Route path='/wiki/*' element={<RootWikiPage />} />

        <Route path='/thaliak' element={<ThaliakContainer />}>
          <Route path='repository/:repoName' element={<ThaliakRepositoryPage />} />
          <Route path='version/:repoName/:versionId' element={<ThaliakVersionPage />} />

          <Route index element={<ThaliakHomePage />} />
        </Route>
      </Routes>

      <Footer />
    </BrowserRouter>
  </React.StrictMode>
);
