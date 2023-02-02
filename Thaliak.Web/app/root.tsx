import { Outlet, Links, Meta, ScrollRestoration, Scripts, LiveReload } from '@remix-run/react';
import React from 'react';
import styles from './styles/app.css';
import TopNavigation from '~/components/navigation/TopNavigation';
import ThaliakContainer from '~/components/ThaliakContainer';
import Footer from '~/components/Footer';

export function links() {
  return [
    { rel: 'icon', href: '/favicon.ico' },
    { rel: 'apple-touch-icon', href: '/logo192.png' },
    { rel: 'manifest', href: '/manifest.json' },
    { rel: 'stylesheet', href: styles },
  ];
}

export function meta() {
  return {
    charset: 'utf-8',
    title: 'Thaliak',
    description: 'Patch/version tracker for Final Fantasy XIV',
    viewport: 'width=device-width, initial-scale=1',
    'theme-color': '#1e293b',
  };
}

export default function Root() {
  return (
    <html lang='en'>
      <head>
        <Meta/>
        <Links/>
      </head>
      <body>
        <TopNavigation/>

        <ThaliakContainer>
          <div className='container mt-16 mx-auto px-1 sm:px-4 min-h-screen'>
            <Outlet/>
          
            <Footer/>
          </div>
        </ThaliakContainer>

        <ScrollRestoration/>
        <Scripts/>
        <LiveReload/>
      </body>
    </html>
  );
}
