import React from 'react';
import ReactDOM from 'react-dom/client';
import './custom.css';
import TopNavigation from './components/navigation/TopNavigation';
import HomePage from './pages/home';
import Footer from './components/Footer';
import RepositoryPage from './pages/repository';
import VersionPage from './pages/version';
import ThaliakContainer from './components/ThaliakContainer';
import { BrowserRouter, Route, Routes } from 'react-router';
import GraphQLPage from './pages/graphql';
import WebhooksPage from './pages/webhooks';
import { AuthProvider } from './contexts/AuthContext';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <BrowserRouter>
      <AuthProvider>
        <TopNavigation />

      <ThaliakContainer>
        <div className='bg-linear-to-br from-gray-50 to-gray-100 min-h-screen'>
          <div className='container mt-20 mx-auto px-4 sm:px-6 py-8 min-h-screen'>
            <Routes>
              <Route path='/' element={<HomePage />} />
              <Route path='/graphql' element={<GraphQLPage />} />
              <Route path='/graphql/:selectedVersion' element={<GraphQLPage />} />
              <Route path='/repository/:repoName' element={<RepositoryPage />} />
              <Route path='/version/:repoName/:versionId' element={<VersionPage />} />
              <Route path='/webhooks' element={<WebhooksPage />} />
            </Routes>
            <Footer />
          </div>
        </div>
      </ThaliakContainer>
      </AuthProvider>
    </BrowserRouter>
  </React.StrictMode>
);
